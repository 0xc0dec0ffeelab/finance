using finance_dotnet.Backend.Extensions;
using finance_dotnet.Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using finance_dotnet.Backend.Kakfa;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace finance_dotnet.Backend.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductApiController : ControllerBase
    {
        readonly IMinioRepository _minioRepository;
        readonly ILogger<ProductApiController> _logger;
        readonly IKafkaProducer _kafkaProducer;
        readonly IConfiguration _configuration;

        [Authorize(Policy = "SearchCatalog")]
        [HttpGet("catalog/search")]
        public IActionResult SearchCatalog()
        {
            return Ok(new { message = "Catalog search results" });
        }


        [HttpPost("sendemail")]
        public async Task<IActionResult> Sendemail(string to, string toAddress, string subject, string body)
        {
            EmailMessage emailMessage = new EmailMessage()
            {
                To = to,
                ToAddress = toAddress,
                Subject = subject,
                Body = body
            };
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var message = JsonSerializer.Serialize(emailMessage, options);
            string topic = _configuration.GetValue<string>("Kafka:Email:Topic")!;
            string? partitionKey = null;
            await _kafkaProducer.ProduceByPartitionKeyAsync(topic, partitionKey, message);
            //await SendEmailAsync(to, toAddress, subject, body);
            return Ok();
        }
        private static async Task SendEmailAsync(string to, string toAddress, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("System", "no-reply@finance-dotnet.com"));
            message.To.Add(new MailboxAddress(to, toAddress));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };
            using var client = new SmtpClient();
            await client.ConnectAsync("postfix-svc", 587, SecureSocketOptions.None);
            //await client.ConnectAsync("postfix-svc", 587, SecureSocketOptions.StartTls);
            //await client.AuthenticateAsync("admin", "1234");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        


        public ProductApiController(IMinioRepository minioRepository, ILogger<ProductApiController> logger, IKafkaProducer kafkaProducer, IConfiguration configuration)
        {
            _minioRepository = minioRepository;
            _logger = logger;
            _kafkaProducer = kafkaProducer;
            _configuration = configuration;
        }

        [HttpPost("file/test")]
        public async Task<IActionResult> Test(string log)
        {
            Exception ex = new Exception($"{DateTimeOffset.Now} {log}");
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogLokiError(ex, traceId);
            return Ok();
        }

        [HttpPost("file/upload")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file provided.");

            var fileUrl = await _minioRepository.PutObjectAsync("products", file);
            
            if (fileUrl == default) return StatusCode(StatusCodes.Status500InternalServerError, "file upload error");

            return Ok(fileUrl);
        }

        [HttpGet("file/download")]
        public async Task<IActionResult> DownloadFileAsync(string fileName)
        {
            var fileStream = await _minioRepository.GetObjectAsync("products", fileName);
            
            if (fileStream != default) return File(fileStream, "application/octet-stream", fileName);

            return StatusCode(StatusCodes.Status500InternalServerError, "file download error");
        }

        [HttpDelete("file/delete")]
        public async Task<IActionResult> DeleteFileAsync(string fileName)
        {
            var isDeleted = await _minioRepository.DeleteObjectAsync("products", fileName);

            if (isDeleted) return Ok(isDeleted);

            return StatusCode(StatusCodes.Status500InternalServerError, isDeleted);
        }
    }
}
