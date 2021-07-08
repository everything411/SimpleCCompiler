using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Stmt
{
    public class EmptyStmt : Stmt
    {
        public override IList<IInstruction> EmitIR()
        {
            return new List<IInstruction>(0);
        }
    }
}
