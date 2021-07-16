using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class IntegerLiteral : Literal
    {
        public int Value { get; set; }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
            ResultVariable = resultTempVar;
            LoadIntLiternalInstruction instruction = new(resultTempVar, Value);
            irList.Add(instruction);
            return irList;
        }
    }
}
