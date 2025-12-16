namespace substrate_shared.Enums
{
    public sealed class CodexEpochParams
    {
        public WindowType WindowType { get; }
        public int WindowSize { get; }

        public CodexEpochParams(WindowType type, int size)
        {
            WindowType = type;
            WindowSize = size;
        }
    }
}