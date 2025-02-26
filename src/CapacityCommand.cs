using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using Handlers = System.CommandLine.Handler;

namespace DiskAnalysis;

internal class CapacityCommand : Command
{
    private readonly Option<long> _minPrintSizeOption;
    private readonly Option<int> _maxPrintDepthOption;
    private readonly Argument<string> _rootPathArgument;

    public CapacityCommand() : base("capacity")
    {
        _rootPathArgument = new(name: "path", description: "Root path");

        _minPrintSizeOption = new(name: "--min-print-size", () => 1024 * 1024 * 1024, description: "Minimum print byte size(default 1GB)");
        _minPrintSizeOption.AddAlias("-s");

        _maxPrintDepthOption = new(name: "--max-print-depth", () => 1, description: "Maximum print depth");
        _maxPrintDepthOption.AddAlias("-d");

        AddArgument(_rootPathArgument);
        AddOption(_minPrintSizeOption);
        AddOption(_maxPrintDepthOption);

        Handlers.SetHandler(this, Execute);
    }

    public void Execute(InvocationContext context)
    {
        var rootPath = context.ParseResult.GetValueForArgument(_rootPathArgument);
        var minPrintSize = context.ParseResult.GetValueForOption(_minPrintSizeOption);
        var maxPrintDepth = context.ParseResult.GetValueForOption(_maxPrintDepthOption);

        if (!Path.IsPathRooted(rootPath))
        {
            rootPath = Path.GetFullPath(rootPath);
        }

        var di = new DirectoryInfo(rootPath);
        var scanner = new Scanner(di);
        scanner.Print(minPrintSize, maxPrintDepth);
    }
}
