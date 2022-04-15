using Newtonsoft.Json;
using Serilog;
using Sparrow.Video.Abstractions;
using Sparrow.Video.Entities;
using Sparrow.Video.Enums;
using static Sparrow.Video.Abstractions.IVideoEditor;

namespace Sparrow.Video
{
    public class FFMpegEditor : IVideoEditor
    {
        public FFMpegEditor(string ffmpegPath = "")
        {
            _config = new Configuration();
            _scriptBuilder = new ScriptBuilder();
            _executableProcess = string.IsNullOrWhiteSpace(ffmpegPath) ? 
                                    new ExecutableProcess().FilePathFromConfig("ffmpegPath") : new ExecutableProcess(ffmpegPath);
            
            _jsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
        }

        private Configuration _config;
        private IScriptBuilder _scriptBuilder;
        private IExecutableProcess _executableProcess;
        private JsonSerializerSettings _jsonSettings;

        public event LogAction OnCachedSource;
        public event LogAction OnConvertedSource;

        public IVideoEditor Configure(Action<Configuration> configBuilder)
        {
            if(configBuilder is null)
                throw new ArgumentNullException($"{nameof(configBuilder)} cannot be null!");
            _config = _config ?? new Configuration();
            configBuilder(_config);
            SaveFilesMeta();
            return this;
        }

        public async Task ConcatSourcesAsync(ConcatType concatType)
        {
            _scriptBuilder.Clean();
            var notCached = _config.Sources.Where(src => string.IsNullOrWhiteSpace(src.Links.Converted));
            foreach (var src in notCached)
                await CacheSource(src, _config.OutputFile.VideoQuality);
            if (concatType == ConcatType.ReencodingConcat || concatType == ConcatType.ReencodingConcatConvertedViaTransportStream)
            {
                if (concatType == ConcatType.ReencodingConcatConvertedViaTransportStream)
                {
                    notCached = _config.Sources.Where(src => string.IsNullOrWhiteSpace(src.Links.Ts) || !File.Exists(src.Links.Ts));
                    foreach (var source in notCached)
                        await ConvertToTsFormatAsync(source);
                }
                var script = _scriptBuilder.ConfigureInputs(cmd => cmd.Add("-y -f concat -safe 0"))
                                           .ConfigureOutputs(cmd => cmd.Add("-c:a copy -c:v copy -preset fast -vsync cfr -r 45"))
                                           .Build(_config, format => format.ChangeAdditionalSettings(_config.Additional)
                                                                           .CombineSourcesInTxt(_config.Sources));
                await _executableProcess.StartAsync(script);
            }

            #region NotImplemented
            if (concatType == ConcatType.ReencodingComplexFilter)
            {
                throw new NotImplementedException("Please, dont use this concationation type, it while not work correct");
                //var filterComplexArgs = new StringBuilder();
                //cachedFiles.ToList().ForEach(file => filterComplexArgs.Append($"[{Array.IndexOf(cachedFiles, file)}:v]"));
                //filterComplexArgs.Append($"concat=n={cachedFiles.Length}");
                //var script = _scriptBuilder.ConfigureInputs(cmd => cmd.Add("-y"))
                //                           .ConfigureOutputs(cmd => cmd.Add($"-filter_complex \"{filterComplexArgs}\" -c:a copy -c:v libx264 -preset fast -vsync cfr -r 45"))
                //                           .Build(_config, format => format.ChangeAdditionalSettings(_config.Additional)
                //                                                           .CombineSources(cachedFiles));
                //await _executableProcess.StartAsync(script);
            }
            if (concatType == ConcatType.Demuxer)
            {
                throw new NotImplementedException("Please, dont use this concationation type, it while not work correct");
            }
            #endregion

        }

