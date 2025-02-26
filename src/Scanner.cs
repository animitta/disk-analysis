using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading;

namespace DiskAnalysis;

internal class Scanner(DirectoryInfo directoryInfo)
{
    public DirectoryEntry? Root { get; set; }
    private readonly DirectoryInfo _info = directoryInfo;
    public void Scan(CancellationToken cancellationToken = default)
    {
        Root = Scan(_info.FullName, cancellationToken);
        Console.WriteLine();
    }
    public void Print(long minPrintSize, int maxPrintDepth, CancellationToken cancellationToken = default)
    {
        if (Root == null)
        {
            Scan(cancellationToken);
        }

        if (Root == null)
        {
            Console.WriteLine($"指定目录不存在:{_info.FullName}");
        }
        else
        {
            Print(Root, 0, minPrintSize, maxPrintDepth);
        }
    }
    private void Print(DirectoryEntry entry, int depth, long minPrintSize, int maxPrintDepth)
    {
        if (entry.IsEmpty)
        {
            return;
        }

        if (entry.TotalSize >= minPrintSize)
        {
            var indent = GetIndentation(depth);
            var size = FormatSize(entry.TotalSize);
            var name = depth == 0 ? _info.FullName : entry.Name;
            Console.WriteLine($"{indent} 大小: {size} {name}");
        }

        if (depth < maxPrintDepth)
        {
            foreach (var child in entry.Children)
            {
                Print(child, depth + 1, minPrintSize, maxPrintDepth);
            }
        }
    }

    private static DirectoryEntry? Scan(string directoryPath, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(directoryPath))
        {
            return null;
        }
        PrintProcess(directoryPath);
        var entryRoot = new DirectoryEntry(Path.GetFileName(directoryPath));
        try
        {
            var paths = Directory.GetFileSystemEntries(directoryPath);
            foreach (var path in paths)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var attributes = File.GetAttributes(path);
                    if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        var entry = Scan(path, cancellationToken);
                        if (entry != null && !entry.IsEmpty)
                        {
                            entryRoot.AddChild(entry);
                        }
                    }
                    else
                    {
                        entryRoot.AddFile(new FileInfo(path).Length);
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("The operation was aborted");
                }
                catch (Exception)
                {
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
        }

        return entryRoot;
    }
    private static void PrintProcess(string directoryPath)
    {
        int currentTop = Console.CursorTop;
        Console.SetCursorPosition(0, currentTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentTop);

        var line = $"正在扫描 {directoryPath}";
        var maxWidth = Console.WindowWidth - 10;
        var width = CalculatePrintWidth(line);
        if (width > maxWidth)
        {
            var removeSize = width - maxWidth + 3;
            var start = maxWidth / 2 - removeSize / 2;

            var sb = new StringBuilder(line);
            sb.Remove(start, removeSize);
            sb.Insert(start, "...");
            line = sb.ToString();
        }
        Console.Write(line);
    }

    private static string GetIndentation(int depth)
    {
        return new string('-', depth * 4);
    }
    private static string FormatSize(long bytes)
    {
        return bytes switch
        {
            < 1024 => $"{bytes,6} 字节",
            < 1024 * 1024 => $"{bytes / 1024.0,6:F2} KB",
            < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024),6:F2} MB",
            _ => $"{bytes / (1024.0 * 1024 * 1024),6:F2} GB"
        };
    }
    private static int CalculatePrintWidth(string input)
    {
        int width = 0;
        var stringInfo = new StringInfo(input);
        for (int i = 0; i < stringInfo.LengthInTextElements; i++)
        {
            var c = stringInfo.SubstringByTextElements(i, 1)[0]; // 获取字符单元
            if (IsFullWidth(c))
            {
                width += 2;
            }
            else
            {
                width += 1;
            }
        }
        return width;
    }
    private static bool IsFullWidth(char c)
    {
        if (c >= 0x4E00 && c <= 0x9FFF)
        {
            return true;
        }

        if (c >= 0xFF00 && c <= 0xFFEF)
        {
            return true;
        }

        if (c == 0x3000)
        {
            return true;
        }

        if (char.GetUnicodeCategory(c) == UnicodeCategory.OtherPunctuation || char.GetUnicodeCategory(c) == UnicodeCategory.ModifierLetter)
        {
            return true;
        }

        return false;
    }
}
