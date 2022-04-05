using PDFToolbox.Interfaces;
using System;

namespace PDFToolbox.Helpers
{
    public class ConsoleLogger : ILogger
    {
        public enum eDebuggerDetail
        {
            None = 0,
            Assert,
            Log,
            Warning,
            Error,
            Fatal,
            All
        };

        private eDebuggerDetail detail = eDebuggerDetail.All;

        private const string ASSERT_PREFIX = "Assertion failed: ";
        private const string LOG_PREFIX = "Log: ";
        private const string WARN_PREFIX = "WARNING: ";
        private const string ERR_PREFIX = "ERROR! ";

        public ConsoleLogger(eDebuggerDetail loggingDetail)
        {
            detail = loggingDetail;
        }

        private void Write(string prefix, object msg)
        {
            Console.WriteLine(prefix + msg as string);
        }
        private void Write(string prefix, object msg, params object[] args)
        {
            Write(prefix, string.Format(msg as string, args));
        }

        public void Assert(bool condition, object msg)
        {
            if ((int)detail >= (int)eDebuggerDetail.Assert)
                if (!condition)
                    Write(ASSERT_PREFIX, msg);
        }
        public void Assert(bool condition, object msg, params object[] args)
        {
            if ((int)detail >= (int)eDebuggerDetail.Assert)
                if (!condition)
                    Write(ASSERT_PREFIX, msg, args);
        }

        public void Log(object msg)
        {
            if ((int)detail >= (int)eDebuggerDetail.Log)
                Write(LOG_PREFIX, msg);
        }
        public void Log(object msg, params object[] args)
        {
            if ((int)detail >= (int)eDebuggerDetail.Log)
                Write(LOG_PREFIX, msg, args);
        }

        public void Warn(object msg)
        {
            if ((int)detail >= (int)eDebuggerDetail.Warning)
                Write(WARN_PREFIX, msg);
        }
        public void Warn(object msg, params object[] args)
        {
            if ((int)detail >= (int)eDebuggerDetail.Warning)
                Write(WARN_PREFIX, msg, args);
        }

        public void Error(object msg)
        {
            if ((int)detail >= (int)eDebuggerDetail.Error)
                Write(ERR_PREFIX, msg);
        }
        public void Error(object msg, params object[] args)
        {
            if ((int)detail >= (int)eDebuggerDetail.Error)
                Write(ERR_PREFIX, msg, args);
        }
    }
}