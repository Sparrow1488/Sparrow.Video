using Newtonsoft.Json;

namespace Sparrow.Video.Entities
{
    public class FileAnalyse
    {
        public IEnumerable<FileAnalyseStream> Streams { get; set; } = new FileAnalyseStream[0];

        public bool WithAudio() => 
            Streams.Where(stream => stream?.CodecType?.Contains("audio") ?? false).Any();

        public FileAnalyseStream GetVideo()
        {
            var videoStream = Streams.Where(stream => stream.CodecType.ToUpper() == "VIDEO").FirstOrDefault();
            return videoStream ?? new FileAnalyseStream() {
                Index = -1,
            };
        }
    }

    public class FileAnalyseStream
    {
        public int Index { get; set; }
        [JsonProperty("codec_name")]
        public string CodecName { get; set; }
        [JsonProperty("codec_type")]
        public string CodecType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double Duration { get; set; }
    }
}
