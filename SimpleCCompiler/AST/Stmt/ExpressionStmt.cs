
using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System.Collections.Generic;

namespace SimpleCCompiler.AST
{
    public class ExpressionStmt : Stmt
    {
        public Expr Expr { get; set; }
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            return Expr.GenerateIR(parentFunction);
        }
    }
}
