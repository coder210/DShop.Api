using DShop.Contracts;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 通用文件上传
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FileUploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 上传Base64图片
        /// </summary>
        /// <remarks>body: { "data": "data:image/png;base64,xxxx" }</remarks>
        [SwaggerOperation(Summary = "上传图片", Description = "上传Base64图片，返回相对URL")]
        [AuthorizePermission("file-upload:image", "上传图片")]
        [HttpPost("Image")]
        public IActionResult UploadImage([FromBody] UploadImageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Data))
            {
                return Ok(new ApiResponse { Code = 400, Msg = "图片数据不能为空" });
            }

            try
            {
                string basePath = _configuration[Constants.FileStorageBasePath] ?? "D:/Uploads/";
                string fullDir = CommonMethod.GetImageDirectory(basePath);
                string filename = Base64ImageSaver.SaveBase64Image(request.Data, fullDir);
                string relativeUrl = CommonMethod.GetImageRelativePath(filename);
                return Ok(new ApiResponse { Code = 200, Data = relativeUrl, Msg = "上传成功" });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Code = 400, Msg = "上传失败：" + ex.Message });
            }
        }
    }

    /// <summary>
    /// 上传图片请求
    /// </summary>
    public class UploadImageRequest
    {
        /// <summary>Base64图片数据（含或不含 data:image 前缀）</summary>
        public string Data { get; set; } = string.Empty;
    }
}
