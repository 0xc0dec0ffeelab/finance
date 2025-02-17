using System.Diagnostics;

namespace finance_dotnet.Backend.Extensions
{
    public static class LoggerExtension
    {
        public static void LogLokiError(this ILogger logger, Exception ex, string? traceId = null, int stackframeSize = 5)
        {
            var stackFrames = new StackTrace(ex, true).GetFrames();
            string stackTraces = string.Empty;

            if (stackFrames != null)
            {
                var topFrames = stackFrames.Take(stackframeSize).Select(frame =>
                {
                    var method = frame.GetMethod();
                    var file = frame.GetFileName();
                    var line = frame.GetFileLineNumber();
                    return $"{method?.DeclaringType?.FullName}.{method?.Name} (at {file}:{line})";
                });

                stackTraces = string.Join("\n", topFrames);
            }

            var callerFrame = stackFrames?.Length > 1 ? stackFrames[1] : null;
            var callerMethod = callerFrame?.GetMethod();
            var callerClass = callerMethod?.DeclaringType?.FullName;
            var callerMethodName = callerMethod?.Name;

            var logProperties = new Dictionary<string, object>
            {
                { "ExceptionMessage", ex.Message },
                { "ExceptionType", ex.GetType().FullName! },
                { "StackTrace", stackTraces },
                { "Caller", $"{callerClass}.{callerMethodName}" },
                { "Timestamp", DateTime.UtcNow.ToString("o") },
                { "TraceId", traceId ?? string.Empty }
            };

            logger.LogError("level=error traceId={TraceId} at={Timestamp} in={Caller} type={ExceptionType} message={ExceptionMessage} stackTrace={StackTrace}",
                logProperties["TraceId"],
                logProperties["Timestamp"],
                logProperties["Caller"],
                logProperties["ExceptionType"],
                logProperties["ExceptionMessage"],
                logProperties["StackTrace"]);
        }
    }
}
