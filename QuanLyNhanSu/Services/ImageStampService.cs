using SkiaSharp;
using System.Drawing;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
namespace QuanLyNhanSu.Services
{
    public class ImageStampService
    {
        public async Task<string> AddStampAsync(
            IFormFile imageFile,
            string viTri,
            double latitude,
            double longitude)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Ảnh chấm công không hợp lệ.");

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(), "Images");

            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(folderPath, fileName);

            using var inputStream = imageFile.OpenReadStream();
            using var memoryStream = new MemoryStream();

            await inputStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var original = SKBitmap.Decode(memoryStream);

            if (original == null)
                throw new Exception("Không thể đọc file ảnh.");

            using var bitmap = new SKBitmap(
                original.Width, original.Height);

            using var canvas = new SKCanvas(bitmap);

            canvas.DrawBitmap(original, 0, 0);

            // Thông tin đóng dấu
            var thoiGian = DateTime.Now.ToString(
                "dd/MM/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);

            var lines = new[]
            {
                $"NGAY GIO: {thoiGian}",
                $"NOI CHAM CONG: {viTri}",
                $"GPS: {latitude.ToString("F6", CultureInfo.InvariantCulture)}, " +
                longitude.ToString("F6", CultureInfo.InvariantCulture)
            };

            // Kích thước dấu thay đổi theo ảnh
            float padding = Math.Max(8, original.Width * 0.025f);
            float fontSize = Math.Clamp(
                original.Width * 0.035f, 14f, 32f);

            using var typeface = SKTypeface.FromFamilyName(
                "Arial", SKFontStyle.Bold);

            using var font = new SKFont(typeface, fontSize);

            using var textPaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true
            };

            using var backgroundPaint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Fill
            };

            float lineHeight = fontSize * 1.4f;
            float maxWidth = original.Width - padding * 2;

            // Tự chia dòng nếu vị trí quá dài
            var wrappedLines = new List<string>();

            foreach (var line in lines)
            {
                var words = line.Split(' ');
                var current = "";

                foreach (var word in words)
                {
                    var candidate = string.IsNullOrEmpty(current)
                        ? word
                        : current + " " + word;

                    if (font.MeasureText(candidate) > maxWidth
                        && !string.IsNullOrEmpty(current))
                    {
                        wrappedLines.Add(current);
                        current = word;
                    }
                    else
                    {
                        current = candidate;
                    }
                }

                if (!string.IsNullOrEmpty(current))
                    wrappedLines.Add(current);
            }

            float stampHeight = padding * 2
                + lineHeight * wrappedLines.Count;

            // Nếu ảnh quá thấp, giảm cỡ chữ
            if (stampHeight > original.Height * 0.45f)
            {
                fontSize = Math.Max(10, fontSize * 0.75f);
                font.Size = fontSize;
                lineHeight = fontSize * 1.4f;
                stampHeight = padding * 2
                    + lineHeight * wrappedLines.Count;
            }

            stampHeight = Math.Min(stampHeight, original.Height);

            float top = original.Height - stampHeight;

            canvas.DrawRect(
                0, top, original.Width, stampHeight,
                backgroundPaint);

            float y = top + padding + fontSize;

            foreach (var line in wrappedLines)
            {
                canvas.DrawText(
                    line,
                    padding,
                    y,
                    SKTextAlign.Left,
                    font,
                    textPaint);

                y += lineHeight;
            }

            canvas.Flush();

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(
                SKEncodedImageFormat.Jpeg, 90);

            // Tạo mới hoặc ghi đè toàn bộ file
            await using (var output = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None))
            {
                data.SaveTo(output);
                await output.FlushAsync();
            }

            return fileName;
        }
    }
}
