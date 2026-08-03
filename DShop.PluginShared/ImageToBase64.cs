namespace DShop.PluginShared
{
    public static class ImageToBase64
    {
        /// <summary>
        /// 从文件路径获取图片并转换为 Base64 字符串
        /// </summary>
        /// <param name="imagePath">图片文件的完整物理路径</param>
        /// <param name="includeMimePrefix">是否包含 MIME 前缀（如 data:image/jpeg;base64,）</param>
        /// <returns>Base64 字符串，如果文件不存在或读取失败则返回 null</returns>
        public static string? GetBase64FromImage(string imagePath, bool includeMimePrefix = false)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentException("文件路径不能为空", nameof(imagePath));

            if (!File.Exists(imagePath))
                return null; // 或抛出异常，根据业务决定

            try
            {
                // 读取文件所有字节
                byte[] imageBytes = File.ReadAllBytes(imagePath);

                // 转换为 Base64
                string base64 = Convert.ToBase64String(imageBytes);

                if (includeMimePrefix)
                {
                    // 根据文件扩展名添加 MIME 前缀
                    string extension = Path.GetExtension(imagePath).ToLowerInvariant();
                    string mimeType = extension switch
                    {
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        ".bmp" => "image/bmp",
                        ".webp" => "image/webp",
                        ".svg" => "image/svg+xml",
                        _ => "application/octet-stream" // 默认二进制流
                    };
                    return $"data:{mimeType};base64,{base64}";
                }

                return base64;
            }
            catch (UnauthorizedAccessException)
            {
                // 无权限访问文件
                throw; // 或者返回 null，记录日志
            }
            catch (IOException ex)
            {
                // 其他 IO 错误（如文件被占用）
                throw new InvalidOperationException("读取文件时发生错误", ex);
            }
        }


    }
}
