using ECPay.Payment.Integration;
using finance_dotnet.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http;

namespace finance_dotnet.Backend.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderApiController : Controller
    {
        private readonly HttpClient _client;
        private readonly AllInOne _allInOne;
        private readonly IConfiguration _configuration;

        public OrderApiController(IHttpClientFactory httpClientFactory, IConfiguration configuration) 
        {
            _client = httpClientFactory.CreateClient();
            _allInOne = new AllInOne(_client);
            _configuration = configuration;
        }
        [HttpPost("test")]
        public async Task<IActionResult> Test(int id, int amount)
        {
            return Ok(default);
        }

        /// <summary>
        /// 全支付 訂單產生html
        /// </summary>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create(OrderViewModel order)
        {
            string? ReturnURL = _configuration["ECPay:ReturnURL"];
            string? OrderResultURL = _configuration["ECPay:OrderResultURL"];
            string? ClientBackURL = _configuration["ECPay:ClientBackURL"];

            /* 服務參數 */
            _allInOne.ServiceMethod = ECPay.Payment.Integration.HttpMethod.HttpPOST;//介接服務時，呼叫 API 的方法
            _allInOne.ServiceURL = @"https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5";//要呼叫介接服務的網址
            _allInOne.HashKey = "pwFHCqoQZGmho4w6";//ECPay提供的Hash Key
            _allInOne.HashIV = "EkRm7iFT261dpevs";//ECPay提供的Hash IV
            _allInOne.MerchantID = "3002607";//ECPay提供的特店編號

            string MerchantTradeNo = $"fd{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            ;
            /* 基本參數 */
            _allInOne.Send.ReturnURL = ReturnURL; //付款完成通知回傳的網址
            _allInOne.Send.ClientBackURL = ClientBackURL; //瀏覽器端返回的廠商網址
            _allInOne.Send.OrderResultURL = OrderResultURL;//瀏覽器端回傳付款結果網址
            //_allInOne.Send.MerchantTradeNo = "ECPay" + new Random().Next(0, 99999).ToString();//廠商的交易編號
            _allInOne.Send.MerchantTradeNo = MerchantTradeNo;
            _allInOne.Send.MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//廠商的交易時間
            _allInOne.Send.TotalAmount = Decimal.Parse("90");//交易總金額
            _allInOne.Send.TradeDesc = "fd transdesc";//交易描述
            _allInOne.Send.ChoosePayment = PaymentMethod.ALL;//使用的付款方式
            _allInOne.Send.Remark = "";//備註欄位
            _allInOne.Send.ChooseSubPayment = PaymentMethodItem.None;//使用的付款子項目
            _allInOne.Send.NeedExtraPaidInfo = ExtraPaymentInfo.Yes;//是否需要額外的付款資訊
            _allInOne.Send.DeviceSource = DeviceType.PC;//來源裝置
            _allInOne.Send.IgnorePayment = ""; //不顯示的付款方式
            _allInOne.Send.PlatformID = "";//特約合作平台商代號
            _allInOne.Send.CustomField1 = "";
            _allInOne.Send.CustomField2 = "";
            _allInOne.Send.CustomField3 = "";
            _allInOne.Send.CustomField4 = "";
            _allInOne.Send.EncryptType = 1;

            //訂單的商品資料
            _allInOne.Send.Items.Add(new Item()
            {
                Name = "fd2",//商品名稱
                Price = Decimal.Parse("90"),//商品單價
                Currency = "新台幣",//幣別單位
                Quantity = Int32.Parse("1"),//購買數量
                URL = "",//商品的說明網址
            });


            /*************************非即時性付款:ATM、CVS 額外功能參數**************/

            #region ATM 額外功能參數

            //_allInOne.SendExtend.ExpireDate = 3;//允許繳費的有效天數
            //_allInOne.SendExtend.PaymentInfoURL = "";//伺服器端回傳付款相關資訊
            //_allInOne.SendExtend.ClientRedirectURL = "";//Client 端回傳付款相關資訊

            #endregion


            #region CVS 額外功能參數

            //_allInOne.SendExtend.StoreExpireDate = 3; //超商繳費截止時間 CVS:以分鐘為單位 BARCODE:以天為單位
            //_allInOne.SendExtend.Desc_1 = "test1";//交易描述 1
            //_allInOne.SendExtend.Desc_2 = "test2";//交易描述 2
            //_allInOne.SendExtend.Desc_3 = "test3";//交易描述 3
            //_allInOne.SendExtend.Desc_4 = "";//交易描述 4
            //_allInOne.SendExtend.PaymentInfoURL = "";//伺服器端回傳付款相關資訊
            //_allInOne.SendExtend.ClientRedirectURL = "";///Client 端回傳付款相關資訊

            #endregion

            /***************************信用卡額外功能參數***************************/

            #region Credit 功能參數

            //_allInOne.SendExtend.BindingCard = BindingCardType.No; //記憶卡號
            //_allInOne.SendExtend.MerchantMemberID = ""; //記憶卡號識別碼
            //_allInOne.SendExtend.Language = ""; //語系設定

            #endregion Credit 功能參數

            #region 一次付清

            //_allInOne.SendExtend.Redeem = false;   //是否使用紅利折抵
            //_allInOne.SendExtend.UnionPay = true; //是否為銀聯卡交易

            #endregion

            #region 分期付款

            //_allInOne.SendExtend.CreditInstallment = "3,6";//刷卡分期期數

            #endregion 分期付款

            #region 定期定額

            //_allInOne.SendExtend.PeriodAmount = 1000;//每次授權金額
            //_allInOne.SendExtend.PeriodType = PeriodType.Day;//週期種類
            //_allInOne.SendExtend.Frequency = 1;//執行頻率
            //_allInOne.SendExtend.ExecTimes = 2;//執行次數
            //_allInOne.SendExtend.PeriodReturnURL = "";//伺服器端回傳定期定額的執行結果網址。

            #endregion

            var (htmlForm, errors) = _allInOne.CheckOut();
            ;
            //Debug.WriteLine(errors);

            return Content(htmlForm, "text/html");

        }
    }
}
