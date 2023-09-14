namespace Tirscript.Publish.Kubernetes.Build.Script
{
    /// <summary>
    /// Asynchronious output provider.
    /// </summary>
    public interface IAsyncOutput : IDisposable
    {
        /// <summary>
        /// Async getting process output.
        /// </summary>
        /// <returns>Process output line by line.</returns>
        IAsyncEnumerable<string> GetOuputAsync();
    }
}
