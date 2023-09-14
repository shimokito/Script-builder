using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Tirscript.Publish.Kubernetes.Build.Script.Reader
{
    /// <summary>
    /// Default process reader.
    /// </summary>
    internal class DefaultReaderOutput : IOutput
    {
        private readonly ProcessOutput _processOut;
        private readonly ConcurrentDictionary<DateTime, string> _output = new ConcurrentDictionary<DateTime, string>();
        private readonly ConcurrentDictionary<DateTime, string> _errors = new ConcurrentDictionary<DateTime, string>();

        public DefaultReaderOutput(ProcessOutput process)
        {
            _processOut = process;
            SubscribeProcessStdReader(process);
        }

        public void Start()
        {
            _processOut.StartRedirectOutput();
            _processOut.StartRedirectError();
        }

        private void SubscribeProcessStdReader(ProcessOutput process)
        {
            if (process.RedirectStdOutput)
            {
                process.OutputDataReceived += DataReceived;
            }

            if (process.RedirectStdError)
            {
                process.ErrorDataReceived += DataReceived;
                process.ErrorDataReceived += ErrorReceived;
            }
        }

        private void UnsubscribeProcessStdReader(ProcessOutput process)
        {
            if (process.RedirectStdOutput)
            {
                process.OutputDataReceived -= DataReceived;
            }

            if (process.RedirectStdError)
            {
                process.ErrorDataReceived -= DataReceived;
                process.ErrorDataReceived -= ErrorReceived;
            }
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (string.IsNullOrEmpty(data))
                return;

            _output.TryAdd(DateTime.UtcNow, data);
        }

        private void ErrorReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (string.IsNullOrEmpty(data))
                return;

            _errors.TryAdd(DateTime.UtcNow, data);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendJoin(Environment.NewLine, GetOutput());

            if (!_errors.IsEmpty)
            {
                sb.AppendLine("\r\nErrors: ");
                var errors = _errors.ToArray();
                sb.AppendJoin(Environment.NewLine, Format(errors));
            }

            return sb.ToString();
        }

        public void Dispose()
        {
            UnsubscribeProcessStdReader(_processOut);
        }

        static IEnumerable<string> Format(KeyValuePair<DateTime, string>[] outputs)
        {
            return outputs.OrderBy(x => x.Key).Select(x => string.Format("{0:hh:mm:ss.ffff} | {1}", x.Key, x.Value));
        }

        public IEnumerable<string> GetOutput()
        {
            var outputs = _output.ToArray();
            return Format(outputs);
        }
    }
}
