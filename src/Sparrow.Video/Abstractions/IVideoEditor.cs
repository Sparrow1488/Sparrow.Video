using Sparrow.Video.Entities;
using Sparrow.Video.Enums;

namespace Sparrow.Video.Abstractions
{
    public interface IVideoEditor
    {
        event LogAction OnCachedSource;
        event LogAction OnConvertedSource;
        delegate void LogAction(string message);

        IVideoEditor Configure(Action<Configuration> config);
        Task ConcatSourcesAsync(ConcatType concatType);
    }
}
