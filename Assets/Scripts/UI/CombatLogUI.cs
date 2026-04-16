using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatLogUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI logText;

    [Header("Settings")]
    public int maxEntries = 12;

    private readonly Queue<string> logEntries = new Queue<string>();

    public void AddEntry(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        logEntries.Enqueue(message);

        while (logEntries.Count > maxEntries)
        {
            logEntries.Dequeue();
        }

        RefreshText();
    }

    public void ClearLog()
    {
        logEntries.Clear();
        RefreshText();
    }

    private void RefreshText()
    {
        if (logText == null)
            return;

        logText.text = string.Join("\n", logEntries);
    }
}