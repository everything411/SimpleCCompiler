using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SimpleCCompiler.Parser;
using System;
using System.Text.Json;

namespace SimpleCCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("usage: ccompiler input");
                return;
            }
            Console.WriteLine("Naive C Compiler by everything411. Too young, too simple.");
            
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
            Console.WriteLine(JsonSerializer.Serialize(astRoot, new JsonSerializerOptions { WriteIndented = true}));
            Console.WriteLine("Complete"); 
            Console.WriteLine("Run IR generation");
            Console.WriteLine("Complete");
            Console.WriteLine("Run code generation");
            Console.WriteLine("Complete");
            //Console.WriteLine("Press Enter to exit...");
            //Console.ReadLine();
        }
    }
}
