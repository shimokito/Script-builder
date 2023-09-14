namespace Tirscript.Publish.Kubernetes.Build.Script
{
    /// <summary>
    /// Async reader process output.
    /// </summary>
    public interface IAsyncProcessOutputReader
    {
        /// <summary>
        /// Start read process output.
        /// </summary>
        /// <param name="processOutput">Process output.</param>
        /// <returns>Ouput of process.</returns>
        IAsyncOutput Read(ProcessOutput processOutput);
    }
}
