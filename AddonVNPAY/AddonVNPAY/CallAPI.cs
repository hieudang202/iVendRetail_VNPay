using AddonVNPAY.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace AddonVNPAY
{
    public class CallAPI
    {
        public static ApiResult Payment(string data)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ApiResult result = new ApiResult();
            // Link test: https://payment-setting.dev.tekoapis.net/api/v2/payment/init/multi-v2
            // Link Production: https://payment-gateway.tekoapis.com/api/v2/payment/init/multi-v2

            var client = new RestClient("https://payment-setting.stag.tekoapis.net");
            var request = new RestRequest("api/v2/payment/init/multi-v2", Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", data, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            result = JsonConvert.DeserializeObject<ApiResult>(response.Content);

            return result;
        }

        public static ApiResultOrderDetail GetTransactionDetail(ParameterRessulAPI data)
        {
            ApiResultOrderDetail result = new ApiResultOrderDetail();
            // Link test: https://payment-setting.dev.tekoapis.net/api/v2/payment/init/multi-v2
            // Link Production: https://payment-gateway.tekoapis.com/api/v2/payment/init/multi-v2

            var client = new RestClient("https://payments.stag.tekoapis.net/");
            string action = string.Format("api/v2/payment/payment-requests?merchantCode={0}&terminalCode={1}&orderCode={2}&checksum={3}",data.merchantCode,data.terminalCode,data.orderCode,data.checksum);
            var request = new RestRequest(action, Method.GET);
            //request.AddHeader("X-User-id", "hashcode");


            IRestResponse response = client.Execute(request);
            result = JsonConvert.DeserializeObject<ApiResultOrderDetail>(response.Content);

            return result;
        }

        public static string SHA256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}
