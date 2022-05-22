namespace PDFToolbox.Interfaces
{
    public interface IConfig
    {
        string DefaultSaveDirectory { get; }
        string DefaultTempSaveDirectory { get; }
        string TempSavePrefix { get; }
        string AppVersion { get; }
        string AppName { get; }
        bool SilenceMessagebox { get; }
    }
}
