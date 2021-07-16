using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR
{
    public class Module
    {
        public List<Variable> GlobalVariables { get; set; }
        public List<Function> Functions { get; set; } = new();
        // public List<IRStringLiteral> StringLiterals { get; set; } = new();
        public IRSymbolTable IRSymbolTable { get; set; }
        public List<Instruction> Instructions { get; set; } = new();
        public void GenerateIRFile(string filename)
        {
            StringBuilder stringBuilder = new();
            foreach (var item in Instructions)
            {
                stringBuilder.AppendLine(item.ToString());
            }
            File.WriteAllText(filename, stringBuilder.ToString());
        }
    }
}
