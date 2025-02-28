# disk-analysis

> 磁盘统计分析工具,用来分析磁盘的文件占用等信息。
> 本项目尚在开发中。
> 项目支持所有主流.NET支持的平台

## 构建
- 请从下方给出的链接下载并安装.NET 8 SDK,本项目基于dotnet 8(支持AOT)
  - [dotnet 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- 构建项目
    ```bash
    git clone git@github.com:animitta/disk-analysis.git

    cd disk-analysis/src

    # 构建Windows x64平台的AOT二进制文件
    # 最终生成的文件位于: artifact\win-x64\disk-analysis.exe
    dotnet publish /p:PublishProfile=Properties/PublishProfiles/x64.pubxml
    ```

## 使用

- 使用帮助
    ```bash
    # 命令行
    $ disk-analysis --help

    # 输出
    Description:
      Disk analysis utils

    Usage:
      disk-analysis [command] [options]

    Options:
      --version       Show version information
      -?, -h, --help  Show help and usage information

    Commands:
      capacity <path>
    ```
- 分析C盘Windows目录大于1GB文件的文件夹
    ```
    # 命令行
    disk-analysis capacity --min-print-size 1073741824 --max-print-depth 10 "C:\Windows"

    # 输出
    大小:  33.04 GB C:\Windows
    ---- 大小:   3.85 GB assembly
    -------- 大小:   1.14 GB NativeImages_v4.0.30319_32
    -------- 大小:   2.68 GB NativeImages_v4.0.30319_64
    ---- 大小:   3.37 GB Installer
    ---- 大小:   7.98 GB System32
    -------- 大小:   3.20 GB DriverStore
    ------------ 大小:   3.20 GB FileRepository
    ---- 大小:   1.22 GB SysWOW64
    ---- 大小:  13.24 GB WinSxS
    ```
