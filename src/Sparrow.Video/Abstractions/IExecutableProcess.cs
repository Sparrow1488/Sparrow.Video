namespace Sparrow.Video.Abstractions
{
    internal interface IExecutableProcess
    {
        Task StartAsync(string command);
        Task<string> StartWithOutputCatchAsync(string command);
        IExecutableProcess FilePathFromConfig(string configKey);
    }
}
