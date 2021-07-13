using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SimpleCCompiler.Parser;
using System;
using System.Text.Json;
using SimpleCCompiler.IR;
using System.Collections.Generic;
using SimpleCCompiler.CodeGeneration;
using System.IO;
using System.Diagnostics;

namespace SimpleCCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("usage: naivecc file");
                return;
            }
            Console.WriteLine("Naive C Compiler by everything411. Too young, too simple.");
            var s = Guid.NewGuid().ToString("N").Substring(0, 8);
            var irFileName = s + ".ir";
            var asmFileName = s + ".asm";
            var objFileName = s + ".obj";
            var exeFile = s + ".exe";
            AntlrFileStream stream = null;
            try
            {
                stream = new(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
            Console.WriteLine("Run lexer and parser");
            CLexer lexer = new(stream);
            CommonTokenStream commonTokenStream = new(lexer);
            CParser parser = new(commonTokenStream);
            var tree = parser.compilationUnit();
            Console.WriteLine("Complete");
            Console.WriteLine("Run semantic, generate AST and do semantic check");
            var walker = new ParseTreeWalker();
            var listener = new SimpleCASTListener();
            try
            {
                walker.Walk(listener, tree);
            }
            catch (Exception e)
            {
                Console.WriteLine("!! Error !!");
                Console.WriteLine("=== Nodes in NodeStack ===");
                var nodeStack = listener.GetNodeStackDebug();
                foreach (var item in nodeStack)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("=== Exception ===");
                Console.WriteLine(e);
                Environment.Exit(1);
            }
            var astRoot = listener.GetASTRoot();
            // Console.WriteLine(JsonSerializer.Serialize(astRoot, new JsonSerializerOptions { WriteIndented = true}));
            Console.WriteLine("Complete"); 
            Console.WriteLine("Run IR generation");
            Module module = null;
            try
            {
                module = IRGenerator.GenerateIRForTranslationUnit(astRoot);
            }
            catch (Exception e)
            {
                Console.WriteLine("=== Exception ===");
                Console.WriteLine(e);
                Environment.Exit(1);
            }
            module.GenerateIRFile(irFileName);
            Console.WriteLine("Complete");
            Console.WriteLine("Run code generation");
            Console.WriteLine(s);
            CodeGenerator.GenerateAssemblyFile(asmFileName, module);
            Console.WriteLine("Complete");
            Console.WriteLine("Run ml & link");
            Process p = new();
            p.StartInfo.FileName = "bin/ml.exe";
            p.StartInfo.ArgumentList.Add("/c");
            p.StartInfo.ArgumentList.Add("/nologo");
            p.StartInfo.ArgumentList.Add("/coff");
            p.StartInfo.ArgumentList.Add(asmFileName);
            p.StartInfo.EnvironmentVariables.Add("include", "include");
            p.Start();
            p.WaitForExit();
            p = new();
            p.StartInfo.FileName = "bin/link.exe";
            p.StartInfo.ArgumentList.Add("/subsystem:console");
            p.StartInfo.ArgumentList.Add("/nologo");
            p.StartInfo.ArgumentList.Add(objFileName);
            p.StartInfo.EnvironmentVariables.Add("lib", "lib");
            p.Start();
            p.WaitForExit();
            File.Delete(objFileName);
            Console.WriteLine("Complete");
        }
    }
}
