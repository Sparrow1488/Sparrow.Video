using System.Configuration;
using System.Diagnostics;
using Sparrow.Video.Abstractions;

namespace Sparrow.Video.Entities
{
    internal class ExecutableProcess : IExecutableProcess
    {
        public ExecutableProcess(string ffmpegPath = "")
        {
            _ffmpegPath = ffmpegPath;
        }

        private string _ffmpegPath = string.Empty;

        public IExecutableProcess FilePathFromConfig(string configKey)
        {
            _ffmpegPath = ConfigurationManager.AppSettings[configKey] ?? throw new KeyNotFoundException("Not found executable file file path in app.config!");
            return this;
        }

        public async Task StartAsync(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException(nameof(command));

            var startInfo = CreateStartInfoDefault();

            startInfo.Arguments = command;
            using (var process = new Process() { StartInfo = startInfo })
            {
                if (process is null)
                    throw new Exception("Process not started");
                process.Start();
                await process.WaitForExitAsync();
            }
        }

        public async Task<string> StartWithOutputCatchAsync(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException(nameof(command));

            var startInfo = CreateStartInfoDebug();
            string result = "";

            startInfo.Arguments = command;
            using (var process = new Process() { StartInfo = startInfo })
            {
                if (process is null)
                    throw new Exception("Process not started");
                process.EnableRaisingEvents = true;
                process.Start();
                result = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();
            }
            return result;
        }

        private ProcessStartInfo CreateStartInfoDefault()
        {
            var ffmpegStartInfo = new ProcessStartInfo()
            {
                FileName = _ffmpegPath,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                CreateNoWindow = true,
                UseShellExecute = true
            };
            return ffmpegStartInfo;
        }

        private ProcessStartInfo CreateStartInfoDebug()
        {
            var ffmpegStartInfo = CreateStartInfoDefault();
            ffmpegStartInfo.RedirectStandardError = true;
            ffmpegStartInfo.RedirectStandardOutput = true;
            ffmpegStartInfo.RedirectStandardInput = true;
            ffmpegStartInfo.UseShellExecute = false;
            ffmpegStartInfo.CreateNoWindow = true;
            return ffmpegStartInfo;
        }

    }
}
