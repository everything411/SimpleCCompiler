using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class EmptyExpr : Expr
    {
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new(0);
            return irList;
        }
    }
}
