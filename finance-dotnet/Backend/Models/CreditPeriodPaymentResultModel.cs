namespace finance_dotnet.Backend.Models
{
    //信用卡 定期定額授權成功通知
    public class CreditPeriodPaymentResultModel
    {
        /// <summary>
        /// 特店編號
        /// </summary>
        public string? MerchantID { get; set; }
        /// <summary>
        /// 特店交易編號
        /// </summary>
        public string? MerchantTradeNo { get; set; }
        /// <summary>
        /// 特店旗下店舖代號
        /// </summary>
        public string? StoreID { get; set; }
        /// <summary>
        /// 交易狀態
        /// 若回傳值為1時，為付款成功
        /// 其餘代碼皆為交易異常，請至廠商管理後台確認後再出貨。
        /// </summary>
        public int RtnCode { get; set; }
        /// <summary>
        /// 交易訊息
        /// </summary>
        public string? RtnMsg { get; set; }
        /// <summary>
        /// 是否為模擬付款
        /// 當交易為模擬付款時，才會回傳此欄位
        /// 1：代表此交易為模擬付款，RtnCode也為1。
        /// 並非是由消費者實際真的付款，所以綠界也不會撥款給廠商，
        /// 請勿對該筆交易做出貨等動作，以避免損失。
        /// </summary>
        public string SimulatePaid { get; set; }
        /// <summary>
        /// 訂單建立時所設定的週期種類
        /// </summary>
        public string? PeriodType { get; set; }
        /// <summary>
        /// 訂單建立時所設定的執行頻率
        /// </summary>
        public string? Frequency { get; set; }
        /// <summary>
        /// 訂單建立時所設定的執行次數
        /// </summary>
        public string? ExecTimes { get; set; }
        /// <summary>
        /// 此次所授權的金額
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 授權交易單號
        /// </summary>
        public string? Gwsr { get; set; }
        /// <summary>
        /// 處理時間
        /// 格式 yyyy/MM/dd HH:mm:ss
        /// </summary>
        public string? ProcessDate { get; set; }
        /// <summary>
        /// 授權碼
        /// </summary>
        public string? AuthCode { get; set; }
        /// <summary>
        /// 初次授權金額
        /// (定期定額交易的第一筆授權金額)
        /// </summary>
        public string? FirstAuthAmount { get; set; }
        /// <summary>
        /// 已執行成功次數
        /// (目前已成功授權的次數)
        /// </summary>
        public string? TotalSuccessTimes { get; set; }
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
