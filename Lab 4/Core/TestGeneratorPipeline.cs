using System.Threading.Tasks.Dataflow;

namespace Core {
    public class TestGeneratorPipeline {
        private CodeParser _parser;
        private TestGenerator _generator;

        public TestGeneratorPipeline() {
            _parser = new CodeParser();
            _generator = new TestGenerator();
        }

        public async Task ProcessAsync(IEnumerable<string> inputFiles, string outputDirectory,
            int maxLoadingParallelism, int maxGenerationParallelism, int maxWritingParallelism) {
            Directory.CreateDirectory(outputDirectory);

            var loadBlock = new TransformBlock<string, (string FileName, string Content)>(
                async filePath => {
                    var content = await File.ReadAllTextAsync(filePath);
                    return (Path.GetFileName(filePath), content);
                },
                new ExecutionDataflowBlockOptions {
                    MaxDegreeOfParallelism = maxLoadingParallelism
                });

            var generateBlock = new TransformBlock<(string FileName, string Content), List<(string TestClassName, string TestContent)>>(
                async input => {
                    var parseResult = await _parser.ParseAsync(input.Content);
                    var results = new List<(string, string)>();

                    foreach (var classInfo in parseResult.Classes) {
                        var testContent = _generator.Generate(classInfo, parseResult.Usings);
                        var testClassName = $"{classInfo.Name}Tests.cs";
                        results.Add((testClassName, testContent));
                    }

                    return results;
                },
                new ExecutionDataflowBlockOptions {
                    MaxDegreeOfParallelism = maxGenerationParallelism
                });

            var writeBlock = new ActionBlock<List<(string TestClassName, string TestContent)>>(
                async testClasses => {
                    foreach (var (testClassName, testContent) in testClasses) {
                        var outputPath = Path.Combine(outputDirectory, testClassName);
                        await File.WriteAllTextAsync(outputPath, testContent);
                    }
                },
                new ExecutionDataflowBlockOptions {
                    MaxDegreeOfParallelism = maxWritingParallelism
                });

            loadBlock.LinkTo(generateBlock, new DataflowLinkOptions { PropagateCompletion = true });
            generateBlock.LinkTo(writeBlock, new DataflowLinkOptions { PropagateCompletion = true });

            foreach (var file in inputFiles) {
                await loadBlock.SendAsync(file);
            }

            loadBlock.Complete();
            await writeBlock.Completion;
        }
    }
}