namespace finance_dotnet.Backend.Kakfa
{
    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string ToAddress { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
