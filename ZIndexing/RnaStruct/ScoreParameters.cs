namespace ZIndexing.RnaStruct
{
    public interface ScoreParameters
    {
        string Source { get; set; }
        void Compute(string source);
        void Render();
    }
}