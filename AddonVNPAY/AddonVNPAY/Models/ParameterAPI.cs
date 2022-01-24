using System.Collections.Generic;

namespace AddonVNPAY.Models
{
    //public class ParameterTransactionAPI
    //{
    //    public string merchantCode { get; set; }
    //    /// <summary>
    //    /// Mã điểm thu của Đối tác trên hệ thống Payment Controll cung cấp tại thời điểm tích hợp.
    //    /// </summary>
    //    public string terminalCode { get; set; }
    //    /// <summary>
    //    /// Mã điểm thu của đối tác trên hệ thống Payment Controll.
    //    /// </summary>
    //    public string userId { get; set; }
    //    /// <summary>
    //    /// ID định danh của User trên hệ thống đối tác
    //    /// mặc định  = "userId"
    //    /// </summary>
    //    public string orderCode { get; set; }
    //    /// <summary>
    //    /// Mã đơn hàng/hoặc Id đơn hàng yêu cầu thanh toán
    //    /// </summary>
    //    public long totalPaymentAmount { get; set; }
    //    /// <summary>
    //    /// Số tiền thanh toán của đơn hàng
    //    /// </summary>
    //    //public string expiredDate { get; set; }
    //    /// <summary>
    //    /// Không bắt buộc
    //    /// thời gian hết hạn của thanh toán, theo formt uuMMddHHmm(24h)
    //    /// </summary>
    //    public List<payments> payments { get; set; }
    //    public string checksum { get; set; }


    //}

    public class ParamApiCard
    {
        public string userId { get; set; } 
        public string checksum { get; set; }
        public string orderCode { get; set; }
        public paymentsCard payments { get; set; }
        public string cancelUrl { get; set; } = "https://success.url/";
        public string successUrl { get; set; } = "https://cancel.url/";
        public string terminalCode { get; set; }
        public string merchantCode { get; set; }
        public long totalPaymentAmount { get; set; }
    }

    public class  paymentsCard
    {
        public Card card { get; set; } 
    }

    public class Card
    {
        /*
         Mã kết nối thanh toán của do Payment Controller cung cấp
            khi triển khai dịch vụ.
            merchantMethodCode phải tồn tại trên hệ thống, đang Active
            và mapping với methodCode & terminalCode gửi lên
         */
        public string merchantMethodCode { get; set; }
        public string methodCode { get; set; }
        public string clientTransactionCode { get; set; }
        public long amount { get; set; }
    }
    public class ParameterRessulAPI
    {
        public string merchantCode { get; set; }
        public string terminalCode { get; set; }
        public string orderCode { get; set; }
        public string paymentRequestId { get; set; }
        public string checksum { get; set; }
        

    }

}
