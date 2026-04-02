using Core;
using Xunit;

namespace Tests {
    public class CodeParserTests {
        [Fact]
        public async Task ShouldDetectPublicMethods() {
            var parser = new CodeParser();
            var sourceCode = @"
                using System;
                
                namespace MyNamespace
                {
                    public class MyClass
                    {
                        public void Method1() { }
                        public int Method2(string param) { return 0; }
                        private void PrivateMethod() { }
                        public static void StaticMethod() { }
                    }
                }";

            var result = await parser.ParseAsync(sourceCode);

            Assert.Single(result.Classes);
            var classInfo = result.Classes[0];
            Assert.Equal("MyClass", classInfo.Name);
            Assert.Equal(2, classInfo.PublicMethods.Count);
            Assert.Equal("Method1", classInfo.PublicMethods[0].Name);
            Assert.Equal("Method2", classInfo.PublicMethods[1].Name);
        }

        [Fact]
        public async Task ShouldDetectConstructorParameters() {
            var parser = new CodeParser();
            var sourceCode = @"
                namespace MyNamespace
                {
                    public interface IService { }
                    public class MyClass
                    {
                        public MyClass(IService service, int value, string name)
                        {
                        }
                        
                        public void Method() { }
                    }
                }";

            var result = await parser.ParseAsync(sourceCode);

            Assert.Single(result.Classes);
            var classInfo = result.Classes[0];
            Assert.Equal(3, classInfo.ConstructorParameters.Count);
            Assert.True(classInfo.HasInterfaceDependencies);
        }

        [Fact]
        public async Task ShouldExtractUsingDirectives() {
            var parser = new CodeParser();
            var sourceCode = @"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using MyProject.Models;
                using MyProject.Services;
                
                namespace MyNamespace
                {
                    public class MyClass
                    {
                        public void Process() { }
                    }
                }";

            var result = await parser.ParseAsync(sourceCode);

            Assert.Equal(5, result.Usings.Count);
            Assert.Contains("System", result.Usings);
            Assert.Contains("System.Collections.Generic", result.Usings);
            Assert.Contains("System.Linq", result.Usings);
            Assert.Contains("MyProject.Models", result.Usings);
            Assert.Contains("MyProject.Services", result.Usings);
        }
    }
}