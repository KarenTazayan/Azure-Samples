using System.Collections.Concurrent;

namespace WebAppForBot;

public class WebLogService
{
    private readonly ConcurrentBag<string> _logBag = [];
    public event Action? OnLogAdded;

    public void AddLog(string log)
    {
        _logBag.Add($"{DateTime.UtcNow.ToLongTimeString()} : {log}");
        OnLogAdded?.Invoke();
    }

    public IEnumerable<string> GetLogs()
    {
        return _logBag;
    }
}