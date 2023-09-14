namespace Tirscript.Publish.Kubernetes.Build.Script
{
    /// <summary>
    /// Output provider.
    /// </summary>
    public interface IOutput : IDisposable
    {
        /// <summary>
        /// Getting process output.
        /// </summary>
        /// <returns>Process output line by line.</returns>
        IEnumerable<string> GetOutput();
    }
}
