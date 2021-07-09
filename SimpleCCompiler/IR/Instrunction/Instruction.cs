using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR.Instrunction
{
    public class Instruction : IInstruction
    {
        public Operation Operation { get; set; }
        public Variable Argument1 { get; set; }
        public Variable Argument2 { get; set; }
        public Variable Result { get; set; }

        public virtual string EmitAssembly()
        {
            throw new NotImplementedException();
        }
    }
}
