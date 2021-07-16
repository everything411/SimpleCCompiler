using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;

namespace SimpleCCompiler.AST
{
    public abstract class Node
    {
        public Node Parent { get; set; }
        public virtual void AddDeclaration(Decl decl)
        {
            throw new NotImplementedException();
        }
        public virtual SymbolTableItem LookupSymbolTable(string name)
        {
            throw new NotImplementedException();
        }
        public virtual IList<SymbolTableItem> CollectSymbolTableItems()
        {
            throw new NotImplementedException();
        }
        public virtual IList<Instruction> GenerateIR(Function parentFunction)
        {
            throw new NotImplementedException();
        }
    }
}
