using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using SimpleCCompiler.IR;

namespace SimpleCCompiler.AST.Decl
{
    public class TranslationUnitDecl : Decl
    {
        public SymbolTable SymbolTable { get; set; } = new();
        // for json serialize change to ValueDecl
        public List<FunctionDecl> FunctionDeclList { get; set; } = new();
        public List<VarDecl> VarDeclList { get; set; } = new();
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            if (SymbolTable.TryGetValue(name, out SymbolTableItem item))
            {
                return item;
            }
            else
            {
                throw new SemanticErrorException($"Unknown symbol {name}");
            }
        }
        public override void AddDeclaration(IDecl decl)
        {
            switch (decl)
            {
                case VarDecl varDecl:
                    if (varDecl.Parent is null)
                    {
                        varDecl.Parent = this;
                    }
                    else if (varDecl.Parent != this)
                    {
                        throw new InternalErrorException("Different parent detacted");
                    }
                    VarDeclList.Add(varDecl);
                    SymbolTable.AddOrException(varDecl.Name, varDecl.Type);
                    break;
                case FunctionDecl varDecl:
                    if (varDecl.Parent is null)
                    {
                        varDecl.Parent = this;
                    }
                    else if (varDecl.Parent != this)
                    {
                        throw new InternalErrorException("Different parent detacted");
                    }
                    FunctionDeclList.Add(varDecl);
                    SymbolTable.AddOrException(varDecl.Name, varDecl.Type);
                    break;
                default:
                    throw new SemanticErrorException($"Unexpected token {decl}, expected VarDecl");
            }
        }
    }
}
