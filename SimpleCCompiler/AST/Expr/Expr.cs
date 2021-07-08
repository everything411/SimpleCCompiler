using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Expr
{
    public class Expr : IExpr
    {
        public INode Parent { get; set; }

        public void AddDeclaration(IDecl decl)
        {
            throw new NotImplementedException();
        }

        public virtual SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
    }
}
