using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR.Instrunction
{
    public class CallInstrunction : Instruction
    {
        // public IR.Type FunctionType { get; set; } = Type.Void;
        // public Function CalledFunction { get; set; }
        public string FunctionName { get; set; }
        public IEnumerable<Variable> Arguments { get; set; }
        // public 
        public CallInstrunction(String functionName, IEnumerable<Variable> args, Variable result)
        {
            Operation = Operation.Call;
            FunctionName = functionName;
            Arguments = args;
            Result = result;
        }
    }
    public class ReturnInstrunction : Instruction
    {
        // public IR.Type ReturnType { get; set; } = Type.Void;
        public ReturnInstrunction(Variable arg)
        {
            Operation = Operation.Return;
            Argument1 = arg;
        }
    }
}
