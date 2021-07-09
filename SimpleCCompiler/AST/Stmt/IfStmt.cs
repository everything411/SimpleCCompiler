using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Stmt
{
    public class IfStmt : Stmt
    {
        public IExpr ConditionalExpr { get; set; }
        public IStmt BodyStmt { get; set; }
        public LabelInstruction IfTrueLabel { get; set; } = new();
        public LabelInstruction IfFalseLabel { get; set; } = new();
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
    }
}
