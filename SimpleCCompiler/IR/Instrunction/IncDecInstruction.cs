using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR.Instrunction
{
    public class IncInstruction : Instruction
    {
        public IncInstruction(Variable argument1, Variable result)
        {
            Operation = Operation.Inc;
            Argument1 = argument1;
            Result = result;
        }
    }
    public class DecInstruction : Instruction
    {
        public DecInstruction(Variable argument1, Variable result)
        {
            Operation = Operation.Dec;
            Argument1 = argument1;
            Result = result;
        }
    }
}
