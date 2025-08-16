using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class LogEntry
{
    public string Severity { get; }
    public string Code { get; }

    private static readonly Regex LogPattern = new Regex(@"(ERROR|WARN):(\w+)", RegexOptions.IgnoreCase);

    public LogEntry(string logLine)
    {
        var match = LogPattern.Match(logLine);
        if (match.Success)
        {
            Severity = match.Groups[1].Value.ToUpper();
            Code = match.Groups[2].Value.ToUpper();
        }
        else
        {
            Severity = null;
            Code = null;
        }
    }

    public bool HasCode => Code != null;
}

public class LogAnalyzer
{
    public static Dictionary<string, int> CountErrorCodes(List<string> logLines)
    {
        var codeCounts = new Dictionary<string, int>();

        foreach (var line in logLines)
        {
            var entry = new LogEntry(line);
            if (entry.HasCode)
            {
                if (codeCounts.ContainsKey(entry.Code))
                {
                    codeCounts[entry.Code]++;
                }
                else
                {
                    codeCounts[entry.Code] = 1;
                }
            }
        }

        return codeCounts;
    }

    public static void PrintTopKCodes(Dictionary<string, int> codeCounts, int topK)
    {
        if (codeCounts.Count == 0)
        {
            Console.WriteLine("No error codes found in logs.");
            return;
        }

        var totalCodes = codeCounts.Values.Sum();
        var sortedCodes = codeCounts.OrderByDescending(kv => kv.Value)
                                   .ThenBy(kv => kv.Key)
                                   .Take(topK);

        Console.WriteLine($"Top {topK} error codes:");
        Console.WriteLine("----------------------");
        Console.WriteLine("| Code | Count | Percentage |");
        Console.WriteLine("|------|-------|------------|");

        foreach (var pair in sortedCodes)
        {
            double percentage = Math.Round((double)pair.Value / totalCodes * 100, 2);
            Console.WriteLine($"| {pair.Key,-4} | {pair.Value,5} | {percentage,8}% |");
        }
    }
}
namespace Log_Analyzer_Error_Frequency
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var logLines = new List<string>();

            Console.WriteLine("Enter log lines (enter blank line to finish):");
            while (true)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    break;

                logLines.Add(line);
            }

            Console.Write("Enter how many top codes to show: ");
            int topK = int.Parse(Console.ReadLine());

            var codeCounts = LogAnalyzer.CountErrorCodes(logLines);
            LogAnalyzer.PrintTopKCodes(codeCounts, topK);
        }
    }
}
