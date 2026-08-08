using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.PluginShared;
using DShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DShop.AdminPlugin.Services
{
    public class IdentityCommandService : IIdentityCommandService
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly IIdentityQueryService _identityQueryService;
        private readonly ILogger<IdentityCommandService> _logger;

        public IdentityCommandService(DatabaseContext context, IConfiguration configuration, IIdentityQueryService identityQueryService, ILogger<IdentityCommandService> logger)
        {
            _context = context;
            _configuration = configuration;
            _identityQueryService = identityQueryService;
            _logger = logger;
        }

        // ==================== 用户管理 ====================

        public bool DeleteUser(long id, out string msg)
        {
            var userInfo = _context.Users.FirstOrDefault(it => it.Id == id);
            if (userInfo != null)
            {
                userInfo.IsDeleted = true;
                if (_context.SaveChanges() > 0)
                {
                    msg = "删除成功";
                    return true;
                }
                else
                {
                    msg = "删除失败";
                    return false;
                }
            }
            else
            {
                msg = "删除失败";
                return false;
            }
        }

        public LoginResponse Login(string username, string password, string captcha, string deviceInfo = "")
        {
            var result = new LoginResponse();
            var now = DateTime.Now;
            const int expireMinutes = 60;

            _logger.LogInformation("Login Service Update2");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                result.Success = false;
                result.Message = "帐号或密码为空";
                return result;
            }

            string hashedPassword = Md5Helper.ComputeMD5Hash(password);

            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.PasswordHash == hashedPassword);

            if (user == null)
            {
                result.Success = false;
                result.Message = "用户名或密码错误";
                return result;
            }

            // 权限来源：角色权限(主) ∪ 用户额外权限(加成)
            var roleIds = _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToList();

            var rolePermissionIds = _context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .ToHashSet();

            var userPermissionIds = _context.UserPermissions
                .Where(up => up.UserId == user.Id)
                .Select(up => up.PermissionId)
                .ToHashSet();

            var permissionIdList = rolePermissionIds.Union(userPermissionIds).ToList();

            var permissionCodeList = _context.Permissions
                .Where(it => permissionIdList.Contains(it.Id))
                .Select(it => it.PermissionCode)
                .ToList();

            string newToken = JwtHelper.GenerateJwtToken(user.Id.ToString(), user.Username, permissionCodeList.ToArray(), expireMinutes);

            _context.RefreshTokens.Add(new RefreshToken
            {
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(expireMinutes),
                Token = newToken,
                UserId = user.Id,
                DeviceInfo = deviceInfo,
                RevokedAt = now.AddMinutes(expireMinutes)
            });

            if (_context.SaveChanges() > 0)
            {
                result.Success = true;
                result.Message = "登录成功";
                result.Token = newToken;
            }
            else
            {
                result.Success = false;
                result.Message = "登录失败";
                result.Token = string.Empty;
            }

            return result;
        }

        public bool Register(string username, string password, out string msg)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                msg = "用户名、密码和验证码均为必填项";
                return false;
            }

            if (password.Length < 6)
            {
                msg = "密码长度至少为6位";
                return false;
            }

            var existingUser = _context.Users.Count(u => u.Username == username) > 0;
            if (existingUser)
            {
                msg = "用户名已被注册";
                return false;
            }

            var user = new User
            {
                Username = username,
                PasswordHash = Md5Helper.ComputeMD5Hash(password),
                Avatar = string.Empty,
                Email = string.Empty,
                CreatedAt = DateTime.Now,
                IdCard = string.Empty,
                Sex = "未知",
                MobilePhoneNumber = string.Empty,
                LastLoginAt = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = 0,
                ModifiedBy = 0,
                ModifiedAt = DateTime.Now,
            };

            _context.Users.Add(user);
            if (_context.SaveChanges() > 0)
            {
                msg = "注册成功";
                return true;
            }
            else
            {
                msg = "注册失败";
                return false;
            }
        }

        public bool ForgotPassword(string username, string oldPassword, string newPassword, string captcha, out string msg)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                msg = "帐号或密码为空";
                return false;
            }

            string hashedPassword = Md5Helper.ComputeMD5Hash(oldPassword);

            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.PasswordHash == hashedPassword);
            if (user == null)
            {
                msg = "用户名或密码错误";
                return false;
            }

            user.PasswordHash = Md5Helper.ComputeMD5Hash(newPassword);
            _context.Users.Update(user);
            if (_context.SaveChanges() > 0)
            {
                msg = "修改成功";
                return true;
            }
            else
            {
                msg = "修改失败";
                return false;
            }
        }

        public bool UpdatePassword(long id, string newPassword, string captcha, out string msg)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                msg = "用户不存在";
                return false;
            }

            user.PasswordHash = Md5Helper.ComputeMD5Hash(newPassword);
            _context.Users.Update(user);
            if (_context.SaveChanges() > 0)
            {
                msg = "修改成功";
                return true;
            }
            else
            {
                msg = "修改失败";
                return false;
            }
        }

        public bool Logout(long id, out string msg)
        {
            var now = DateTime.Now;
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                msg = "用户不存在";
                return false;
            }

            if (_identityQueryService.GetValidatedToken(id, out RefreshToken? tokenInfo))
            {
                user.LastLoginAt = now;
                tokenInfo.RevokedAt = now;
                _context.Update(user);
                _context.RefreshTokens.Update(tokenInfo);
                if (_context.SaveChanges() > 0)
                {
                    msg = "登出成功";
                    return true;
                }
                else
                {
                    msg = "登出失败";
                    return false;
                }
            }
            else
            {
                msg = "登出成功";
                return true;
            }
        }

        public bool UpdateUser(long id, UpdateUserRequest userRequest, out string msg)
        {
            var found = _context.Users.FirstOrDefault(u => u.Id == id);
            if (found == null)
            {
                msg = "用户不存在";
                return false;
            }

            if (!string.IsNullOrEmpty(userRequest.AvatarData))
            {
                string basePath = _configuration[Constants.FileStorageBasePath].ToString();
                string fullDir = CommonMethod.GetImageDirectory(basePath);
                string filename = Base64ImageSaver.SaveBase64Image(userRequest.AvatarData, fullDir);
                found.Avatar = CommonMethod.GetImageRelativePath(filename);
            }

            found.Sex = userRequest.Sex;
            found.Email = userRequest.Email;

            _context.Update(found);
            if (_context.SaveChanges() > 0)
            {
                msg = "修改成功";
                return true;
            }
            else
            {
                msg = "修改失败";
                return false;
            }
        }

        // ==================== 用户-菜单绑定 ====================

        public bool BindMenuList(long userId, List<long> menuIdList)
        {
            var existingRelations = _context.UserMenus
                .Where(um => um.UserId == userId)
                .ToList();

            var existingMenuIds = existingRelations
                .Select(um => um.MenuId)
                .ToHashSet();

            var toDelete = existingRelations
                .Where(um => !menuIdList.Contains(um.MenuId))
                .ToList();

            var toAdd = menuIdList
                .Where(id => !existingMenuIds.Contains(id))
                .Select(id => new UserMenu
                {
                    UserId = userId,
                    MenuId = id,
                    CreatedAt = DateTime.Now
                })
                .ToList();

            if (toDelete.Any())
            {
                _context.UserMenus.RemoveRange(toDelete);
            }
            if (toAdd.Any())
            {
                _context.UserMenus.AddRange(toAdd);
            }

            _context.SaveChanges();
            return true;
        }

        // ==================== 用户-权限绑定 ====================

        public (bool Success, string Message) BindPermissionList(long userId, List<long> permissionIdList)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existing = _context.UserPermissions.Where(up => up.UserId == userId);
                _context.UserPermissions.RemoveRange(existing);

                var newPermissions = permissionIdList.Select(permissionId => new UserPermission
                {
                    UserId = userId,
                    PermissionId = permissionId,
                    CreatedAt = DateTime.Now
                });
                _context.UserPermissions.AddRange(newPermissions);

                _context.SaveChanges();
                transaction.Commit();
                return (true, "绑定成功");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (false, "绑定失败：" + ex.Message);
            }
        }

        // ==================== 菜单管理 ====================

        public bool AddMenu(MenuCreateRequest request)
        {
            var menu = new Menu
            {
                Name = request.Name,
                Path = request.Path,
                Icon = request.Icon,
                ParentId = request.ParentId,
                SortOrder = request.SortOrder,
                CreatedAt = DateTime.Now
            };

            _context.Menus.Add(menu);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateMenu(MenuUpdateRequest request, out string msg)
        {
            var existing = _context.Menus.Find(request.Id);
            if (existing == null)
            {
                msg = "当前菜单不存在";
                return false;
            }

            if (request.ParentId != 0 && _context.Menus.Count(it => it.Id == request.ParentId) <= 0)
            {
                msg = "父级菜单不存在";
                return false;
            }

            existing.Name = request.Name;
            existing.Path = request.Path;
            existing.Icon = request.Icon;
            existing.ParentId = request.ParentId;
            existing.SortOrder = request.SortOrder;

            msg = "修改成功";
            return _context.SaveChanges() > 0;
        }

        public bool DeleteMenu(long id, out string msg)
        {
            var menu = _context.Menus.Find(id);
            if (menu == null)
            {
                msg = "该菜单不存在";
                return false;
            }

            if (_context.UserMenus.Where(it => it.MenuId == id).Count() > 0)
            {
                msg = "当前菜单已被用户使用";
                return false;
            }

            _context.Menus.Remove(menu);
            if (_context.SaveChanges() > 0)
            {
                msg = "删除成功";
                return true;
            }
            else
            {
                msg = "删除失败";
                return false;
            }
        }

        public bool DeleteMenus(IEnumerable<long> ids, out string msg)
        {
            var menus = _context.Menus.Where(m => ids.Contains(m.Id)).ToList();
            if (_context.UserMenus.Where(it => ids.Contains(it.MenuId)).Count() > 0)
            {
                msg = "当前菜单已被用户使用";
                return false;
            }

            _context.Menus.RemoveRange(menus);
            if (_context.SaveChanges() >= menus.Count())
            {
                msg = $"删除成功";
                return true;
            }
            else
            {
                msg = $"部分删除失败";
                return false;
            }
        }

        // ==================== 权限管理 ====================

        public bool AddPermission(Permission permission, out string msg)
        {
            int count = _context.Permissions.Where(it => it.PermissionCode == permission.PermissionCode).Count();
            if (count > 0)
            {
                msg = $"权限已经存在,{permission.PermissionCode}";
                return false;
            }
            _context.Permissions.Add(permission);
            if (_context.SaveChanges() > 0)
            {
                msg = "添加成功";
                return true;
            }
            else
            {
                msg = "添加失败";
                return false;
            }
        }

        public bool AddPermissions(IEnumerable<Permission> permissions, out string msg)
        {
            int count = _context.Permissions.Where(it => permissions.Select(it => it.PermissionCode).Contains(it.PermissionCode)).Count();
            if (count > 0)
            {
                msg = $"添加权限中有{count}条已经存在";
                return false;
            }
            _context.Permissions.AddRange(permissions);
            if (_context.SaveChanges() == permissions.Count())
            {
                msg = "添加成功";
                return true;
            }
            else
            {
                msg = "添加失败";
                return false;
            }
        }

        public bool UpdatePermission(Permission permission, out string msg)
        {
            var existing = _context.Permissions.Find(permission.Id);
            if (existing == null)
            {
                msg = "当前权限不存在";
                return false;
            }

            int count = _context.Permissions
                .Where(it => it.PermissionCode == permission.PermissionCode)
                .Where(it => it.Id != permission.Id)
                .Count();
            if (count > 0)
            {
                msg = $"权限已经存在,{permission.PermissionCode}";
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(permission);
            if (_context.SaveChanges() > 0)
            {
                msg = "修改成功";
                return true;
            }
            else
            {
                msg = "修改失败";
                return false;
            }
        }

        public bool DeletePermission(long id, out string msg)
        {
            var permission = _context.Permissions.Find(id);
            if (permission == null)
            {
                msg = "权限不存在";
                return false;
            }

            if (_context.UserPermissions.Where(it => it.PermissionId == permission.Id).Count() > 0)
            {
                msg = "当前权限已被用户使用";
                return false;
            }

            _context.Permissions.Remove(permission);
            if (_context.SaveChanges() > 0)
            {
                msg = "删除成功";
                return true;
            }
            else
            {
                msg = "删除失败";
                return false;
            }
        }

        public bool DeletePermissions(IEnumerable<long> ids, out string msg)
        {
            var permissions = _context.Permissions.Where(p => ids.Contains(p.Id));
            if (_context.UserPermissions.Where(it => ids.Contains(it.PermissionId)).Count() > 0)
            {
                msg = "当前权限已被用户使用";
                return false;
            }

            _context.Permissions.RemoveRange(permissions);
            if (_context.SaveChanges() == ids.Count())
            {
                msg = "删除成功";
                return true;
            }
            else
            {
                msg = "删除失败";
                return false;
            }
        }

        public void SeedPermissions()
        {
            SeedPermissions(Enumerable.Empty<Assembly>());
        }

        public void SeedPermissions(IEnumerable<Assembly> additionalAssemblies)
        {
            var assemblies = new List<Assembly> { Assembly.GetEntryAssembly()! };
            foreach (var referenced in Assembly.GetEntryAssembly()!.GetReferencedAssemblies())
            {
                try
                {
                    assemblies.Add(Assembly.Load(referenced));
                }
                catch { }
            }

            assemblies.AddRange(additionalAssemblies);

            var controllers = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);

            var permissionMap = new Dictionary<string, string>();

            foreach (var controller in controllers)
            {
                string? modulePrefix = null;
                var ns = controller.Namespace;

                if (ns != null)
                {
                    if (ns.Contains(".Admin"))
                        modulePrefix = "admin";
                    else if (ns.Contains(".App"))
                        modulePrefix = "app";
                }

                var actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var action in actions)
                {
                    var actionAttr = action.GetCustomAttribute<AuthorizePermissionAttribute>();
                    if (actionAttr != null)
                    {
                        var code = modulePrefix != null
                            ? $"{modulePrefix}::{actionAttr.PermissionCode}"
                            : actionAttr.PermissionCode;
                        var name = actionAttr.Name ?? code;
                        permissionMap[code] = name;
                    }
                }
            }

            var allPermissions = _context.Permissions.ToList();

            var existingPermissionDict = allPermissions
                .ToDictionary(p => p.PermissionCode);

            // 标记代码中已删除的权限
            foreach (var perm in allPermissions)
            {
                if (!permissionMap.ContainsKey(perm.PermissionCode))
                {
                    if (perm.Remark != "实体没有该标识")
                    {
                        perm.Remark = "实体没有该标识";
                        perm.UpdatedAt = DateTime.Now;
                    }
                }
            }

            foreach (var kv in permissionMap)
            {
                var code = kv.Key;
                var description = kv.Value;

                if (existingPermissionDict.TryGetValue(code, out var existing))
                {
                    var hasChanged = false;

                    if (existing.Description != description)
                    {
                        existing.Description = description;
                        hasChanged = true;
                    }

                    if (existing.Remark == "实体没有该标识")
                    {
                        existing.Remark = "实体没有该标识(fixed)";
                        hasChanged = true;
                    }

                    if (hasChanged)
                    {
                        existing.UpdatedAt = DateTime.Now;
                    }
                }
                else
                {
                    _context.Permissions.Add(new Permission
                    {
                        PermissionCode = code,
                        Description = description,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Remark = "更新成功",
                        SortOrder = 0,
                    });
                }
            }

            _context.SaveChanges();
        }

        // ==================== 角色管理 ====================

        public (bool Success, string Message) BindRoleList(long userId, List<long> roleIdList)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existing = _context.UserRoles.Where(ur => ur.UserId == userId);
                _context.UserRoles.RemoveRange(existing);

                var newRoles = roleIdList.Select(roleId => new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });
                _context.UserRoles.AddRange(newRoles);

                _context.SaveChanges();
                transaction.Commit();
                return (true, "绑定成功");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (false, "绑定失败：" + ex.Message);
            }
        }

        public long CreateRole(Role role, out string msg)
        {
            if (_context.Roles.Any(r => r.Code == role.Code))
            {
                msg = "角色编码已存在";
                return 0;
            }

            var entity = new Role
            {
                Code = role.Code,
                Name = role.Name,
                Description = role.Description,
                SortOrder = role.SortOrder,
                IsSystem = false
            };
            _context.Roles.Add(entity);
            if (_context.SaveChanges() > 0)
            {
                msg = "创建成功";
                return entity.Id;
            }
            msg = "创建失败";
            return 0;
        }

        public bool UpdateRole(Role role, out string msg)
        {
            var existing = _context.Roles.Find(role.Id);
            if (existing == null)
            {
                msg = "角色不存在";
                return false;
            }

            existing.Name = role.Name;
            existing.Description = role.Description;
            existing.SortOrder = role.SortOrder;

            if (_context.SaveChanges() > 0)
            {
                msg = "更新成功";
                return true;
            }
            msg = "更新失败";
            return false;
        }

        public bool DeleteRole(long id, out string msg)
        {
            var existing = _context.Roles.Find(id);
            if (existing == null)
            {
                msg = "角色不存在";
                return false;
            }
            if (existing.IsSystem)
            {
                msg = "内置角色不可删除";
                return false;
            }
            if (_context.UserRoles.Any(ur => ur.RoleId == id))
            {
                msg = "当前角色已被用户使用";
                return false;
            }

            _context.Roles.Remove(existing);
            if (_context.SaveChanges() > 0)
            {
                msg = "删除成功";
                return true;
            }
            msg = "删除失败";
            return false;
        }

        public bool BindRoleMenus(long roleId, List<long> menuIds)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existing = _context.RoleMenus.Where(rm => rm.RoleId == roleId);
                _context.RoleMenus.RemoveRange(existing);

                var newMenus = menuIds.Select(menuId => new RoleMenu
                {
                    RoleId = roleId,
                    MenuId = menuId
                });
                _context.RoleMenus.AddRange(newMenus);

                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool BindRolePermissions(long roleId, List<long> permissionIds)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existing = _context.RolePermissions.Where(rp => rp.RoleId == roleId);
                _context.RolePermissions.RemoveRange(existing);

                var newPermissions = permissionIds.Select(permissionId => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
                _context.RolePermissions.AddRange(newPermissions);

                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
