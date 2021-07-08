using SimpleCCompiler.IR.Instrunction;
using System.Collections.Generic;

namespace SimpleCCompiler.IR
{
    public interface IIRGenerator
    {
        public IList<IInstruction> EmitIR();
    }
}
