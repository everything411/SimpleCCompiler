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
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument1.OffsetEBP}]");
            stringBuilder.AppendLine($"mov ecx, [ebp + {Argument2.OffsetEBP}]");
            stringBuilder.AppendLine($"add eax, ecx");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], eax");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(AddInstruction, {Argument1}, {Argument2}, {Result})";
        }
    }
    public class SubInstruction : ArithmeticalInstruction
    {
        public SubInstruction(Variable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
            Operation = Operation.Sub;
        }
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument1.OffsetEBP}]");
            stringBuilder.AppendLine($"mov ecx, [ebp + {Argument2.OffsetEBP}]");
            stringBuilder.AppendLine($"sub eax, ecx");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], eax");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(SubInstruction, {Argument1}, {Argument2}, {Result})";
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
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument1.OffsetEBP}]");
            stringBuilder.AppendLine($"mov ecx, [ebp + {Argument2.OffsetEBP}]");
            stringBuilder.AppendLine($"cmp eax, ecx");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(CmpInstruction, {Argument1}, {Argument2}, {Result})";
        }
    }
    public class MulInstruction : ArithmeticalInstruction
    {
        public MulInstruction(Variable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
            Operation = Operation.Mul;
        }
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument1.OffsetEBP}]");
            stringBuilder.AppendLine($"mov ecx, [ebp + {Argument2.OffsetEBP}]");
            stringBuilder.AppendLine($"imul eax, ecx");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], eax");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(MulInstruction, {Argument1}, {Argument2}, {Result})";
        }
    }
    public class DivInstruction : ArithmeticalInstruction
    {
        public DivInstruction(Variable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
            Operation = Operation.Div;
        }
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument1.OffsetEBP}]");
            stringBuilder.AppendLine($"cdq");
            stringBuilder.AppendLine($"div word ptr [ebp + {Argument2.OffsetEBP}]");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], eax");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(DivInstruction, {Argument1}, {Argument2}, {Result})";
        }
    }
    public class ModInstruction : ArithmeticalInstruction
    {
        public ModInstruction(Variable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
            Operation = Operation.Mod;
        }
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument1.OffsetEBP}]");
            stringBuilder.AppendLine($"cdq");
            stringBuilder.AppendLine($"div word ptr [ebp + {Argument2.OffsetEBP}]");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], edx");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(ModInstruction, {Argument1}, {Argument2}, {Result})";
        }
    }
}
