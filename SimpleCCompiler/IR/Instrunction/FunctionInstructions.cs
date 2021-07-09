using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR.Instrunction
{
    public class CallInstrunction : Instruction
    {
        public string FunctionName { get; set; }
        public List<Variable> Arguments { get; set; }
        public CallInstrunction(string functionName, List<Variable> args, Variable result)
        {
            Operation = Operation.Call;
            FunctionName = functionName;
            Arguments = args;
            Result = result;
        }
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            for (int i = Arguments.Count - 1; i >= 0; i--)
            {
                stringBuilder.AppendLine($"push [ebp + {Arguments[i].OffsetEBP}]");
            } 
            stringBuilder.AppendLine($"call {FunctionName}");
            stringBuilder.AppendLine($"add esp, {Arguments.Count * 4}");
            stringBuilder.AppendLine($"mov [ebp + {Result.OffsetEBP}], eax");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(CallInstruction, {FunctionName}, ArgList, {Result})";
        }
    }
    public class ReturnInstrunction : Instruction
    {
        // public IR.Type ReturnType { get; set; } = Type.Void;
        // public int FrameSize { get; set; }
        public ReturnInstrunction(Variable arg)
        {
            Operation = Operation.Return;
            Argument1 = arg;
        }
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"mov eax, [ebp + {Argument1.OffsetEBP}]");
            stringBuilder.AppendLine($"mov esp, ebp");
            stringBuilder.AppendLine($"pop ebp");
            stringBuilder.AppendLine("ret");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(ReturnInstruction, {Argument1}, , {Result})";
        }
    }
    public class AllocateFrameInstrunction : Instruction
    {
        public int FrameSize { get; set; }
        public override string EmitAssembly()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("push ebp");
            stringBuilder.AppendLine("mov ebp, esp");
            stringBuilder.AppendLine($"sub esp, {FrameSize}");
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"(AllocateFrameInstruction, {FrameSize}, , )";
        }
    }
}
