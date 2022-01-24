using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddonVNPAY.Models
{
    public class ParameterAPIQR
    {
        public string userId { get; set; }
        public string checksum { get; set; }
        public string orderCode { get; set; }
        public paymentsQR payments { get; set; }
        public string cancelUrl { get; set; } = "https://cancel.url/";
        public string successUrl { get; set; } = "https://success.url/";
        public string terminalCode { get; set; }
        public string merchantCode { get; set; }
        public long totalPaymentAmount { get; set; }
        public string expiredDate { get; set; }


    }
    public class paymentsQR
    {
        public QR qr { get; set; }
    }
    public class QR
    {
        /*
         Mã kết nối thanh toán của do Payment Controller cung cấp
            khi triển khai dịch vụ.
            merchantMethodCode phải tồn tại trên hệ thống, đang Active
            và mapping với methodCode & terminalCode gửi lên
         */
        public string methodCode { get; set; }
        public long amount { get; set; }
        public int qrWidth { get; set; } = 400;
        public int qrHeight { get; set; } = 400;
        public int qrImageType { get; set; } = 0;
        public string customerPhone { get; set; } ="";
        public string merchantMethodCode { get; set; }
        
        public string clientTransactionCode { get; set; }
        
    }
}
