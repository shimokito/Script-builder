using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using Tirscript.Publish.Kubernetes.Build.Script.Reader;

namespace Tirscript.Publish.Kubernetes.Build.Script
{
    /// <summary>
    /// Shell script executor. Start shell scripts with provided output reader mechanisms.
    /// </summary>
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("windows")]
    internal static class ScriptExecutor
    {
        const string UnixShell = "/bin/bash";
        const string WindowsShell = "cmd.exe";

        /// <summary>
        /// Start shell script.
        /// </summary>
        /// <param name="pathToScript">Path to shell script.</param>
        /// <param name="priveleged">Start script as priveleged.</param>
        /// <returns>Ouput string of script.</returns>
        public static string? Start(string pathToScript, bool priveleged = false)
        {
            var reader = new DefaultProcessReader();
            var output = Start(pathToScript, reader, priveleged);
            return output.ToString();
        }

        /// <summary>
        /// Start shell script async.
        /// </summary>
        /// <param name="pathToScript">Path to shell script.</param>
        /// <param name="priveleged">Start script as priveleged.</param>
        /// <returns>Ouput string of script.</returns>
        public static async Task<string?> StartAsync(string pathToScript, bool priveleged = false)
        {
            var reader = new DefaultProcessReader();
            var output = await StartAsync(pathToScript, reader, priveleged);
            return output.ToString();
        }

        /// <summary>
        /// Start shell script.
        /// </summary>
        /// <param name="pathToScript">Path to shell script.</param>
        /// <param name="reader">Ouput reader.</param>
        /// <param name="priveleged">Start script as privileged.</param>
        /// <param name="waitForExit">Wait completion script.</param>
        /// <returns>Ouput of script.</returns>
        public static IOutput Start(string pathToScript, IProcessOutputReader reader, bool priveleged = false, bool waitForExit = true)
        {
            var process = StartShell(pathToScript, out var processOut, priveleged);
            var output = reader.Read(processOut);

            if (waitForExit)
                process.WaitForExit();

            return output;
        }

        /// <summary>
        /// Start shell script async.
        /// </summary>
        /// <param name="pathToScript">Path to shell script.</param>
        /// <param name="reader">Ouput reader.</param>
        /// <param name="priveleged">Start script as privileged.</param>
        /// <param name="waitForExit">Wait completion script.</param>
        /// <returns>Ouput of script.</returns>
        public static async Task<IOutput> StartAsync(string pathToScript, IProcessOutputReader reader, bool priveleged = false, bool waitForExit = true)
        {
            var process = StartShell(pathToScript, out var processOut, priveleged);
            var output = reader.Read(processOut);

            if (waitForExit)
                await process.WaitForExitAsync();

            return output;
        }

        /// <summary>
        /// Start shell script.
        /// </summary>
        /// <param name="pathToScript">Path to shell script.</param>
        /// <param name="reader">Ouput reader.</param>
        /// <param name="priveleged">Start script as privileged.</param>
        /// <param name="waitForExit">Wait completion script.</param>
        /// <returns>Async ouput of script.</returns>
        public static IAsyncOutput Start(string pathToScript, IAsyncProcessOutputReader reader, bool priveleged = false, bool waitForExit = true)
        {
            var process = StartShell(pathToScript, out var processOut, priveleged);
            var output = reader.Read(processOut);

            if (waitForExit)
                process.WaitForExit();

            return output;
        }

        /// <summary>
        /// Start shell script async.
        /// </summary>
        /// <param name="pathToScript">Path to shell script.</param>
        /// <param name="reader">Ouput reader.</param>
        /// <param name="priveleged">Start script as privileged.</param>
        /// <param name="waitForExit">Wait completion script.</param>
        /// <returns>Async ouput of script.</returns>
        public static async Task<IAsyncOutput> StartAsync(string pathToScript, IAsyncProcessOutputReader reader, bool priveleged = false, bool waitForExit = true)
        {
            var process = StartShell(pathToScript, out var processOut, priveleged);
            var output = reader.Read(processOut);

            if (waitForExit)
                await process.WaitForExitAsync();

            return output;
        }

        /// <summary>
        /// Start shell script.
        /// </summary>
        /// <param name="pathToScript">Path to shell script.</param>
        /// <param name="output">Process output.</param>
        /// <param name="priveleged">Start scrip as privileged.</param>
        /// <returns>Process of shell script.</returns>
        private static Process StartShell(string pathToScript, out ProcessOutput output, bool priveleged = false)
        {
            var process = GetShellProcess(pathToScript, priveleged);
            output = new ProcessOutput(process);

            process.Start();
            return process;
        }

        /// <summary>
        /// Getting configured shell process.
        /// </summary>
        /// <param name="pathToScript">Path to script.</param>
        /// <param name="priveleged">Signalize if script need to start as privileged.</param>
        /// <returns>Proess of shell script.</returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        private static Process GetShellProcess(string pathToScript, bool priveleged)
        {
            var platform = Environment.OSVersion.Platform;
            var startInfo = GetShellStartInfo(platform, false, true, true);
            string format = platform switch
            {
                PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.Win32NT or PlatformID.WinCE => GetCmdExec(priveleged),
                PlatformID.Unix => GetBashExec(priveleged),
                _ => throw new PlatformNotSupportedException(),
            };

            var argSb = new StringBuilder();
            argSb.AppendFormat(format, pathToScript);

            startInfo.Arguments = argSb.ToString();

            return new Process
            {
                StartInfo = startInfo
            };
        }

        static string GetBashExec(bool privileged)
        {
            return privileged
                ? "-c \"sudo bash {0}\""
                : "-c \"{0}\"";
        }

        static string GetCmdExec(bool priveleged)
        {
            return "/c \"{0}\"";
        }

        /// <summary>
        /// Gettings shell startup info.
        /// </summary>
        /// <param name="platform">Current platform.</param>
        /// <param name="redirectStdInput">Start redirect standard input.</param>
        /// <param name="redirectStdOutput">Start redirect standard output.</param>
        /// <param name="redirectStdError">Start redirect standard error.</param>
        /// <returns>Startup info.</returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        private static ProcessStartInfo GetShellStartInfo(PlatformID platform, bool redirectStdInput, bool redirectStdOutput, bool redirectStdError)
        {
            string shellStartup = platform switch
            {
                PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.Win32NT or PlatformID.WinCE => WindowsShell,
                PlatformID.Unix => UnixShell,
                _ => throw new PlatformNotSupportedException(),
            };

            return new ProcessStartInfo
            {
                FileName = shellStartup,
                RedirectStandardInput = redirectStdInput,
                RedirectStandardOutput = redirectStdOutput,
                RedirectStandardError = redirectStdError,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
        }
    }
}
