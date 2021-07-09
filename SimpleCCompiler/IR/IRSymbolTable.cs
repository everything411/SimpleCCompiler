using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR
{
    public class IRSymbolTable
    {
        public List<Function> FunctionList { get; set; } // default == null
        public Dictionary<Guid, Variable> VariableDictionary { get; set; } = new();
    }
}
