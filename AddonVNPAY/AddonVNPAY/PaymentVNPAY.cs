using AddonVNPAY.Models;
using CXS.Platform.DomainObjects;
using CXS.Retail.Extensibility;
using CXS.Retail.Extensibility.Modules.Transaction;
using CXS.SubSystem.Payment;
using CXS.SubSystem.Transaction;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace AddonVNPAY
{
    class PaymentVNPAY : TransactionPaymentModuleBase
    {
        // Bắt sự kiện trước khi user click button OK trong màn hình thanh toán (sau khi đã chọn phương thức thanh toán)
        public override void OnBeforeAddPayment(object sender, EventArgs<Transaction, TransactionPayment> args)
        {
            Transaction tran = args.Item;
            TransactionPayment tranPayment = args.ChildItem;
            string idPayType = tranPayment.PaymentType.Id;
            TenderType paytype = tranPayment.PaymentType.Type;
            if (paytype == TenderType.Custom && (idPayType == "VNPAYCard" || idPayType == "VNPAYQRCode"))
            {
                string popMappingFile = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\Models\\POSMapping.json";
                string popMappingText = File.ReadAllText(popMappingFile);
                List<POSMapping> lstPOSMapping = JsonConvert.DeserializeObject<List<POSMapping>>(popMappingText);

                // Lấy thông tin máy mPOS tương ứng với từng điểm bán hàng đã được setup
                POSMapping objMapping = lstPOSMapping.FirstOrDefault(x => x.StoreKey == tran.StoreKey && x.POSKey == tran.POSKey);
                if (objMapping == null)
                {
                    XtraMessageBox.Show("Không tìm thấy máy Smart POS tương ứng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    args.Cancel = true;
                    return;
                }
                string terminalCode = objMapping.TerminalCode;
                string SecretKey = "e30ef8e7c78d685d79445e9f3a2efd89"; // Ma bi mat dung de gui thanh toan

                if (idPayType == "VNPAYCard")
                {
                    // Tạo parameter call API
                    ParamApiCard para = new ParamApiCard();
                    para.userId = "userId"; // Ten cua khach hang sau khi ky hopj dong
                    para.orderCode = Guid.NewGuid().ToString();
                    para.terminalCode = terminalCode;
                    para.merchantCode = "FTI";
                    para.totalPaymentAmount = Money.ToInt64(tran.Balance);
                    para.successUrl = "https://success.url/";
                    para.cancelUrl = "https://cancel.url/";

                    paymentsCard _paymentsCard = new paymentsCard();
                    Card card = new Card();
                    card.merchantMethodCode = "PE4019B260584_SPOS_CARD";
                    card.methodCode = "VNPAY_SPOS_CARD";
                    card.clientTransactionCode = Guid.NewGuid().ToString();
                    card.amount = Money.ToInt64(tran.Balance);
                    _paymentsCard.card = card;

                    string strCheckSum = string.Format("{0}{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}", SecretKey, para.orderCode, para.userId, para.terminalCode, para.merchantCode, para.totalPaymentAmount, para.successUrl, para.cancelUrl, card.clientTransactionCode, card.merchantMethodCode, card.methodCode, card.amount);
                    para.checksum = CallAPI.SHA256(strCheckSum);
                    para.payments = _paymentsCard;
                    string jsonPara = JsonConvert.SerializeObject(para);

                    // Call API để tạo Transaction
                    ApiResult resultPaymentVNPAYCard = CallAPI.Payment(jsonPara);
                    // Tạo giao dịch thanh toán bằng VNPAY Card bị lỗi => Thông báo cho user biết lỗi
                    if (resultPaymentVNPAYCard.code != 200)
                    {
                        XtraMessageBox.Show(resultPaymentVNPAYCard.message, "GỬI YÊU CẦU THANH TOÁN LỖI", MessageBoxButtons.OK);
                        args.Cancel = true;
                        return;
                    }

                    Thread.Sleep(12000); // Đợi 15s trước khi call API tra cứu kết quả
                    bool tryGetDetail = true;
                    while (tryGetDetail)
                    {
                        Thread.Sleep(3000);
                        // Tạo giao dịch thanh toán bằng VNPAY Card thành công => Gọi API lấy kết quả giao dịch.
                        ParameterRessulAPI paraResult = new ParameterRessulAPI();
                        paraResult.merchantCode = para.merchantCode;
                        paraResult.terminalCode = terminalCode;
                        paraResult.orderCode = para.orderCode;
                        paraResult.paymentRequestId = resultPaymentVNPAYCard.paymentRequestId;
                        string secret = "0be8e913de1eb49723d4ade6db7c71cc"; // Mã bí mật dùng để lấy kết quả giao dịch
                        var strParaResultBody = string.Format("{0}|{1}|{2}|{3}", paraResult.terminalCode, paraResult.merchantCode, "", paraResult.orderCode);
                        paraResult.checksum = CallAPI.SHA256(secret + strParaResultBody);
                        string strParaResult = JsonConvert.SerializeObject(paraResult);
                        ApiResultOrderDetail resultOrderDetail = CallAPI.GetTransactionDetail(paraResult);
                        if (resultOrderDetail.code != 0)
                        {
                            XtraMessageBox.Show(resultOrderDetail.message, "LẤY KẾT QUẢ GIAO DỊCH LỖI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            args.Cancel = true;
                            return;
                        }
                        else
                        {
                            string _status = resultOrderDetail.data.transactions.First().status;
                            if (_status == "SUCCESS")
                            {
                                XtraMessageBox.Show("Thanh toán thành công!", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tryGetDetail = false;
                                return;
                            }
                            else if (_status == "WAITING" || _status == "PROCESSING")
                            {
                                tryGetDetail = true;
                            }
                            else if (_status == "FAILURE")
                            {
                                XtraMessageBox.Show(resultOrderDetail.message, "GIAO DỊCH THẤT BẠI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                tryGetDetail = false;
                                args.Cancel = true;
                                return;
                            }
                            else if (_status == "CANCELED")
                            {
                                XtraMessageBox.Show("Giao dịch bị hủy", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                tryGetDetail = false;
                                args.Cancel = true;
                                return;
                            }
                            else
                            {
                                XtraMessageBox.Show("Giao dịch thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                tryGetDetail = false;
                                args.Cancel = true;
                                return;
                            }
                        }
                    }
                }
                else if (idPayType == "VNPAYQRCode")
                {
                    // Tạo parameter call API
                    ParameterAPIQR para = new ParameterAPIQR();
                    para.totalPaymentAmount = Money.ToInt64(tran.Total);
                    para.userId = "toet";
                    para.orderCode = Guid.NewGuid().ToString();
                    para.successUrl = "https://success.url/";
                    para.cancelUrl = "https://cancel.url/";
                    para.merchantCode = "FTI";
                    para.terminalCode = terminalCode;
                    para.expiredDate = DateTime.Now.AddDays(1).ToString("yyMMddHHmm");

                    paymentsQR _paymentsQR = new paymentsQR();
                    QR qr = new QR();
                    qr.merchantMethodCode = "PE4019B260584_QRCODE";
                    qr.methodCode = "VNPAY_QRCODE";
                    qr.clientTransactionCode = Guid.NewGuid().ToString();
                    qr.amount = Money.ToInt64(tran.Balance);
                    _paymentsQR.qr = qr;

                    var strCheckSum = string.Format("{0}{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}", SecretKey, para.orderCode, para.userId, para.terminalCode, para.merchantCode, para.totalPaymentAmount, para.successUrl, para.cancelUrl, qr.clientTransactionCode, qr.merchantMethodCode, qr.methodCode, qr.amount);
                    para.checksum = CallAPI.SHA256(strCheckSum);
                    para.payments = _paymentsQR;
                    string strPara = JsonConvert.SerializeObject(para);

                    // Call API để tạo Transaction
                    ApiResult resultPaymentVNPAYQR = CallAPI.Payment(strPara);
                    // Tạo giao dịch thanh toán bằng VNPAY QR bị lỗi => Thông báo cho user biết lỗi
                    if (resultPaymentVNPAYQR.code != 200)
                    {
                        XtraMessageBox.Show(resultPaymentVNPAYQR.message, "GỬI YÊU CẦU THANH TOÁN LỖI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        args.Cancel = true;
                        return;
                    }

                    Thread.Sleep(12000); // Đợi 15s trước khi call API tra cứu kết quả
                    bool tryGetDetail = true;
                    while (tryGetDetail)
                    {
                        Thread.Sleep(3000);

                        // Tạo giao dịch thanh toán bằng VNPAY Card thành công => Gọi API lấy kết quả giao dịch.
                        ParameterRessulAPI paraResult = new ParameterRessulAPI();
                        paraResult.merchantCode = para.merchantCode;
                        paraResult.terminalCode = terminalCode;
                        paraResult.orderCode = para.orderCode;
                        paraResult.paymentRequestId = resultPaymentVNPAYQR.paymentRequestId;
                        string secret = "0be8e913de1eb49723d4ade6db7c71cc";
                        var strParaResultBody = string.Format("{0}|{1}|{2}|{3}", paraResult.terminalCode, paraResult.merchantCode, "", paraResult.orderCode);
                        paraResult.checksum = CallAPI.SHA256(secret + strParaResultBody);
                        string strParaResult = JsonConvert.SerializeObject(paraResult);
                        ApiResultOrderDetail resultOrderDetail = CallAPI.GetTransactionDetail(paraResult);
                        if (resultOrderDetail.code != 0)
                        {
                            XtraMessageBox.Show(resultOrderDetail.message, "LẤY KẾT QUẢ GIAO DỊCH LỖI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            args.Cancel = true;
                            return;
                        }
                        else
                        {
                            string _status = resultOrderDetail.data.transactions.First().status;
                            if (_status == "SUCCESS")
                            {
                                XtraMessageBox.Show("Thanh toán thành công!", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tryGetDetail = false;
                                return;
                            }
                            else if (_status == "WAITING" || _status == "PROCESSING")
                            {
                                tryGetDetail = true;
                            }
                            else if (_status == "FAILURE")
                            {
                                XtraMessageBox.Show(resultOrderDetail.message, "GIAO DỊCH THẤT BẠI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                tryGetDetail = false;
                                args.Cancel = true;
                                return;
                            }
                            else if (_status == "CANCELED")
                            {
                                XtraMessageBox.Show("Giao dịch bị hủy", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                tryGetDetail = false;
                                args.Cancel = true;
                                return;
                            }
                            else
                            {
                                XtraMessageBox.Show("Giao dịch thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                tryGetDetail = false;
                                args.Cancel = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
