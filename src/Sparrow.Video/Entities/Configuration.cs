using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Sparrow.Video.Enums;

namespace Sparrow.Video.Entities
{
    public class Configuration
    {
        public Configuration()
        {
            _sources = new List<FileMeta>();
            _additionalSettings = new List<OutputAdditionalSettings>();
        }

        public IEnumerable<FileMeta> Sources { get => _sources; }
        public FileMeta OutputFile { get => _outputFile; }
        public IEnumerable<OutputAdditionalSettings> Additional { get => _additionalSettings; }

        private List<FileMeta> _sources;
        private FileMeta _outputFile;
        private List<OutputAdditionalSettings> _additionalSettings; 

        public Configuration AddSrc(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File from \"{path}\" not exists");
            _sources.Add(FileMeta.From(path));
            return this;
        }

        public Configuration RestoreSrc()
        {
            _sources = new List<FileMeta>(RestoreFilesMeta());
            return this;
        }

        public Configuration Loop(Func<FileMeta, bool> condition, int loopCount)
        {
            foreach (var src in Sources)
            {
                var success = condition?.Invoke(src) ?? false;
                if (success)
                {
                    var fileManipulation = new FileManipulation() { Loop = loopCount };
                    var outputSetting = new OutputAdditionalSettings(src.Links.Original, fileManipulation);
                    _additionalSettings.Add(outputSetting);
                }
            }
            return this;
        }

        public Configuration AddSrcRange(IEnumerable<string> paths)
        {
            if (paths is null)
                throw new ArgumentNullException($"{nameof(paths)} cannot be null!");
            foreach (var path in paths)
                _sources.Add(FileMeta.From(path));
            return this;
        }

        public Configuration AddDistinctSrcRange(IEnumerable<string> paths)
        {
            if (paths is null)
                throw new ArgumentNullException($"{nameof(paths)} cannot be null!");
            foreach (var path in paths)
                if(!_sources.Any(src => src.Links.Original == path))
                    _sources.Add(FileMeta.From(path));
            return this;
        }

        public Configuration SaveTo(string name, string dirPath = "", bool createDirIfNotExists = false)
        {
            if (Project.IsAvailable && string.IsNullOrWhiteSpace(dirPath))
                dirPath = Paths.OutputProjectFiles.Path;
            if (string.IsNullOrWhiteSpace(dirPath))
                dirPath = Paths.OutputFiles.Path;
            if (string.IsNullOrWhiteSpace(dirPath) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(dirPath)} and {nameof(name)} cannot be empty or null!");
            if (createDirIfNotExists)
                Directory.CreateDirectory(dirPath);
            if (!Directory.Exists(dirPath))
                throw new DirectoryNotFoundException($"\"{dirPath}\" not found!");

            _outputFile = new FileMeta(dirPath, FileType.Video)
            {
                Name = name,
                Extension = "mp4"
            };
            return this;
        }

        public Configuration SaveAs(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException($"{nameof(extension)} cannot be null!");
            _outputFile.Extension = extension;
            return this;
        }

        public Configuration Quality(VideoQuality quality)
        {
            if (quality is null)
                throw new ArgumentException($"{nameof(quality)} cannot be null!");
            _outputFile.VideoQuality = quality;
            return this;
        }

        private IEnumerable<FileMeta> RestoreFilesMeta()
        {
            IEnumerable<FileMeta> result = new List<FileMeta>();
            var metaPath = Paths.CurrentProject + "/meta.txt";
            if (File.Exists(metaPath))
            {
                var metaJson = File.ReadAllText(metaPath);
                result = JsonConvert.DeserializeObject<IEnumerable<FileMeta>>(metaJson) ?? throw new Exception("Failed restore files meta");
            }
            else Log.Error("Не удалось восстановить мету файлов, т.к. файл с сохранением не обнаружен");
            return result;
        }
    }
}
