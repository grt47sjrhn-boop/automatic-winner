namespace substrate_shared.interfaces
{
    public interface ISummary
    {
        string Title { get; }
        string Description { get; }
        void Print();
        string ToString();
    }
}