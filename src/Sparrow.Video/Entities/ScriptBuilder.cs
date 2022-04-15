using System.Text;
using Sparrow.Video.Abstractions;

namespace Sparrow.Video.Entities
{
    public class ScriptBuilder : IScriptBuilder
    {
        private List<string> _input = new List<string>();
        private List<string> _middle = new List<string>();
        private List<string> _output = new List<string>();
        private ScriptFormatter _formatter = new ScriptFormatter();

        public string Build(Configuration config)
        {
            return Build(config, formatter => new ScriptFormatter());
        }

        public string Build(Configuration config, Action<ScriptFormatter> format)
        {
            if (config is null)
                throw new ArgumentNullException($"{nameof(config)} cannot be null!");
            if (format is null)
                throw new ArgumentNullException($"Configure method '{nameof(format)}' cannot be null!");
            format(_formatter);
            var builder = new StringBuilder();

            SetValuesFromConfigUsingFormatter(config);
            builder.AppendJoin(" ", _input);
            builder.Append(" ");
            builder.AppendJoin(" ", _middle);
            if(_middle.Count > 0)
                builder.Append(" ");
            builder.AppendJoin(" ", _output);
            return builder.ToString();
        }

        public IScriptBuilder ChangeFormat(Action<ScriptFormatter> format)
        {
            if (format is null)
                throw new ArgumentNullException($"Configure method '{nameof(format)}' cannot be null!");
            format(_formatter);
            return this;
        }

        public IScriptBuilder ConfigureInputs(Action<IList<string>> commands)
        {
            if (commands is null)
                throw new ArgumentNullException($"Configure method '{nameof(commands)}' cannot be null!");
            commands(_input);
            return this;
        }

        public IScriptBuilder ConfigureOutputs(Action<IList<string>> commands)
        {
            if (commands is null)
                throw new ArgumentNullException($"Configure method '{nameof(commands)}' cannot be null!");
            commands(_output);
            return this;
        }

        public IScriptBuilder Clean()
        {
            _input = new List<string>();
            _output = new List<string>();
            _middle = new List<string>();
            _formatter = new ScriptFormatter();
            return this;
        }

        private void SetValuesFromConfigUsingFormatter(Configuration config)
        {
            if (_formatter.IsCombinedSourcesInTxt)
                _input.Add($"-i \"{_formatter.Result.CombinedSourcesInTxt}\"");
            else if (_formatter.IsCombinedSources)
                _input.Add(_formatter.Result.CombinedSources);
            else SetInputScriptParams(config);
            SetOutputScriptParams(config);
        }

        private void SetInputScriptParams(Configuration config)
        {
            foreach (var source in config.Sources)
                _input.Add($"-i \"{source}\"");
        }

        private void SetOutputScriptParams(Configuration config)
        {
            //if (config.OutputFile.VideoQuality != null && config.OutputFile.VideoQuality != Enums.VideoQuality.Undefined)
            //    _middle.Add($"-s {config.OutputFile.VideoQuality.Width}x{config.OutputFile.VideoQuality.Height}");
            _output.Add($"\"{config.OutputFile}\"");
        }
        
    }
}
