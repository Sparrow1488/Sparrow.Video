using Newtonsoft.Json;
using Sparrow.Video.Enums;

namespace Sparrow.Video.Entities
{
    public class FileMeta
    {
        public FileMeta(string rootPath, FileType type)
        {
            RootPath = rootPath;
            Type = type;
            Links = new FileLinks();
            Links.Original = rootPath;
        }

        public static readonly FileMeta Empty = new FileMeta("./", FileType.Undefined);

        [JsonProperty]
        public string RootPath { get; internal set; }
        [JsonProperty]
        public string Name { get; internal set; }
        [JsonProperty]
        public string Extension { get; internal set; }
        public FileLinks Links { get; set; }
        public FileType Type { get; } = FileType.Undefined;
        [JsonProperty]
        public FileAnalyse Analyse { get; internal set; }
        [JsonProperty]
        public VideoQuality VideoQuality { get; internal set; } = VideoQuality.Undefined;

        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static FileMeta From(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"{nameof(filePath)} cannot be empty or null!");
            if (!IsFilePathCharsValid(filePath))
                new InvalidOperationException($"{filePath} contains invalid chars unusable in ffmpeg (' ' ', '$') or file path too long");
            var info = new FileInfo(filePath);
            string correctExtension = info.Extension.Remove(0, 1).ToLower();
            string correctName = info.Name.Remove(info.Name.Length - info.Extension.Length, info.Extension.Length);
            var metaType = SelectType(correctExtension);
            var meta = new FileMeta(filePath, metaType)
            {
                Extension = correctExtension,
                Name = correctName,
                RootPath = info.DirectoryName ?? string.Empty,
            };
            meta.Analyse = new FFProbeAnalyser().AnalyseAsync(filePath).GetAwaiter().GetResult();
            return meta;
        }

        /// <exception cref="ArgumentException"></exception>
        public static bool IsFilePathCharsValid(string filePath)
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"{nameof(filePath)} cannot be empty or null!");
            if (filePath.Contains("$") || filePath.Contains("'"))
                isValid = false;
            return isValid;
        }

        public override string ToString()
        {
            return $"{Path.Combine(RootPath, Name)}.{Extension}".Replace('/', '\\');
        }

        // TODO: переписать
        private static FileType SelectType(string extension)
        {
            var result = FileType.Undefined;

            if (new string[] { "mp4", "avi", "ts" }.Contains(extension))
                result = FileType.Video;
            if (new string[] { "mp3", "flac" }.Contains(extension))
                result = FileType.Audio;
            return result;
        }
    }
}
