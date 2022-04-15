using System.Text;
using Sparrow.Video.Enums;

namespace Sparrow.Video.Entities
{
    public class ScriptFormatter
    {
        public ScriptFormatter()
        {
            Result = new ScriptFormatterResult();
        }

        public bool IsCombinedSourcesInTxt { get; private set; }
        public bool IsCombinedSources { get; private set; }
        public ScriptFormatterResult Result { get; private set; }
        public IEnumerable<OutputAdditionalSettings> AdditionalSettings { get; private set; }

        public ScriptFormatter ChangeAdditionalSettings(IEnumerable<OutputAdditionalSettings> settings)
        {
            if (settings != null)
                AdditionalSettings = settings;
            return this;
        }

        public ScriptFormatter CombineSourcesInTxt(IEnumerable<FileMeta> sources)
        {
            IsCombinedSourcesInTxt = true;
            sources = ApplyAdditionalSettings(sources);
            if (!Directory.Exists(Paths.Meta.Path))
                Directory.CreateDirectory(Paths.Meta.Path);
            string filePath = $"{Paths.Meta.Path}/combined-files_{DateTime.Now.Ticks}.txt";
            using (var writer = File.CreateText(filePath))
            {
                foreach (var source in sources)
                    if(!string.IsNullOrWhiteSpace(source.Links.Ts))
                        writer.WriteLine($"file '{Path.GetFullPath(source.Links.Ts)}'");
            }
            Result.CombinedSourcesInTxt = filePath;
            return this;
        }

        public ScriptFormatter CombineSources(IEnumerable<FileMeta> sources)
        {
            IsCombinedSources = true;
            var builder = new StringBuilder();
            sources = ApplyAdditionalSettings(sources);
            sources.ToList().ForEach(source => builder.Append($"-i \"{Path.GetFullPath(source.Links.Ts)}\" "));
            Result.CombinedSources = builder.ToString();
            return this;
        }

        private IEnumerable<FileMeta> ApplyAdditionalSettings(IEnumerable<FileMeta> sources)
        {
            var sourcesWithSettings = new List<FileMeta>();
            foreach (var src in sources)
            {
                var existsSetting = AdditionalSettings.Where(settings => settings.OriginalSource == src.Links.Original).FirstOrDefault();
                if(existsSetting != null)
                {
                    sourcesWithSettings.AddRange(CreateLoopCollection(src, existsSetting.Manipulation.Loop));
                }
                else
                {
                    sourcesWithSettings.Add(src);
                }
            }
            return sourcesWithSettings;
        }

        private IEnumerable<FileMeta> CreateLoopCollection(FileMeta file, int loop)
        {
            var loops = new List<FileMeta>();
            if (loop < 1) loops.Add(file);
            for (int i = 0; i < loop; i++)
                loops.Add(file);
            return loops;
        }
    }
}
