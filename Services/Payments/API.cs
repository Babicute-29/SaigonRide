using QRCoder;
public class PaymentService 
{
    // Mục tạo mã MoMo
    public byte[] GenerateMomoQrCode(long amount, string note)
    {
        string phone = "0862410073";
        string name = "DINH NGOC HOA";
     
     //momo
        string data = $"2|99|{phone}|{name}||0|0|{amount}|{note}|transfer_myqr";
        
        return RenderQrImage(data);
    }

    // paypal
    public byte[] GeneratePaypalQrCode(double amount, string note)
    {
         // cũng nhập thông tin tk nhận tiền
        string email = "your-email@gmail.com";
    
        string url = $"https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business={email}&amount={amount}&currency_code=USD&item_name={Uri.EscapeDataString(note)}";
        
        return RenderQrImage(url);
    }




    private byte[] RenderQrImage(string payload)
    {
        using (var qrGenerator = new QRCodeGenerator())
        {
            var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}