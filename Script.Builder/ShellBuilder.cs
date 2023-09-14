namespace Tirscript.Publish.Kubernetes.Build.Script
{
    /// <summary>
    /// Локальный билдер скриптов.
    /// </summary>
    internal class ShellBuilder
    {
        const string BuildScriptFileNameFormat = "{0}_build-script.{1}";

        private readonly string? _tempPath;

        public ShellBuilder()
            : this(null)
        {
        }

        public ShellBuilder(string? tempPath)
        {
            _tempPath = tempPath;
        }

        public string? BuildAsScript(IEnumerable<string> commands)
        {
            using var tempScript = TempScript.Create(string.Format(BuildScriptFileNameFormat, Guid.NewGuid(), GetCurrentPlatformScriptExtension()), commands, _tempPath);
            var output = ScriptExecutor.Start(tempScript.PathToScript, true);
            return output;
        }

        public async Task<string?> BuildAsScriptAsync(IEnumerable<string> commands)
        {
            using var tempScript = await TempScript.CreateAsync(string.Format(BuildScriptFileNameFormat, Guid.NewGuid(), GetCurrentPlatformScriptExtension()), commands, _tempPath);
            var output = await ScriptExecutor.StartAsync(tempScript.PathToScript, true);
            return output;
        }

        public string? BuildAsScript(string scriptContent)
        {
            using var tempScript = TempScript.Create(string.Format(BuildScriptFileNameFormat, Guid.NewGuid(), GetCurrentPlatformScriptExtension()), scriptContent, _tempPath);
            var output = ScriptExecutor.Start(tempScript.PathToScript, true);
            return output;
        }

        public async Task<string?> BuildAsScriptAsync(string scriptContent)
        {
            using var tempScript = await TempScript.CreateAsync(string.Format(BuildScriptFileNameFormat, Guid.NewGuid(), GetCurrentPlatformScriptExtension()), scriptContent, _tempPath);
            var output = await ScriptExecutor.StartAsync(tempScript.PathToScript, true);
            return output;
        }

        private static string GetCurrentPlatformScriptExtension()
        {
            return Environment.OSVersion.Platform switch
            {
                PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.Win32NT or PlatformID.WinCE => "bat",
                PlatformID.Unix => "sh",
                _ => throw new PlatformNotSupportedException(),
            };
        }
    }
}
