using Sparrow.Video.Entities;

namespace Sparrow.Video.Abstractions
{
    public interface IScriptBuilder
    {
        string Build(Configuration config, Action<ScriptFormatter> format);
        string Build(Configuration config);
        IScriptBuilder Clean();
        IScriptBuilder ChangeFormat(Action<ScriptFormatter> format);
        IScriptBuilder ConfigureInputs(Action<IList<string>> commands);
        IScriptBuilder ConfigureOutputs(Action<IList<string>> commands);
    }
    
}
