using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR.Instrunction
{
    public class LoadInstruction : Instruction
    {
    }
    public class LoadArrayInstruction : LoadInstruction
    {
        public LoadArrayInstruction(Variable arg1, Variable result)
        {
            Result = result;
            Argument1 = arg1;
        }
    }
    public class LoadStringLiternalInstruction : LoadInstruction
    {
        public IRStringLiteral Literal { get; set; }
        public LoadStringLiternalInstruction(Variable result, IRStringLiteral s)
        {
            Result = result;
            Literal = s;
        }
    }
    public class LoadIntLiternalInstruction : LoadInstruction
    {
        public int Literal { get; set; }
        public LoadIntLiternalInstruction(Variable result, int s)
        {
            Result = result;
            Literal = s;
        }
    }
}
