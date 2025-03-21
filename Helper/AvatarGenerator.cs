using System;
using System.IO;
using SkiaSharp;

namespace hrms_api.Helper
{
    public static class AvatarGenerator
    {
        public static string GenerateAvatar(string name, string baseurl)
        {
            var userId = Guid.NewGuid().ToString(); // Unique filename
            string initials = GetInitials(name);
            string avatarPath = $"/Uploads/{userId}.png";  // Store in Uploads folder
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", avatarPath.TrimStart('/'));

            // Ensure the Uploads folder exists
            string uploadsDir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            using var bitmap = new SKBitmap(200, 200);
            using var canvas = new SKCanvas(bitmap);

            // Generate a consistent color based on the name
            var hash = name.GetHashCode();
            var color = new SKColor((byte)(hash & 255), (byte)((hash >> 8) & 255), (byte)((hash >> 16) & 255));

            // Draw background color
            canvas.Clear(color);

            // Draw initials
            using var paint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 80,
                TextAlign = SKTextAlign.Center,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            };

            float x = 100;
            float y = 120;
            canvas.DrawText(initials, x, y, paint);

            // Save the avatar image
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(fullPath);
            data.SaveTo(stream);

            return $"{baseurl}{avatarPath}";  // Return full URL instead of relative path
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "U"; // Default initial for unknown users

            var words = name.Split(' ');
            string initials = words.Length > 1
                ? $"{words[0][0]}{words[1][0]}" // First letter of first and last name
                : $"{words[0][0]}"; // First letter of the single name

            return initials.ToUpper();
        }
    }
}
