using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCCompiler.AST.Expr.Reference;

namespace SimpleCCompiler.AST.Expr
{
    public class DeclRefExpr : Expr
    {
        public SymbolTableItem Ref { get; set; }
        public override string ToString() => $"{base.ToString()}:{Ref}";
        // public IReference Reference { get; set; }
    }
}
