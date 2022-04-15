namespace Sparrow.Video.Entities
{
    public class OutputAdditionalSettings
    {
        public OutputAdditionalSettings(string originalSource)
        {
            OriginalSource = originalSource;
            _manipulation = new FileManipulation();
        }

        public OutputAdditionalSettings(string originalSource, FileManipulation manipulation) : this(originalSource)
        {
            AddSettings(manipulation);
        }

        public string OriginalSource { get; private set; }
        public FileManipulation Manipulation { get => _manipulation; }
        private FileManipulation _manipulation;

        public void AddSettings(FileManipulation manipulation)
        {
            if (manipulation != null)
                _manipulation = manipulation;
        }
    }

    public class FileManipulation
    {
        public int Loop { get; set; } = 1;
    }
}
