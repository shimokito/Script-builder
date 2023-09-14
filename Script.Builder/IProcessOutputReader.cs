namespace Tirscript.Publish.Kubernetes.Build.Script
{
    /// <summary>
    /// Reader process output.
    /// </summary>
    public interface IProcessOutputReader
    {
        /// <summary>
        /// Start read process output.
        /// </summary>
        /// <param name="processOutput">Process output.</param>
        /// <returns>Ouput of process.</returns>
        IOutput Read(ProcessOutput processOutput);
    }
}
