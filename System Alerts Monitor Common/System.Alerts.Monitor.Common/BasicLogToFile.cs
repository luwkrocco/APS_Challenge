using System.Text;

namespace System.Alerts.Monitor.Common
{
    // BASIC LOG TO FILE METHOD TAKEN OFF CHATGPT
    public class BasicLogToFile(string filepath)
    {
        private readonly string _filepath = filepath;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task LogException(Exception ex)
        {
            await LogMessage($" EXCEPTION OCCURED: {ex.Message} {Environment.NewLine} {ex.StackTrace}");
        }

        public async Task LogMessage(string message)
        {
            try
            {
                FileInfo fi = new FileInfo(_filepath);
                if (!fi.Directory!.Exists) fi.Directory.Create();

                await _semaphore.WaitAsync();
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                await File.AppendAllTextAsync(this._filepath, logMessage, Encoding.UTF8);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}