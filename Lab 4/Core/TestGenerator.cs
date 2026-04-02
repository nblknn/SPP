using Core.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Core {
    public class TestGenerator {

        public string Generate(ClassInfo classInfo, List<string> originalUsings) {
            var compilationUnit = CompilationUnit();

            var usings = new List<UsingDirectiveSyntax>() {
                UsingDirective(ParseName("System")), UsingDirective(ParseName("Xunit")) };

            if (classInfo.HasInterfaceDependencies) {
                usings.Add(UsingDirective(ParseName("Moq")));
            }

            if (!string.IsNullOrEmpty(classInfo.Namespace)) {
                usings.Add(UsingDirective(ParseName(classInfo.Namespace)));
            }

            var uniqueUsings = originalUsings.Where(u => u != "System").Distinct();

            foreach (var usingNamespace in uniqueUsings) {
                usings.Add(UsingDirective(ParseName(usingNamespace)));
            }

            compilationUnit = compilationUnit.AddUsings(usings.ToArray());

            var testNamespace = classInfo.Namespace != null ? $"{classInfo.Namespace}.Tests" : "Tests";

            var namespaceDeclaration = NamespaceDeclaration(ParseName(testNamespace));

            var testClass = CreateTestClass(classInfo);
            namespaceDeclaration = namespaceDeclaration.AddMembers(testClass);

            compilationUnit = compilationUnit.AddMembers(namespaceDeclaration);

            compilationUnit = compilationUnit.NormalizeWhitespace();

            return compilationUnit.ToFullString();
        }

        private ClassDeclarationSyntax CreateTestClass(ClassInfo classInfo) {
            var className = $"{classInfo.Name}Tests";
            var testClass = ClassDeclaration(className).AddModifiers(Token(SyntaxKind.PublicKeyword));

            var classUnderTestField = FieldDeclaration(
                VariableDeclaration(ParseTypeName(classInfo.Name))
                .AddVariables(VariableDeclarator($"_{classInfo.Name.ToLower()}UnderTest")))
                .AddModifiers(Token(SyntaxKind.PrivateKeyword));
            testClass = testClass.AddMembers(classUnderTestField);

            foreach (var param in classInfo.ConstructorParameters.Where(p => p.IsInterface)) {
                var mockField = FieldDeclaration(
                    VariableDeclaration(GenericName(Identifier("Mock"))
                    .AddTypeArgumentListArguments(ParseTypeName(param.Type)))
                    .AddVariables(VariableDeclarator($"_{param.Name}")))
                    .AddModifiers(Token(SyntaxKind.PrivateKeyword));
                testClass = testClass.AddMembers(mockField);
            }

            var constructor = CreateConstructor(classInfo);
            testClass = testClass.AddMembers(constructor);

            var methodGroups = classInfo.PublicMethods.GroupBy(m => m.Name).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var methodGroup in methodGroups) {
                var methods = methodGroup.Value;

                if (methods.Count == 1) {
                    var testMethod = CreateTestMethod(methods[0], classInfo, null);
                    testClass = testClass.AddMembers(testMethod);
                }
                else {
                    for (int i = 0; i < methods.Count; i++) {
                        var overloadNumber = i + 1;
                        var testMethod = CreateTestMethod(methods[i], classInfo, overloadNumber);
                        testClass = testClass.AddMembers(testMethod);
                    }
                }
            }

            return testClass;
        }

        private ConstructorDeclarationSyntax CreateConstructor(ClassInfo classInfo) {
            var constructor = ConstructorDeclaration($"{classInfo.Name}Tests")
                .AddModifiers(Token(SyntaxKind.PublicKeyword));

            var statements = new List<StatementSyntax>();

            if (classInfo.HasInterfaceDependencies)
                foreach (var param in classInfo.ConstructorParameters.Where(p => p.IsInterface)) {
                    var mockCreation = ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName($"_{param.Name}"),
                            ObjectCreationExpression(
                                GenericName(Identifier("Mock"))
                                .AddTypeArgumentListArguments(ParseTypeName(param.Type)))
                            .WithArgumentList(ArgumentList())));
                    statements.Add(mockCreation);
                }

            var arguments = new List<ArgumentSyntax>();
            foreach (var param in classInfo.ConstructorParameters) {
                if (param.IsInterface) {
                    arguments.Add(Argument(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName($"_{param.Name}"),
                            IdentifierName("Object"))));
                }
                else {
                    arguments.Add(Argument(
                        LiteralExpression(
                            SyntaxKind.DefaultLiteralExpression,
                            Token(SyntaxKind.DefaultKeyword))));
                }
            }

            var instanceCreation = ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName($"_{classInfo.Name.ToLower()}UnderTest"),
                    ObjectCreationExpression(ParseTypeName(classInfo.Name))
                    .WithArgumentList(ArgumentList(SeparatedList(arguments)))));
            statements.Add(instanceCreation);

            constructor = constructor.AddBodyStatements(statements.ToArray());

            return constructor;
        }

        private MethodDeclarationSyntax CreateTestMethod(MethodInfo method, ClassInfo classInfo, int? overloadNumber) {
            string testMethodName;
            if (overloadNumber.HasValue) {
                testMethodName = $"{method.Name}{overloadNumber.Value}Test";
            }
            else {
                testMethodName = $"{method.Name}Test";
            }

            var testMethod = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), testMethodName)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Fact")))));

            var parameterTypes = string.Join(", ", method.Parameters.Select(p => p.Type));

            var statements = new List<StatementSyntax>();

            // Arrange

            foreach (var param in method.Parameters) {
                var defaultValue = LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword));
                var declaration = LocalDeclarationStatement(
                    VariableDeclaration(ParseTypeName(param.Type))
                    .AddVariables(VariableDeclarator(param.Name)
                    .WithInitializer(EqualsValueClause(defaultValue))));
                statements.Add(declaration);
            }

            // Act

            var arguments = method.Parameters.Select(p => Argument(IdentifierName(p.Name))).ToArray();

            var methodInvocation = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName($"_{classInfo.Name.ToLower()}UnderTest"),
                    IdentifierName(method.Name)))
                .WithArgumentList(ArgumentList(SeparatedList(arguments)));

            if (!method.IsVoid) {
                var variableDeclaration = LocalDeclarationStatement(
                    VariableDeclaration(ParseTypeName(method.ReturnType))
                    .AddVariables(VariableDeclarator("actual")
                        .WithInitializer(EqualsValueClause(methodInvocation))));
                statements.Add(variableDeclaration);
            }
            else {
                statements.Add(ExpressionStatement(methodInvocation));
            }

            // Assert

            if (!method.IsVoid) {
                var expectedValue = LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword));
                var expectedDeclaration = LocalDeclarationStatement(
                    VariableDeclaration(ParseTypeName(method.ReturnType))
                    .AddVariables(VariableDeclarator("expected")
                        .WithInitializer(EqualsValueClause(expectedValue))));
                statements.Add(expectedDeclaration);

                var assertion = ExpressionStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("Assert"),
                            IdentifierName("Equal")))
                    .WithArgumentList(ArgumentList(
                        SeparatedList(new[]
                        {
                            Argument(IdentifierName("expected")),
                            Argument(IdentifierName("actual"))
                        }))));
                statements.Add(assertion);
            }

            var failAssertion = ExpressionStatement(
                InvocationExpression(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Assert"), IdentifierName("Fail")))
                .WithArgumentList(ArgumentList(SingletonSeparatedList(
                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("autogenerated")))))));

            statements.Add(failAssertion);

            testMethod = testMethod.AddBodyStatements(statements.ToArray());

            return testMethod;
        }
    }
}