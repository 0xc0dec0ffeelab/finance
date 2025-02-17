using ECPay.Payment.Integration;
using Microsoft.AspNetCore.Mvc;

namespace finance_dotnet.Backend.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentApiController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly AllInOne _allInOne;
        private readonly IConfiguration _configuration;

        public PaymentApiController(IHttpClientFactory httpClientFactory, IConfiguration configuration) 
        {
            _client = httpClientFactory.CreateClient();
            _allInOne = new AllInOne(_client);
            _configuration = configuration;
        }

        /// <summary>
        /// 全支付 一般付款結果通知
        /// </summary>
        /// <returns></returns>
        [HttpPost("callback")]
        public IActionResult PaymentCallback()
        {
            try
            {
                var (feedback, errors) = _allInOne.CheckOutFeedback(HttpContext.Request);

                foreach (var kv in feedback)
                {
                    Console.WriteLine($"PaymentCallback key: {kv.Key}, value: {kv.Value}");
                }

                return Ok("1|OK");
            }
            catch
            {
                return new ObjectResult("0|Error")
                {
                    StatusCode = 500
                };
            }
        }

        /// <summary>
        /// 全支付 一般付款結果通知 - 信用卡 定期定額的授權成功通知
        /// </summary>
        /// <returns></returns>
        [HttpPost("credit/period/callback")]
        public IActionResult PaymentCreditPeriodCallback()
        {
            try
            {
                var (feedback, errors) = _allInOne.CheckOutFeedback(HttpContext.Request);

                foreach (var kv in feedback)
                {
                    Console.WriteLine($"PaymentCallback key: {kv.Key}, value: {kv.Value}");
                }

                return Ok("1|OK");
            }
            catch
            {
                return new ObjectResult("0|Error")
                {
                    StatusCode = 500
                };
            }
        }

        /// <summary>
        /// 全支付 訂單查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost("query")]
        public async Task<IActionResult> Query(string MerchantTradeNo)
        {
            try
            {
                /* 服務參數 */
                _allInOne.ServiceMethod = ECPay.Payment.Integration.HttpMethod.ServerPOST; //介接服務時，呼叫 API 的方法
                _allInOne.ServiceURL = "https://payment-stage.ecpay.com.tw/Cashier/QueryTradeInfo";//要呼叫介接服務的網址
                _allInOne.HashKey = "pwFHCqoQZGmho4w6";//ECPay提供的Hash Key
                _allInOne.HashIV = "EkRm7iFT261dpevs";//ECPay提供的Hash IV
                _allInOne.MerchantID = "3002607";//ECPay提供的特店編號

                /* 基本參數 */
                _allInOne.Query.MerchantTradeNo = MerchantTradeNo;// 廠商的交易編號
                //_allInOne.Query.PlatformID = "";//特約合作平台商代號

                var (feedback, errors) = await _allInOne.QueryTradeInfoAsync();

                foreach (var kv in feedback)
                {
                    Console.WriteLine($"Query key: {kv.Key}, value: {kv.Value}");
                }

                return Ok(feedback);
            }
            catch
            {
                return new ObjectResult("Error")
                {
                    StatusCode = 500
                };
            }
        }

    }
}
