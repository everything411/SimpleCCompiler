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
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument2.OffsetEBP}]");
            stringBuilder.AppendLine($"mov [ebp + {Argument1.OffsetEBP}], eax");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], eax");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(AssignmentInstruction, {Argument1}, {Argument2}, {Result})";
        }
    }
    public class ArrayAssignmentInstruction : AssignmentInstruction
    {
        public ArrayAssignmentInstruction(ArrayIndexVariable argument1, Variable argument2, Variable result)
            : base(argument1, argument2, result)
        {
        }
        public override string EmitAssembly()
        {
            var arrayIndexVar = Argument1 as ArrayIndexVariable;
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov ecx, [ebp + {arrayIndexVar.Index.OffsetEBP}]");
            stringBuilder.AppendLine($"lea eax, [ebp + {arrayIndexVar.OffsetEBP} + ecx * 4]");
            stringBuilder.AppendLine($"mov edx, [ebp + {Argument2.OffsetEBP}]");
            stringBuilder.AppendLine($"mov [eax], edx");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], edx");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(AssignmentInstruction, {Argument1}, {Argument2}, {Result})";
        }
    }
}
