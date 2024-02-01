using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace ApacheOrcDemo
{
    public class RoslynLogic
    {
        public static Type CreateClass(List<Field> fields, string newClassName, string newNamespace = "ApacheOrcDemo")
        {
            var fieldsCode = fields
                                .Select(field => $"public {field.FieldType} {field.FieldName} {{get; set;}}")
                                .ToString(Environment.NewLine);

            var classCode = $@"
                using System;

                namespace {newNamespace}
                {{
                    public class {newClassName}
                    {{
                        public {newClassName}()
                        {{
                        }}

                        {fieldsCode}
                    }}
                }}
            ".Trim();

            classCode = FormatUsingRoslyn(classCode);


            var assemblies = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            };

            var syntaxTree = CSharpSyntaxTree.ParseText(classCode);

            var compilation = CSharpCompilation
                                .Create(newNamespace)
                                .AddSyntaxTrees(syntaxTree)
                                .AddReferences(assemblies)
                                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());

                    var newTypeFullName = $"{newNamespace}.{newClassName}";

                    var type = assembly.GetType(newTypeFullName);
                    return type;
                }
                else
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return null;
                }
            }
        }

        public static string FormatUsingRoslyn(string csCode)
        {
            var tree = CSharpSyntaxTree.ParseText(csCode);
            var root = tree.GetRoot().NormalizeWhitespace();
            var result = root.ToFullString();
            return result;
        }
    }

    public class Field
    {
        public string FieldName;
        public string FieldType;

        public Field(string fieldName, string fieldType)
        {
            FieldName = fieldName;
            FieldType = fieldType;
        }
    }

    public static class Extensions
    {
        public static string ToString(this IEnumerable<string> list, string separator)
        {
            string result = string.Join(separator, list);
            return result;
        }
    }
}
