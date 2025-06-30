using AfterWindowsInstaller.Core;

using System.Diagnostics;

namespace AfterWindowsInstaller.infrastructure.Extensions
{
    public static class ProcessExtensions
    {
        public static async Task ListenProcessAsync(this Process process, CancellationToken cancellationToken)
        {
            try
            {
                using var _ = cancellationToken.Register(() =>
                {
                    try
                    {
                        if (!process.HasExited)
                            process.Kill(true);
                    }
                    catch (InvalidOperationException) { }
                });

                await process.WaitForExitAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            { }
        }
    }
}
