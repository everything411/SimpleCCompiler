using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCCompiler.AST.Expr;
using SimpleCCompiler.IR.Instrunction;

namespace SimpleCCompiler.AST.Stmt
{
    public class ExpressionStmt : Stmt
    {
        public IExpr Expr { get; set; }
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
    }
}
