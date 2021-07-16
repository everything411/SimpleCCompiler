using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class BreakStmt : Stmt
    {
        public ForStmt ParentIterationStmt { get; set; }
        public BreakStmt(Stmt parent)
        {
            ParentIterationStmt = parent.GetIterationStmtOrException();
        }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            irList.Add(new JmpInstruction(ParentIterationStmt.EndLabel));
            return irList;
        }
    }
}
