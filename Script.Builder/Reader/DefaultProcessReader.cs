namespace Tirscript.Publish.Kubernetes.Build.Script.Reader
{
    /// <summary>
    /// Default process reader.
    /// </summary>
    internal class DefaultProcessReader : IProcessOutputReader
    {
        public IOutput Read(ProcessOutput processOutput)
        {
            var reader = new DefaultReaderOutput(processOutput);
            reader.Start();
            return reader;
        }
    }
}
