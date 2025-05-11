using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace FlakyTestNUnit.src.Logging
{
    public class CsvLogger
    {
        protected Stopwatch? stopwatch;
        protected int retryCount = 0;
        protected string status = "Passed";
        protected string browserName = "Unknown";
        protected string browserVersion = "Unknown";
        private static readonly string logPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestExecutionLog.csv"));

        public static IConfiguration Config { get; private set; }
        public static int RetryLimit { get; private set; }      
        public static int TotalRuns { get; private set; }
        public static int WaitingTime { get; private set; }

        [OneTimeSetUp]
        public void InitLogger()
        {
            Config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            RetryLimit = int.Parse(Config["RetryLimit"] ?? "1");
            TotalRuns = int.Parse(Config["TotalRuns"] ?? "1");
            WaitingTime = int.Parse(Config["WaitingTime"] ?? "10");

            if (!File.Exists(logPath))
            {
                File.WriteAllText(logPath, "TestName,Status,Duration,Timestamp,RetryCount,BrowserName,BrowserVersion\n");
            }
        }
        public void ExecuteWithRetry(Action testLogic)
        {
            retryCount = 0;
            stopwatch = Stopwatch.StartNew();
            var testName = TestContext.CurrentContext.Test.Name;

            while (retryCount < RetryLimit)
            {
                try
                {
                    status = "Passed";
                    testLogic.Invoke(); // Run the actual test logic
                    break; // Test passed
                }
                catch (Exception ex)
                {
                    status = "Failed";
                    retryCount++;
                    Console.WriteLine($"[Retry {retryCount}: {ex.Message}");

                    if (retryCount >= RetryLimit)
                    {
                        break;
                    }
                }
            }

            LogTestResult(status, testName);
        }
        public void LogTestResult(string status, string testName)
        {
            if (stopwatch != null)
            {
                stopwatch.Stop();
                var duration = (int)stopwatch.Elapsed.TotalMilliseconds;
                var timestamp = DateTime.Now.ToString("s");
                var line = $"{testName},{status},{duration},{timestamp},{retryCount},{browserName},{browserVersion}";
                File.AppendAllText(logPath, line + Environment.NewLine);
            }
        }        
    }
}
