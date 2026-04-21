using QRCoder;

namespace EyelixEyewear_Project.Helpers
{
    public static class QrCodeHelper
    {
        // Trả về base64 string để nhúng thẳng vào <img> tag
        public static string GenerateQrCodeBase64(string uri)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(5); // 5 = pixel per module
            return Convert.ToBase64String(qrBytes);
        }
    }
}