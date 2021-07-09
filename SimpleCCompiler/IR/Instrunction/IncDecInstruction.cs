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
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument1.OffsetEBP}]");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], eax");
            stringBuilder.AppendLine($"inc dword ptr [ebp + {Argument1.OffsetEBP}]");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(IncInstruction, {Argument1}, , {Result})";
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
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument1.OffsetEBP}]");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], eax");
            stringBuilder.AppendLine($"dec dword ptr [ebp + {Argument1.OffsetEBP}]");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(DecInstruction, {Argument1}, , {Result})";
        }
    }
}
