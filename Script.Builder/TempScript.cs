using System.Text;

namespace Tirscript.Publish.Kubernetes.Build.Script
{
    /// <summary>
    /// Временный скрипт.
    /// </summary>
    internal class TempScript : IDisposable
    {
        private readonly string _pathToScript;

        public string PathToScript => _pathToScript;

        private TempScript(string pathToScript)
        {
            _pathToScript = pathToScript;
        }

        public static TempScript Create(string name, string scriptContent, string? path = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Temporary script name is null or empty.");

            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var pathToScript = Path.Combine(path, name);
            File.WriteAllText(pathToScript, scriptContent);
            return new TempScript(pathToScript);
        }

        public static async Task<TempScript> CreateAsync(string name, string scriptContent, string? path = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Temporary script name is null or empty.");

            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var pathToScript = Path.Combine(path, name);
            await File.WriteAllTextAsync(pathToScript, scriptContent);
            return new TempScript(pathToScript);
        }

        public static TempScript Create(string name, IEnumerable<string> commands, string? path = null)
        {
            var sb = new StringBuilder();
            sb.AppendJoin(Environment.NewLine, commands);
            return Create(name, sb.ToString(), path);
        }

        public static Task<TempScript> CreateAsync(string name, IEnumerable<string> commands, string? path = null)
        {
            var sb = new StringBuilder();
            sb.AppendJoin(Environment.NewLine, commands);
            return CreateAsync(name, sb.ToString(), path);
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(_pathToScript))
                {
                    File.Delete(_pathToScript);
                }
            }
            catch (FileNotFoundException)
            {
                //ignored
            }
        }
    }
}
