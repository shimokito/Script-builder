using System.Diagnostics;

namespace Tirscript.Publish.Kubernetes.Build.Script
{
    /// <summary>
    /// Process output.
    /// </summary>
    public class ProcessOutput
    {
        private readonly Process _process;

        /// <summary>
        /// <inheritdoc cref="Process.ProcessName"/>
        /// </summary>
        public string ProcessName => _process.ProcessName;
        /// <summary>
        /// <inheritdoc cref="Process.StandardOutput"/>
        /// </summary>
        public StreamReader StdOutput => _process.StandardOutput;
        /// <summary>
        /// <inheritdoc cref="Process.StandardError"/>
        /// </summary>
        public StreamReader StdError => _process.StandardError;
        /// <summary>
        /// <inheritdoc cref="ProcessStartInfo.RedirectStandardOutput"/>
        /// </summary>
        public bool RedirectStdOutput => _process.StartInfo.RedirectStandardOutput;
        /// <summary>
        /// <inheritdoc cref="ProcessStartInfo.RedirectStandardError"/>
        /// </summary>
        public bool RedirectStdError => _process.StartInfo.RedirectStandardError;
        /// <summary>
        /// <inheritdoc cref="Process.OutputDataReceived"/>
        /// </summary>
        public event DataReceivedEventHandler? OutputDataReceived
        {
            add
            {
                _process.OutputDataReceived += value;
            }
            remove
            {
                _process.OutputDataReceived -= value;
            }
        }
        /// <summary>
        /// <inheritdoc cref="Process.ErrorDataReceived"/>
        /// </summary>
        public event DataReceivedEventHandler? ErrorDataReceived
        {
            add
            {
                _process.ErrorDataReceived += value;
            }
            remove
            {
                _process.ErrorDataReceived -= value;
            }
        }

        internal ProcessOutput(Process process)
        {
            _process = process;
        }

        /// <summary>
        /// Start redirect output on <see cref="OutputDataReceived"/>.
        /// </summary>
        public void StartRedirectOutput()
        {
            _process.BeginOutputReadLine();
        }

        /// <summary>
        /// Stop redirect output on <see cref="OutputDataReceived"/>.
        /// </summary>
        public void StopRedirectOutput()
        {
            _process.CancelOutputRead();
        }

        /// <summary>
        /// Start redirect output on <see cref="ErrorDataReceived"/>.
        /// </summary>
        public void StartRedirectError()
        {
            _process.BeginErrorReadLine();
        }

        /// <summary>
        /// Stop redirect output on <see cref="ErrorDataReceived"/>.
        /// </summary>
        public void StopRedirectError()
        {
            _process.CancelErrorRead();
        }
    }
}
