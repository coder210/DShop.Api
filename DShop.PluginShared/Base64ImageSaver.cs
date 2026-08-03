namespace DShop.PluginShared
{
    public static class Base64ImageSaver
    {
        /// <summary>
        /// 将Base64图片字符串保存到磁盘
        /// </summary>
        /// <param name="base64Image">包含或不包含前缀的Base64字符串</param>
        /// <param name="saveDirectory">保存目录（如不存会自动创建）</param>
        /// <param name="fileNameWithoutExtension">可选：自定义文件名（不含扩展名），如不提供则自动生成</param>
        /// <returns>保存后的完整文件路径</returns>
        public static string SaveBase64Image(string base64Image, string saveDirectory, string? fileNameWithoutExtension = null)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
                throw new ArgumentException("Base64字符串不能为空", nameof(base64Image));

            // 1. 去除前缀（如果有）
            string base64Data = base64Image;
            int commaIndex = base64Image.IndexOf(',');
            if (commaIndex >= 0)
            {
                base64Data = base64Image.Substring(commaIndex + 1);
            }

            // 2. 清理可能存在的空格/换行（可选）
            base64Data = base64Data.Replace(" ", "").Replace("\n", "").Replace("\r", "");

            // 3. 转换为字节数组
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("无效的Base64格式", ex);
            }

            // 4. 确定扩展名（从原始前缀中提取MIME）
            string extension = ".png"; // 默认
            if (base64Image.StartsWith("data:image/jpeg", StringComparison.OrdinalIgnoreCase))
                extension = ".jpg";
            else if (base64Image.StartsWith("data:image/png", StringComparison.OrdinalIgnoreCase))
                extension = ".png";
            else if (base64Image.StartsWith("data:image/gif", StringComparison.OrdinalIgnoreCase))
                extension = ".gif";
            else if (base64Image.StartsWith("data:image/bmp", StringComparison.OrdinalIgnoreCase))
                extension = ".bmp";

            // 5. 生成文件名
            string fileName;
            if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                // 自动生成：时间戳_随机Guid
                fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
            }
            else
            {
                // 使用自定义名称，但清理非法字符
                string safeName = string.Join("_", fileNameWithoutExtension.Split(Path.GetInvalidFileNameChars()));
                fileName = safeName + extension;
            }

            // 6. 确保目录存在
            Directory.CreateDirectory(saveDirectory);

            // 7. 写入文件
            string fullPath = Path.Combine(saveDirectory, fileName);
            File.WriteAllBytes(fullPath, imageBytes);

            return fileName;
        }


    }
}
