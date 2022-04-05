namespace PDFToolbox.Interfaces
{
    public interface ILogger
    {
        void Assert(bool condition, object msg);
        void Assert(bool condition, object msg, params object[] args);
        void Log(object msg);
        void Log(object msg, params object[] args);
        void Warn(object msg);
        void Warn(object msg, params object[] args);
        void Error(object msg);
        void Error(object msg, params object[] args);
    }
}