using System.Collections.Generic;

namespace DiskAnalysis;

internal class DirectoryEntry(string name)
{
    /// <summary>
    /// 文件夹名称
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 非空子文件夹
    /// </summary>
    public List<DirectoryEntry> Children { get; set; } = [];

    /// <summary>
    /// 文件数量
    /// </summary>
    public int FileCount = 0;
    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileTotalSize = 0;

    /// <summary>
    /// 是否为空文件夹
    /// </summary>
    public bool IsEmpty => Children.Count == 0 && FileCount == 0;

    /// <summary>
    /// 总大小
    /// </summary>
    public long TotalSize { get; private set; }

    public void AddFile(long size)
    {
        FileCount++;
        FileTotalSize += size;
        TotalSize += size;
    }

    public void AddChild(DirectoryEntry child)
    {
        if (child.IsEmpty)
        {
            return;
        }

        Children.Add(child);
        TotalSize += child.TotalSize;
    }
}
