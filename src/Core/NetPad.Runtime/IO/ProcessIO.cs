using System.Diagnostics;
using System.IO;

namespace NetPad.IO;

public class ProcessIO : IDisposable
{
    public ProcessIO(Process process)
    {
        Process = process;

        OnOutputReceivedHandlers = new HashSet<Func<string, Task>>();
        OnErrorReceivedHandlers = new HashSet<Func<string, Task>>();

        Process.OutputDataReceived += OutputReceived;
        Process.ErrorDataReceived += ErrorReceived;
    }

    public Process Process { get; }

    public StreamWriter StandardInput => Process.StandardInput;
    public HashSet<Func<string, Task>> OnOutputReceivedHandlers { get; }
    public HashSet<Func<string, Task>> OnErrorReceivedHandlers { get; }

    private void OutputReceived(object? sender, DataReceivedEventArgs ev)
    {
        if (ev.Data == null)
            return;

        foreach (var handler in OnOutputReceivedHandlers)
        {
            Task.Run(async () => { await handler(ev.Data); });
        }
    }

    private void ErrorReceived(object? sender, DataReceivedEventArgs ev)
    {
        if (ev.Data == null)
            return;

        foreach (var handler in OnErrorReceivedHandlers)
        {
            AsyncUtil.RunSync(async () => await handler(ev.Data));
        }
    }

    public void Dispose()
    {
        Process.OutputDataReceived -= OutputReceived;
        Process.ErrorDataReceived -= ErrorReceived;
        OnOutputReceivedHandlers.Clear();
        OnErrorReceivedHandlers.Clear();
    }
}
