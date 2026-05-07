using System;

namespace SaigonRide.Services.Payments
{
    public class MomoService
    {
        public string CreateVietQR(long amount, string description)
        {
            // Thông tin tài khoản của bạn (Ví dụ dùng ngân hàng MB: 970422 hoặc Sacombank: 970403)
            string bankId = "970422";
            string accountNo = "0789792300";
            string accountName = "NGUYEN GIA PHUC";
            string template = "qr_only"; // Giao diện thẻ QR gọn đẹp

            // Tạo link API VietQR (Số tiền 0 để mặc định, ta sẽ cập nhật động bằng JS ở View)
            string url = $"https://img.vietqr.io/image/{bankId}-{accountNo}-{template}.png" +
                         $"?amount={amount}" +
                         $"&addInfo={Uri.EscapeDataString(description)}" +
                         $"&accountName={Uri.EscapeDataString(accountName)}";

            return url;
        }
    }
}