using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCCompiler.AST.Decl;
using SimpleCCompiler.IR.Instrunction;

namespace SimpleCCompiler.AST.Stmt
{
    public class ReturnStmt : Stmt
    {
        public IExpr ReturnValueExpr { get; set; }
        public FunctionDecl FunctionReturnedFrom { get; set; }
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
    }
}
