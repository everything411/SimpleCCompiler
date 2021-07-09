using SimpleCCompiler.AST.Stmt;
using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;

namespace SimpleCCompiler.AST.Decl
{
    public class FunctionDecl : ValueDecl
    {
        public List<ParmVarDecl> ParmVarDeclList { get; set; } = new();
        public SymbolTable SymbolTable { get; set; } = new();
        public CompoundStmt Body { get; set; }
        int parameterCount = 0;
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            if (SymbolTable.TryGetValue(name, out SymbolTableItem item))
            {
                return item;
            }
            else
            {
                return Parent.LookupSymbolTable(name);
            }
        }
        public override void AddDeclaration(IDecl decl)
        {
            switch (decl)
            {
                case ParmVarDecl varDecl:
                    if (varDecl.Parent is null)
                    {
                        varDecl.Parent = this;
                    }
                    else if (varDecl.Parent != this)
                    {
                        throw new InternalErrorException("Different parent detacted");
                    }
                    ParmVarDeclList.Add(varDecl);
                    SymbolTable.AddParameterOrException(varDecl.Name, varDecl.Type, ++parameterCount);
                    break;
                default:
                    throw new SemanticErrorException($"Unexpected token {decl}, expected ParmVarDecl");
            }
        }

    }
}
