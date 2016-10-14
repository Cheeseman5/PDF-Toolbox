using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFToolbox.Helpers
{
    static class D
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

        private static eDebuggerDetail detail = eDebuggerDetail.All;

        private const string ASSERT_PREFIX = "Assertion failed: ";
        private const string LOG_PREFIX = "Log: ";
        private const string WARN_PREFIX = "WARNING: ";
        private const string ERR_PREFIX = "ERROR! ";

        private static void Write(string prefix, object msg)
        {
            Console.WriteLine(prefix + msg as string);
        }
        private static void Write(string prefix, object msg, params object[] args)
        {
            Write(prefix, string.Format(msg as string, args));
        }

        public static void Assert(bool condition, object msg)
        {
            if ((int)detail >= (int)eDebuggerDetail.Assert)
                if(!condition)
                    Write(ASSERT_PREFIX, msg);
        }
        public static void Assert(bool condition, object msg, params object[] args)
        {
            if ((int)detail >= (int)eDebuggerDetail.Assert)
                if (!condition)
                    Write(ASSERT_PREFIX, msg, args);
        }

        public static void Log(object msg)
        {
            if((int)detail >= (int)eDebuggerDetail.Log)
                Write(LOG_PREFIX, msg);
        }
        public static void Log(object msg, params object[] args)
        {
            if ((int)detail >= (int)eDebuggerDetail.Log)
                Write(LOG_PREFIX, msg, args);
        }

        public static void Warn(object msg)
        {
            if ((int)detail >= (int)eDebuggerDetail.Warning)
                Write(WARN_PREFIX, msg);
        }
        public static void Warn(object msg, params object[] args)
        {
            if ((int)detail >= (int)eDebuggerDetail.Warning)
                Write(WARN_PREFIX, msg, args);
        }

        public static void Error(object msg)
        {
            if ((int)detail >= (int)eDebuggerDetail.Error)
                Write(ERR_PREFIX, msg);
        }
        public static void Error(object msg, params object[] args)
        {
            if ((int)detail >= (int)eDebuggerDetail.Error)
                Write(ERR_PREFIX, msg, args);
        }

        //Fatal

    }
}
