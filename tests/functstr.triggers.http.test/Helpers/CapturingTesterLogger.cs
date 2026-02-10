using functstr.core;
using System.Text;

namespace functstr.triggers.http.test.Helpers
{
    internal sealed class CapturingTesterLogger : ITesterLogger
    {
        private readonly StringBuilder _sb = new();

        public void Log(string message)
        {
            _sb.AppendLine(message);
        }

        public override string ToString() => _sb.ToString();
    }
}
