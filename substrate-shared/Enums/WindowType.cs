namespace substrate_shared.Enums
{
    public enum WindowType
    {
        Tick,   // epoch defined by number of ticks
        Count   // epoch defined by number of duels
    }

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