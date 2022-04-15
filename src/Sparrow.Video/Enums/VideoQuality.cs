using Newtonsoft.Json;

namespace Sparrow.Video.Enums
{
    public class VideoQuality
    {
        [JsonConstructor]
        private VideoQuality(int width, int height, string name)
        {
            Height = height;
            Width = width;
            Name = name;
        }

        public readonly int Height;
        public readonly int Width;
        public readonly string Name = string.Empty;

        //public static readonly VideoQuality QuadHD = new VideoQuality(2560, 1440, nameof(QuadHD));
        public static readonly VideoQuality FHD = new VideoQuality(1920, 1080, nameof(FHD));
        //public static readonly VideoQuality HD = new VideoQuality(1280, 720, nameof(HD));
        //public static readonly VideoQuality Preview = new VideoQuality(640, 480, nameof(Preview));
        public static readonly VideoQuality Undefined = new VideoQuality(0, 0, nameof(Undefined));

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}
