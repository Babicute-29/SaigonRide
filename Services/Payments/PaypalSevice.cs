using QRCoder;
using System;

namespace SaigonRide.Services.Payments
{
    public class PaypalService
    {
        public string CreatePayPalQR(string username, double amount, string currency)
        {
            // Cấu trúc link PayPal.me để khách hàng thanh toán nhanh
            string payPalLink = $"https://www.paypal.me/{username}/{amount}{currency}";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(payPalLink, QRCodeGenerator.ECCLevel.Q))
                {
                    using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeImage = qrCode.GetGraphic(20);

                        // Chuyển sang Base64 để hiển thị trực tiếp lên trình duyệt
                        string base64String = Convert.ToBase64String(qrCodeImage);
                        return $"data:image/png;base64,{base64String}";
                    }
                }
            }
        }
    }
}