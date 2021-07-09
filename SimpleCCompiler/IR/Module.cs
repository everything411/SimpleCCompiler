using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR
{
    public class Module : IIR
    {
        public List<Variable> GlobalVariables { get; set; }
        public List<Function> Functions { get; set; } = new();
        public IRSymbolTable IRSymbolTable { get; set; }
    }
}
