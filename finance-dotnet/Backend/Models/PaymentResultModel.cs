namespace finance_dotnet.Backend.Models
{
    public class PaymentResultModel
    {
        /// <summary>
        /// 特店編號
        /// </summary>
        public string? MerchantID { get; set; }
        /// <summary>
        /// 特店交易編號
        /// (訂單產生時傳送給綠界的特店交易編號)
        /// </summary>
        public string? MerchantTradeNo { get; set; }
        /// <summary>
        /// 特店旗下店舖代號
        /// </summary>
        public string? StoreID { get; set; }
        /// <summary>
        /// 付款時間
        /// 格式為yyyy/MM/dd HH:mm:ss
        /// </summary>
        public string? PaymentDate { get; set; }
        /// <summary>
        /// 特店選擇的付款方式 
        /// </summary>
        public string? PaymentType { get; set; }
        /// <summary>
        /// 交易手續費金額
        /// </summary>
        public string? PaymentTypeChargeFee { get; set; }
        /// <summary>
        /// 交易狀態
        /// 若回傳值為1時，為付款成功
        /// 其餘代碼皆為交易異常，請至廠商管理後台確認後再出貨。
        /// </summary>
        public string? RtnCode { get; set; }
        /// <summary>
        /// 交易訊息
        /// </summary>
        public string? RtnMsg { get; set; }
        /// <summary>
        /// 是否為模擬付款
        /// 
        /// 是否為模擬付款
        /// 0：代表此交易非模擬付款。
        /// 1：代表此交易為模擬付款，RtnCode也為1。並非是由消費者實際真的付款，所以綠界也不會撥款給廠商，請勿對該筆交易做出貨等動作，以避免損失
        /// </summary>
        public string? SimulatePaid { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public string? TradeAmt { get; set; }
        /// <summary>
        /// 訂單成立時間
        /// 格式為yyyy/MM/dd HH:mm:ss
        /// </summary>
        public string? TradeDate { get; set; }
        /// <summary>
        /// 綠界的交易編號
        /// (請保存綠界的交易編號與特店交易編號[MerchantTradeNo]的關連)
        /// </summary>
        public string? TradeNo { get; set; }
        /// <summary>
        /// 特約合作平台商代號
        /// (為專案合作的平台商使用)
        /// </summary>
        public string? PlatformID { get; set; }
        /// <summary>
        /// 自訂名稱欄位1
        /// 提供合作廠商使用記錄用客製化使用欄位
        /// </summary>
        public string? CustomField1 { get; set; }
        /// <summary>
        /// 自訂名稱欄位2
        /// 提供合作廠商使用記錄用客製化使用欄位
        /// </summary>
        public string? CustomField2 { get; set; }
        /// <summary>
        /// 自訂名稱欄位3
        /// 提供合作廠商使用記錄用客製化使用欄位
        /// </summary>
        public string? CustomField3 { get; set; }
        /// <summary>
        /// 自訂名稱欄位4
        /// 提供合作廠商使用記錄用客製化使用欄位
        /// </summary>
        public string? CustomField4 { get; set; }
        /// <summary>
        /// 檢查碼
        /// </summary>
        public string? CheckMacValue { get; set; }
        /// <summary>
        /// 錯誤列表
        /// </summary>
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
