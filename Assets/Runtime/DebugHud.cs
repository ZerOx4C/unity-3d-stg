using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DebugHud : MonoBehaviour
{
    private const float LogLifetime = 5f;
    private static List<LogEntry> _logEntries = new();
    private static bool _isLogDirty = true;
    public Text logText;

    private void Update()
    {
        foreach (var e in _logEntries)
        {
            e.Lifetime -= Time.deltaTime;
        }

        var newEntries = _logEntries.Where(e => 0 < e.Lifetime).ToList();
        if (newEntries.Count != _logEntries.Count)
        {
            _isLogDirty = true;
        }

        if (!_isLogDirty)
        {
            return;
        }

        _logEntries = newEntries;
        logText.text = string.Join("\n", _logEntries.Select(e => e.Value));
        _isLogDirty = false;
    }

    public static void Log(string key, object value)
    {
        var entry = _logEntries.Find(e => e.Key == key);
        if (entry == null)
        {
            entry = new LogEntry { Key = key };
            _logEntries.Add(entry);
            _logEntries = _logEntries.OrderBy(e => e.Key).ToList();
            _isLogDirty = true;
        }

        entry.Lifetime = LogLifetime;

        var newValue = value.ToString();
        if (entry.Value == newValue)
        {
            return;
        }

        entry.Value = newValue;
        _isLogDirty = true;
    }

    private class LogEntry
    {
        public string Key;
        public float Lifetime;
        public string Value;
    }
}
