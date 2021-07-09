using SimpleCCompiler.AST.Expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR.Instrunction
{
    public class ArithmeticalInstruction : Instruction
    {
        public ArithmeticalInstruction(Variable argument1, Variable argument2, Variable result)
        {
            Argument1 = argument1;
            Argument2 = argument2;
            Result = result;
        }
    }
    public class AddInstruction : ArithmeticalInstruction
    {
        public AddInstruction(Variable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
            Operation = Operation.Add;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            // argument1;
            stringBuilder.Append("mov ");
            return stringBuilder.ToString();
        }
    }
    public class SubInstruction : ArithmeticalInstruction
    {
        public SubInstruction(Variable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
            Operation = Operation.Sub;
        }
    }
    public class CmpInstruction : ArithmeticalInstruction
    {
        public BinaryOperator BinaryOperator { get; set; }
        public CmpInstruction(Variable argument1, Variable argument2, Variable result, BinaryOperator o)
            : base(argument1, argument2, result)
        {
            BinaryOperator = o;
            Operation = Operation.Cmp;
        }
    }
    public class MulInstruction : ArithmeticalInstruction
    {
        public MulInstruction(Variable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
            Operation = Operation.Mul;
        }
    }
    public class DivInstruction : ArithmeticalInstruction
    {
        public DivInstruction(Variable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
            Operation = Operation.Div;
        }
    }
    public class ModInstruction : ArithmeticalInstruction
    {
        public ModInstruction(Variable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
            Operation = Operation.Mod;
        }
    }
}
