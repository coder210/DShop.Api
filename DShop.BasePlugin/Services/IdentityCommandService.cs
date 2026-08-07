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

namespace DShop.BasePlugin.Services
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

            // 权限来源：角色权限 ∪ 用户直绑权限（通过 IdentityQueryService 汇总）
            var userPermissions = _identityQueryService.GetUserPermissions(user.Id);
            var permissionCodeList = userPermissions
                .Select(p => p.PermissionCode)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .ToList();

            // 角色来源：用户所有角色编码的并集
            var roleCodeList = _identityQueryService.GetUserRoleCodes(user.Id);

            string newToken = JwtHelper.GenerateJwtToken(
                user.Id.ToString(),
                user.Username,
                permissionCodeList.ToArray(),
                expireMinutes,
                roleCodeList.ToArray());

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
            // 启用重试执行策略，将事务整体作为可重试单元（EnableRetryOnFailure 下不允许直接 BeginTransaction）
            var strategy = _context.Database.CreateExecutionStrategy();
            return strategy.Execute(() =>
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    // 校验用户是否存在
                    if (_context.Users.FirstOrDefault(u => u.Id == userId) == null)
                    {
                        _logger.LogWarning("BindPermissionList 失败：用户 {UserId} 不存在", userId);
                        transaction.Rollback();
                        return (false, $"用户 {userId} 不存在");
                    }

                    // 校验传入的权限Id是否都存在于 Permissions 表，避免外键约束异常
                    var validPermissionIds = _context.Permissions
                        .Where(p => permissionIdList.Contains(p.Id))
                        .Select(p => p.Id)
                        .ToHashSet();
                    var invalidIds = permissionIdList.Where(id => !validPermissionIds.Contains(id)).ToList();
                    if (invalidIds.Any())
                    {
                        _logger.LogWarning("BindPermissionList 跳过不存在的权限Id: {InvalidIds}", string.Join(",", invalidIds));
                    }

                    var existingPermissions = _context.UserPermissions.Where(up => up.UserId == userId);
                    _context.UserPermissions.RemoveRange(existingPermissions);

                    var newPermissions = validPermissionIds.Select(permissionId => new UserPermission
                    {
                        UserId = userId,
                        PermissionId = permissionId,
                        CreatedAt = DateTime.Now
                    });
                    _context.UserPermissions.AddRange(newPermissions);

                    _context.SaveChanges();
                    transaction.Commit();

                    if (invalidIds.Any())
                    {
                        return (true, $"绑定成功，但以下权限Id在Permissions表中不存在已跳过: {string.Join(",", invalidIds)}");
                    }
                    return (true, "绑定成功");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BindPermissionList 异常：UserId={UserId}", userId);
                    transaction.Rollback();
                    return (false, $"绑定异常: {ex.Message}");
                }
            });
        }

        public (bool Success, string Message) BindRoleList(long userId, List<int> roleIdList)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return strategy.Execute(() =>
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    if (_context.Users.FirstOrDefault(u => u.Id == userId) == null)
                    {
                        _logger.LogWarning("BindRoleList 失败：用户 {UserId} 不存在", userId);
                        transaction.Rollback();
                        return (false, $"用户 {userId} 不存在");
                    }

                    var validRoleIds = _context.Roles
                        .Where(r => roleIdList.Contains(r.Id))
                        .Select(r => r.Id)
                        .ToHashSet();
                    var invalidIds = roleIdList.Where(id => !validRoleIds.Contains(id)).ToList();
                    if (invalidIds.Any())
                    {
                        _logger.LogWarning("BindRoleList 跳过不存在的角色Id: {InvalidIds}", string.Join(",", invalidIds));
                    }

                    var existingRoles = _context.UserRoles.Where(ur => ur.UserId == userId);
                    _context.UserRoles.RemoveRange(existingRoles);

                    var newRoles = validRoleIds.Select(roleId => new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                    });
                    _context.UserRoles.AddRange(newRoles);

                    _context.SaveChanges();
                    transaction.Commit();

                    if (invalidIds.Any())
                    {
                        return (true, $"绑定成功，但以下角色Id在Roles表中不存在已跳过: {string.Join(",", invalidIds)}");
                    }
                    return (true, "绑定成功");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BindRoleList 异常：UserId={UserId}", userId);
                    transaction.Rollback();
                    var detail = ex.InnerException != null ? $"（{ex.InnerException.Message}）" : string.Empty;
                    return (false, $"绑定异常: {ex.Message}{detail}");
                }
            });
        }

        // ==================== 角色管理 ====================

        public int CreateRole(Role role, out string msg)
        {
            msg = string.Empty;
            if (role == null || string.IsNullOrWhiteSpace(role.Code) || string.IsNullOrWhiteSpace(role.Name))
            {
                msg = "角色编码和名称不能为空";
                return 0;
            }
            if (_context.Roles.Any(r => r.Code == role.Code))
            {
                msg = $"角色编码 {role.Code} 已存在";
                return 0;
            }
            _context.Roles.Add(role);
            _context.SaveChanges();
            msg = "创建成功";
            return role.Id;
        }

        public bool UpdateRole(Role role, out string msg)
        {
            msg = string.Empty;
            var existing = _context.Roles.FirstOrDefault(r => r.Id == role.Id);
            if (existing == null)
            {
                msg = "角色不存在";
                return false;
            }
            if (_context.Roles.Any(r => r.Code == role.Code && r.Id != role.Id))
            {
                msg = $"角色编码 {role.Code} 已存在";
                return false;
            }
            existing.Code = role.Code;
            existing.Name = role.Name;
            existing.Description = role.Description;
            existing.SortOrder = role.SortOrder;
            existing.IsSystem = role.IsSystem;
            _context.SaveChanges();
            msg = "更新成功";
            return true;
        }

        public bool DeleteRole(int id, out string msg)
        {
            msg = string.Empty;
            var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                msg = "角色不存在";
                return false;
            }
            if (role.IsSystem)
            {
                msg = "系统角色不可删除";
                return false;
            }
            if (_context.UserRoles.Any(ur => ur.RoleId == id))
            {
                msg = "该角色已分配给用户，无法删除";
                return false;
            }
            _context.RoleMenus.RemoveRange(_context.RoleMenus.Where(rm => rm.RoleId == id));
            _context.RolePermissions.RemoveRange(_context.RolePermissions.Where(rp => rp.RoleId == id));
            _context.Roles.Remove(role);
            _context.SaveChanges();
            msg = "删除成功";
            return true;
        }

        public bool BindRoleMenus(int roleId, List<long> menuIds)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return strategy.Execute(() =>
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    if (_context.Roles.FirstOrDefault(r => r.Id == roleId) == null)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    var existing = _context.RoleMenus.Where(rm => rm.RoleId == roleId).ToList();
                    _context.RoleMenus.RemoveRange(existing);
                    var validIds = _context.Menus.Where(m => menuIds.Contains(m.Id)).Select(m => m.Id).ToHashSet();
                    var toAdd = validIds.Select(menuId => new RoleMenu
                    {
                        RoleId = roleId,
                        MenuId = menuId
                    }).ToList();
                    _context.RoleMenus.AddRange(toAdd);
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BindRoleMenus 异常：RoleId={RoleId}", roleId);
                    transaction.Rollback();
                    return false;
                }
            });
        }

        public bool BindRolePermissions(int roleId, List<long> permissionIds)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return strategy.Execute(() =>
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    if (_context.Roles.FirstOrDefault(r => r.Id == roleId) == null)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    var existing = _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToList();
                    _context.RolePermissions.RemoveRange(existing);
                    var validIds = _context.Permissions.Where(p => permissionIds.Contains(p.Id)).Select(p => p.Id).ToHashSet();
                    var toAdd = validIds.Select(permissionId => new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    }).ToList();
                    _context.RolePermissions.AddRange(toAdd);
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BindRolePermissions 异常：RoleId={RoleId}", roleId);
                    transaction.Rollback();
                    return false;
                }
            });
        }

        // ==================== 菜单管理 ====================

        public bool AddMenu(MenuCreateRequest request)
        {
            var menu = new Menu
            {
                Name = request.Name,
                Path = request.Path,
                Icon = request.Icon,
                Controller = request.Controller ?? "",
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
            existing.Controller = request.Controller ?? "";
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
                var actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var action in actions)
                {
                    var actionAttr = action.GetCustomAttribute<AuthorizePermissionAttribute>();
                    if (actionAttr != null)
                    {
                        var client = actionAttr.Client ?? "admin";
                        var code = $"{client}::{actionAttr.PermissionCode}";
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
    }
}
