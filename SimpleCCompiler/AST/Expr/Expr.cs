using System;
using System.Collections.Generic;
using SimpleCCompiler.IR;

namespace SimpleCCompiler.AST
{
    public class Expr : Node
    {
        public Variable ResultVariable { get; set; }
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
    }
}
