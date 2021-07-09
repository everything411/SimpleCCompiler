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
        public LoadArrayInstruction(ArrayIndexVariable arg1, Variable result)
        {
            Result = result;
            Argument1 = arg1;
        }
        public override string EmitAssembly()
        {
            var arrayIndexVar = Argument1 as ArrayIndexVariable;
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov ecx, [ebp + {arrayIndexVar.Index.OffsetEBP}]");
            stringBuilder.AppendLine($"lea eax, [ebp + {arrayIndexVar.OffsetEBP} + ecx * 4]");
            stringBuilder.AppendLine($"mov ecx, [eax]");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], ecx");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(LoadArrayInstruction, {Argument1}, , {Result})";
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
        public override string EmitAssembly()
        {
            return $"mov dword ptr [ebp + {Result.OffsetEBP}], offset {Literal.LiteralName}";
        }
        public override string ToString()
        {
            return $"(LoadStringLiternalInstruction, {Literal.LiteralName}, , {Result})";
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
        public override string EmitAssembly()
        {
            return $"mov dword ptr [ebp + {Result.OffsetEBP}], {Literal}";
        }
        public override string ToString()
        {
            return $"(LoadIntLiternalInstruction, {Literal}, , {Result})";
        }
    }
}
