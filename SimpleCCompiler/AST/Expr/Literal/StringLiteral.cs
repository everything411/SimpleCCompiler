using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class StringLiteral : Literal
    {
        public string Value { get; set; }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
            ResultVariable = resultTempVar;
            IRStringLiteral iRStringLiteral = new(this);
            parentFunction.StringLiterals.Add(iRStringLiteral);
            LoadStringLiternalInstruction instruction = new(resultTempVar, iRStringLiteral);
            irList.Add(instruction);
            return irList;
        }
    }
}
