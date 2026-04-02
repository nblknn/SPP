using Core.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core {
    public class CodeParser {
        public async Task<ParseResult> ParseAsync(string sourceCode) {
            var result = new ParseResult();

            var tree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = await tree.GetRootAsync();

            var compilationUnit = root as CompilationUnitSyntax;
            if (compilationUnit != null) {
                foreach (var usingDirective in compilationUnit.Usings) {
                    result.Usings.Add(usingDirective.Name.ToString());
                }
            }

            var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            var fileScopedNamespaces = root.DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>();

            foreach (var namespaceDecl in namespaceDeclarations.Concat<BaseNamespaceDeclarationSyntax>(fileScopedNamespaces)) {
                var classDeclarations = namespaceDecl.DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classDecl in classDeclarations) {
                    var classInfo = ParseClass(classDecl, namespaceDecl.Name.ToString());
                    result.Classes.Add(classInfo);
                }
            }

            var classesWithoutNamespace = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(c => c.Parent is CompilationUnitSyntax);

            foreach (var classDecl in classesWithoutNamespace) {
                var classInfo = ParseClass(classDecl, null);
                result.Classes.Add(classInfo);
            }

            return result;
        }

        private ClassInfo ParseClass(ClassDeclarationSyntax classDecl, string namespaceName) {
            var classInfo = new ClassInfo {
                Namespace = namespaceName,
                Name = classDecl.Identifier.Text,
                ConstructorParameters = new List<ConstructorParameterInfo>(),
                PublicMethods = new List<MethodInfo>()
            };

            var methods = classDecl.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(SyntaxKind.PublicKeyword) && !m.Modifiers.Any(SyntaxKind.StaticKeyword))
                .ToList();

            foreach (var method in methods) {
                var methodInfo = new MethodInfo {
                    Name = method.Identifier.Text,
                    ReturnType = method.ReturnType.ToString()
                };

                var parameters = method.ParameterList.Parameters;
                foreach (var param in parameters) {
                    var paramInfo = new ParameterInfo {
                        Name = param.Identifier.Text,
                        Type = param.Type.ToString()
                    };
                    methodInfo.Parameters.Add(paramInfo);
                }

                classInfo.PublicMethods.Add(methodInfo);
            }

            var constructors = classDecl.DescendantNodes().OfType<ConstructorDeclarationSyntax>();

            var publicConstructor = constructors.FirstOrDefault(c => c.Modifiers.Any(SyntaxKind.PublicKeyword));

            if (publicConstructor != null) {
                foreach (var param in publicConstructor.ParameterList.Parameters) {
                    var paramType = param.Type.ToString();
                    var isInterface = paramType.StartsWith("I") && paramType.Length > 1 && char.IsUpper(paramType[1]);

                    classInfo.ConstructorParameters.Add(new ConstructorParameterInfo {
                        Name = param.Identifier.Text,
                        Type = paramType,
                        IsInterface = isInterface
                    });
                }

                classInfo.HasInterfaceDependencies = classInfo.ConstructorParameters.Any(p => p.IsInterface);
            }

            return classInfo;
        }
    }
}