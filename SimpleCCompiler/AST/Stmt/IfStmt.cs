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
        public IStmt IfTrueBody { get; set; }
        public IStmt IfFalseBody { get; set; } = new EmptyStmt();
        public LabelInstruction IfEndLabel { get; set; } = new();
        public LabelInstruction IfFalseLabel { get; set; } = new();
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
        public override IEnumerable<SymbolTableItem> CollectSymbolTableItems()
        {
            return IfTrueBody.CollectSymbolTableItems();
        }
    }
}
