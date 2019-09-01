using DuplicateDetection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DuplicateDetection.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddTransient<IDuplicateDetectionService, DuplicateDetectionService>()
                .AddTransient<IFileCrawler, FileCrawler>()
                .AddTransient<IFileHashService>(services => new CashingFileHashService(new FileHashService()));

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var duplicateDetectionService = serviceProvider.GetRequiredService<IDuplicateDetectionService>();

            var duplicateFiles = duplicateDetectionService.CollectCandidates(args[0]);
        }
    }
}
