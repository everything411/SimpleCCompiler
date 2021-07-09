using System;
using System.Collections.Generic;
using SimpleCCompiler.IR;

namespace SimpleCCompiler.AST.Expr
{
    public class Expr : IExpr
    {
        public INode Parent { get; set; }
        public Variable ResultVariable { get; set; }

        public void AddDeclaration(IDecl decl)
        {
            throw new NotImplementedException();
        }

        public virtual SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
        public virtual IEnumerable<SymbolTableItem> CollectSymbolTableItems()
        {
            throw new NotImplementedException();
        }
    }
}
