using System;
using EasyNetQ;
using EasyNetQ.Loggers;

namespace EasyNetQCommon
{
    public class NoDebugLogger : IEasyNetQLogger
    {
        readonly IEasyNetQLogger _inner = new ConsoleLogger();

        public void DebugWrite(string format, params object[] args)
        {
        }

        public void InfoWrite(string format, params object[] args)
        {
            _inner.InfoWrite(format, args);
        }

        public void ErrorWrite(string format, params object[] args)
        {
            _inner.ErrorWrite(format, args);
        }

        public void ErrorWrite(Exception exception)
        {
            _inner.ErrorWrite(exception);
        }
    }
}
