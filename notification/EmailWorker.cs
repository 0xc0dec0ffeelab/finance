using Confluent.Kafka;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text.Json;

namespace notification
{
    public class EmailWorker : BackgroundService
    {
        private readonly ILogger<EmailWorker> _logger;
        private readonly IConsumer<string?, string> _consumer;
        private readonly IConfiguration _configuration;
        private readonly string _bootstrapServers = string.Empty;
        private readonly string _emailTopic = string.Empty;
        private readonly string _emailGroupId = string.Empty;
        private readonly string _emailSender = string.Empty;
        private readonly string _emailHostName = string.Empty;


        public EmailWorker(ILogger<EmailWorker> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;

            _emailSender = _configuration.GetValue<string>("Email:Sender")!;
            _emailHostName = _configuration.GetValue<string>("Email:HostName")!;

            _emailTopic = _configuration.GetValue<string>("Kafka:Email:Topic")!;
            _emailGroupId = _configuration.GetValue<string>("Kafka:Email:GroupId")!;

            _bootstrapServers = _configuration.GetValue<string>("Kafka:BootstrapServers")!

            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _emailGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false, // exactly-once
                IsolationLevel = IsolationLevel.ReadCommitted
            };

            _consumer = new ConsumerBuilder<string?, string>(config).Build();
            _consumer.Subscribe(_emailTopic);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested == false)
            {

                //await Task.Delay(1000, stoppingToken);

                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    await ProcessMessage(
                        consumeResult.Topic,
                        consumeResult.Message.Value,
                        consumeResult.Message.Key);
                    _consumer.Commit(consumeResult);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Kafka Error: {ex.Message}");
                }
            }
        }

        private async Task ProcessMessage(string topic, string messageJson, string? partitionKey)
        {
            //if (topic == "order_status_changed")
            //{
            //    _logger.LogInformation($"Processing order status: {message}");
            //    await SendEmail("user@example.com", "Order Status Update", $"Order Update: {message}");
            //}
            //else if (topic == "account_status_updated")
            //{
            //    _logger.LogInformation($"Processing account status: {message}");
            //    await SendEmail("user@example.com", "Account Status Update", $"Account Update: {message}");
            //}

            if (topic == _emailTopic)
            {
                await SendEmailAsync(messageJson);
            }
        }
        private async Task SendEmailAsync(string messageJson)
        {

            EmailMessage emailMessage = JsonSerializer.Deserialize<EmailMessage>(messageJson)!;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("System", _emailSender));
            message.To.Add(new MailboxAddress(emailMessage.To, emailMessage.ToAddress));
            message.Subject = emailMessage.Subject;
            message.Body = new TextPart("plain")
            {
                Text = emailMessage.Body
            };
            using var client = new SmtpClient();
            await client.ConnectAsync(_emailHostName, 587, SecureSocketOptions.None);
            //await client.ConnectAsync(_emailHostName, 587, SecureSocketOptions.StartTls);
            //await client.AuthenticateAsync("admin", "1234");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
