namespace Sparrow.Video.Enums
{
    public class ConcatType
    {
        private ConcatType(string type)
        {
            _type = type;
        }

        private string _type = string.Empty;

        /// <summary>
        /// FFMpeg action looks like:
        /// 1. Reencoding your input videos to selected resolution and set libx264 encoding
        /// 2. Concatinating all this source in one video using: -f concat -safe 0 -i input-files -c:a copy -c:v copy -preset fast -vsync cfr -r 45
        /// </summary>
        public static readonly ConcatType ReencodingConcat = new ConcatType(nameof(ReencodingConcat));
        /// <summary>
        /// Analog of ReencodingConcat, but after Reencoding all videos converted to .ts file, after that streams copied in one concanitated file
        /// </summary>
        public static readonly ConcatType ReencodingConcatConvertedViaTransportStream = new ConcatType(nameof(ReencodingConcatConvertedViaTransportStream));
        public static readonly ConcatType ReencodingComplexFilter = new ConcatType(nameof(ReencodingComplexFilter));
        public static readonly ConcatType Demuxer = new ConcatType(nameof(Demuxer));

        public override string ToString() => _type;
        public override bool Equals(object? obj)
        {
            bool result = false;
            if (obj is ConcatType input)
                if (input._type == _type)
                    result = true;
            return result;
        }
    }
}
