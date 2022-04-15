using Newtonsoft.Json;
using Sparrow.Video.Abstractions;
using Sparrow.Video.Entities;

namespace Sparrow.Video
{
    public class FFProbeAnalyser
    {
        public FFProbeAnalyser()
        {
            _process = new ExecutableProcess().FilePathFromConfig("ffprobePath");
        }

        private IExecutableProcess _process;

        public async Task<FileAnalyse> AnalyseAsync(string filePath)
        {
            var jsonInfo = await _process.StartWithOutputCatchAsync($"-i \"{filePath}\" -v quiet -print_format json -show_format -show_streams");
            var convertSettings = new JsonSerializerSettings() {
                NullValueHandling = NullValueHandling.Ignore
            };
            var info = JsonConvert.DeserializeObject<FileAnalyse>(jsonInfo, convertSettings);
            return info ?? new FileAnalyse() { Streams = new FileAnalyseStream[0] };
        }
    }
}
