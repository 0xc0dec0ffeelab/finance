using ECPay.Payment.Integration;
using finance_dotnet.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace finance_dotnet.Backend.Controllers
{
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly HttpClient _client;
        private readonly AllInOne _allInOne;

        public PaymentController(IHttpClientFactory httpClientFactory) 
        {
            _client = httpClientFactory.CreateClient();
            _allInOne = new AllInOne(_client);
        }

        /// <summary>
        /// 付款完成 返回網址
        /// </summary>
        /// <returns></returns>
        [HttpPost("client/result")]
        public IActionResult ClientResult()
        {
            var (feedback, errors) = _allInOne.CheckOutFeedback(HttpContext.Request);
            PaymentResultModel paymentResultModel = new();

            //paymentResultModel.RtnCode = "1";
            //paymentResultModel.MerchantTradeNo = "2";
            //paymentResultModel.Errors = new List<string>() { "error1" };

            if (errors.Any())
            {
                // 取得資料
                foreach (string szKey in feedback.Keys)
                {
                    switch (szKey)
                    {
                        /* 支付後的回傳的基本參數 */
                        case nameof(PaymentResultModel.MerchantID): paymentResultModel.MerchantID = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.MerchantTradeNo): paymentResultModel.MerchantTradeNo = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.PaymentDate): paymentResultModel.PaymentDate = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.PaymentType): paymentResultModel.PaymentType = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.PaymentTypeChargeFee): paymentResultModel.PaymentTypeChargeFee = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.RtnCode): paymentResultModel.RtnCode = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.RtnMsg): paymentResultModel.RtnMsg = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.SimulatePaid): paymentResultModel.SimulatePaid = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.TradeAmt): paymentResultModel.TradeAmt = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.TradeDate): paymentResultModel.TradeDate = feedback[szKey].ToString(); break;
                        case nameof(PaymentResultModel.TradeNo): paymentResultModel.TradeNo = feedback[szKey].ToString(); break;
                        default: break;
                    }
                }
            }
            else
            {
                paymentResultModel.Errors = errors;
            }

            return View("~/BackEnd/Views/PaymentResult.cshtml", paymentResultModel);
        }
    }
}
