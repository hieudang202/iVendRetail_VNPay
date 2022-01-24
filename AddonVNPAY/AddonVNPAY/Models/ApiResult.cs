using System.Collections.Generic;

namespace AddonVNPAY.Models
{
    #region VNPAY CARD
    public class ApiResult
    {
        /// <summary>
        /// Theo format httpCode:
        /// 200: Thành công
        /// 401, 403: Thông tin clientCode hoặc mã hóa không hợp lệ
        /// 400: Dữ liệu không đúng định dạng hoặc lỗi nghiệp vụ
        /// </summary>
        public virtual int code { get; set; }

        /// <summary>
        /// True: Thành công, False: Thất bại
        /// </summary>
        public virtual bool IsSuccessful { get; set; }

        /// <summary>
        /// Mô tả chi tiết lỗi trong trường hợp thất bại
        /// </summary>
        public virtual string message { get; set; }

        /// <summary>
        /// Dữ liệu trả về trong trường hợp thành công
        /// </summary>
        public virtual SuccessData data { get; set; }
        public virtual string orderCode { get; set; }
        public virtual string paymentRequestId { get; set; }

        /// <summary>
        /// Dữ liệu trả về trong trường hợp thất bại
        /// </summary>
        public virtual ErrorData errors { get; set; }
    }
    public class SuccessData
    {
        public string merchantPartnerCode { get; set; }

        /// <summary>
        /// Mã giao dịch trên hệ thống Payment Hub của VNPAY
        /// </summary>
        public string psTransactionCode { get; set; }
    }
    public class ErrorData
    {
        /// <summary>
        /// Loại lỗi
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Mã lỗi của hệ thống
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Mô tả lỗi
        /// </summary>
        public string message { get; set; }
    }
    #endregion

    #region TRANSACTION DETAIL
    public class ApiResultOrderDetail
    {
        public int code { get; set; }
        public string message { get; set; }
        public data data{ get; set; }
    }
    public class data
    {
        public List<transsactions> transactions { get; set; }
    }

    public class transsactions
    {
        public string transactionCode { get; set; }
        public string orderCode { get; set; }
        public int amount { get; set; }
        public string partnerTransactionCode { get; set; }
        public string methodCode { get; set; }
        public string paymentRequestId{ get; set; }
        public string status { get; set; }
        public string partnerCode { get; set; }
        public string buyerPhone { get; set; }
        public string buyerName { get; set; }
        public string qrContent { get; set; }
    }

    #endregion
}
