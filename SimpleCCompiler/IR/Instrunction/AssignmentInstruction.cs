using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR.Instrunction
{
    public class AssignmentInstruction : Instruction
    {
        public AssignmentInstruction(Variable argument1, Variable argument2, Variable result)
        {
            Operation = Operation.Mov;
            Argument1 = argument1;
            Argument2 = argument2;
            Result = result;
        }
    }
    public class ArrayAssignmentInstruction : AssignmentInstruction
    {
        public ArrayAssignmentInstruction(ArrayIndexVariable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
        }
    }
}
