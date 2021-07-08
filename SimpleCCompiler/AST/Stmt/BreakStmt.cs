using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Stmt
{
    public class BreakStmt : Stmt
    {
        public ForStmt ParentIterationStmt { get; set; }
        public BreakStmt(Stmt parent)
        {
            ParentIterationStmt = parent.GetIterationStmtOrException();
        }
        public override IList<IInstruction> EmitIR()
        {
            return new List<IInstruction>(0);
        }
    }
}
