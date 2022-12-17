using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golfscript
{
    abstract class ErrorReporter
    {
        public delegate void ErrorHandler(object sender, string error);

        public event ErrorHandler? Error;

        public string? LastError { get; protected set; }

        protected void Report(string error)
        {
            LastError = error;
            Error?.Invoke(this, error);
        }
    }
}