        private async Task CacheSource(FileMeta fileMeta, VideoQuality quality)
        {
            // TODO: работает только для видео и картинок, нужно бы завезти кэширование отдельно аудио
            if (!fileMeta.Analyse.WithAudio())
                await PutSilentOnVideoAsync(fileMeta);

            var config = new Configuration();
            string cacheDirPath = Paths.ConvertedFiles.Path;
            var cachedFilesCount = Directory.GetFiles(cacheDirPath).Length;
            config.AddSrc(fileMeta.ToString())
                  .SaveTo($"video{cachedFilesCount+1}", cacheDirPath)
                  .SaveAs("mp4")
                  .Quality(quality);

            var scriptBuilder = new ScriptBuilder();
            scriptBuilder.ConfigureInputs(commands => commands.Add($"-y -i {Paths.Resources}/black{config.OutputFile.VideoQuality}.png"));
            
            string filterScaleArgument = string.Empty;
            var videoStream = fileMeta.Analyse.Streams.First(file => file.CodecType.ToUpper().Contains("VIDEO"));
            if (videoStream.Width > videoStream.Height)
                filterScaleArgument = $"{quality.Width}:-1";
            else filterScaleArgument = $"-1:{quality.Height}";
            scriptBuilder.ConfigureOutputs(commands => commands.Add($"-filter_complex \"[1:v]scale={filterScaleArgument}[v2];[0:v][v2]overlay=(main_w - overlay_w)/2:(main_h - overlay_h)/2\""));
            
            var script = scriptBuilder.Build(config);
            Log.Debug($"Кэшируем файл {fileMeta.Name}...");
            await _executableProcess.StartAsync(script);
            if (!File.Exists(config.OutputFile.ToString()))
                throw new Exception("Failed caching file!");
            fileMeta.Links.Converted = config.OutputFile.ToString();
            Log.Debug($"Кэширован файл {fileMeta.Name}");
            OnCachedSource?.Invoke($"file: {fileMeta.Name} was cached");
            
            SaveFilesMeta();
        }

        private async Task ConvertToTsFormatAsync(FileMeta file)
        {
            var scriptBuilder = new ScriptBuilder();
            var tsCachedFilesDir = Paths.TsFiles.Path;
            if (!Directory.Exists(tsCachedFilesDir))
                Directory.CreateDirectory(tsCachedFilesDir);
            var tsFileCount = Directory.GetFiles(tsCachedFilesDir).Length;
            var config = new Configuration()
                            .AddSrc(file.Links.Converted)
                            .SaveTo($"videoTransportStream({tsFileCount})", tsCachedFilesDir)
                            .SaveAs("ts");
            var command = scriptBuilder.ConfigureInputs(commands => commands.Add("-y"))
                                        .ConfigureOutputs(commands => commands.Add("-acodec copy -vcodec copy -vbsf h264_mp4toannexb -f mpegts"))
                                        .Build(config);
            file.Links.Ts = config.OutputFile.ToString();
            Log.Debug("Конвертируем файлы в формат .ts");
            await _executableProcess.StartAsync(command);
            scriptBuilder.Clean();
            Log.Debug($"Файл: {new string(Path.GetFileName(file.ToString()))} успешно конвертирован в формат .ts");
            OnConvertedSource?.Invoke($"file: {new string(Path.GetFileName(file.ToString()))} was converted to .ts");
            
            SaveFilesMeta();
        }

        private void CleanTsCache()
        {
            Log.Debug("Очищаем файлы кэшей с расширением .ts");
            var tsCacheFiles = Paths.TsFiles.Path;
            var files = Directory.GetFiles(tsCacheFiles);
            foreach (var file in files)
                File.Delete(file);
        }

        private IEnumerable<FileMeta> GetCachedCopies()
        {
            var projectCachedFilesPath = Paths.ConvertedFiles.Path;
            var files = Directory.GetFiles(projectCachedFilesPath);
            var cached = new List<FileMeta>();
            foreach (var file in files)
            {
                cached.Add(FileMeta.From(file));
            }
            return cached;
        }

        private IEnumerable<FileMeta> GetCachedTsCopies()
        {
            var projectCachedFilesPath = Paths.TsFiles.Path;
            var files = Directory.GetFiles(projectCachedFilesPath);
            var cached = new List<FileMeta>();
            foreach (var file in files)
                cached.Add(FileMeta.From(file));
            return cached;
        }

        private void SaveFilesMeta()
        {
            var metaPath = Paths.CurrentProject + "/meta.txt";
            if(File.Exists(metaPath))
                File.Delete(metaPath);
            using (var sw = File.CreateText(metaPath))
            {
                var json = JsonConvert.SerializeObject(_config.Sources, _jsonSettings);
                sw.WriteLine(json);
            }
            Log.Debug("Сохранены мета файлы");
        }

        private async Task PutSilentOnVideoAsync(FileMeta fileMeta)
        {
            Log.Debug("Добавляем аудиодорожку немому видео");
            string endPattern = "(silent)";
            string oldName = fileMeta.Name;
            string newFileName = $"{oldName}{endPattern}";
            await _executableProcess.StartAsync($"-y -f lavfi -i anullsrc=channel_layout=stereo:sample_rate=44100 -i \"{fileMeta}\" -c:v copy -c:a aac -shortest \"{fileMeta.RootPath}/{newFileName}.{fileMeta.Extension}\"");
            fileMeta.Name = newFileName;
            File.Delete($"{fileMeta.RootPath}/{oldName}.{fileMeta.Extension}");
        }
    }
}