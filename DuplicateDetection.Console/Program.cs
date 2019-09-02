using System;
using System.Collections.Generic;
using System.Linq;
using DuplicateDetection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DuplicateDetection.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // init services
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddTransient<IDuplicateDetectionService, DuplicateDetectionService>()
                .AddTransient<IFileCrawler, FileCrawler>()
                .AddTransient<IFileHashService>(services => new CashingFileHashService(new FileHashService()))
                .AddTransient<Application>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var application = serviceProvider.GetRequiredService<Application>();

            try
            {
                var (path, mode) = GetArguments(args);
                application.Run(path, mode);
            }
            catch
            {
                // display some help
                System.Console.WriteLine("First argument should be a valid filepath.");
                System.Console.WriteLine("Second argument can be '--size' for size only comparison or empty for size and name comparison.");
                throw;
            }
        }

        private static (string path, ComparisonMode mode) GetArguments(string[] args)
        {
            var pathArg = args[0];
            var mode = ComparisonMode.SizeAndName;
            if (args.Length > 1 && args[1] == "--size")
            {
                mode = ComparisonMode.Size;
            }

            return (pathArg, mode);
        }
    }
}
