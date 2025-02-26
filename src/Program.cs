using System.CommandLine;
using System.Threading.Tasks;

namespace DiskAnalysis;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Disk analysis utils");
        rootCommand.AddCommand(new CapacityCommand());

        return await rootCommand.InvokeAsync(args);
    }
}
