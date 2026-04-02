using Core;

namespace ConsoleApp {
    class Program {
        static async Task Main(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("Usage: ConsoleApp <input-files-list> <output-directory> [options]");
                Console.WriteLine("Options:");
                Console.WriteLine("  --max-loading <n>     Max files to load simultaneously (default: 3)");
                Console.WriteLine("  --max-generation <n>  Max classes to generate simultaneously (default: 3)");
                Console.WriteLine("  --max-writing <n>     Max files to write simultaneously (default: 3)");
                return;
            }

            var inputFiles = new List<string>();

            inputFiles.AddRange(File.ReadAllLines(args[0]));

            var outputDirectory = args[1];

            int maxLoading = 3;
            int maxGeneration = 3;
            int maxWriting = 3;

            for (int i = 2; i < args.Length; i++) {
                switch (i) {
                    case 2:
                        maxLoading = int.Parse(args[i]);
                        break;
                    case 3:
                        maxGeneration = int.Parse(args[i]);
                        break;
                    case 4:
                        maxWriting = int.Parse(args[i]);
                        break;
                }
            }

            var pipeline = new TestGeneratorPipeline();

            try {
                Console.WriteLine("Starting test generation...");
                Console.WriteLine($"Input files: {inputFiles.Count}");
                Console.WriteLine($"Output directory: {outputDirectory}");
                Console.WriteLine($"Max loading parallelism: {maxLoading}");
                Console.WriteLine($"Max generation parallelism: {maxGeneration}");
                Console.WriteLine($"Max writing parallelism: {maxWriting}");

                await pipeline.ProcessAsync(inputFiles, outputDirectory, maxLoading, maxGeneration, maxWriting);

                Console.WriteLine("Test generation completed successfully!");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error during test generation: {ex.Message}");
                throw;
            }
        }
    }
}