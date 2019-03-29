using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Pegasus;
using Pegasus.Common;

namespace PegasusExample
{
    class Program
    {
        private const string PegString = @"
additive <double> -memoize
    = left:additive ""+"" right:multiplicative { left + right }
    / left:additive ""-"" right:multiplicative { left - right }
    / multiplicative

multiplicative <double> -memoize
    = left:multiplicative ""*"" right:power { left * right }
    / left:multiplicative ""/"" right:power { left / right }
    / power

power <double>
    = left:primary ""^"" right:power { Math.Pow(left, right) }
    / primary

primary <double> -memoize
    = decimal
    / ""-"" primary:primary { -primary }
    / ""("" additive:additive "")"" { additive }

decimal <double>
    = value:([0-9]+ ("","" [0-9]+)?) { double.Parse(value) }";

        static void Main(string[] args)
        {

            var compileResult = CompileManager.CompileString(PegString);

            var dotnetCoreDirectory = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            var compilation = CSharpCompilation.Create("LibraryName")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(GeneratedCodeAttribute).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(MulticastDelegate).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IList<object>).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Cursor).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "netstandard.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Runtime.dll")))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(compileResult.Code));

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);

                if (!emitResult.Success)
                {
                    // Debug output. In case your environment is different it may show some messages.
                    foreach (var compilerMessage in compilation.GetDiagnostics())
                        Console.WriteLine(compilerMessage);
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    var context = AssemblyLoadContext.Default;
                    var assembly = context.LoadFromStream(ms);

                    var parserType = assembly.GetType("Parsers.Parser");

                    object parser = Activator.CreateInstance(parserType);

                    var result = parserType.GetMethod("Parse").Invoke(parser, new object[] { "5,7+8,9*42", null });

                    Console.WriteLine(result);
                }
            }
        }
    }
}
