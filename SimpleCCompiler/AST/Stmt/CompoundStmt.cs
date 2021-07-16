using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System.Collections.Generic;

namespace SimpleCCompiler.AST
{
    public class CompoundStmt : Stmt
    {
        public SymbolTable SymbolTable { get; set; } = new();
        public List<VarDecl> Decls { get; set; } = new();
        // TODO: hack
        // public List<object> Stmts { get; set; } = new();
        public List<Stmt> Stmts { get; set; } = new();
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            SymbolTableItem item;
            if (SymbolTable.TryGetValue(name, out item))
            {
                return item;
            }
            else
            {
                return Parent.LookupSymbolTable(name);
            }
        }
        public override void AddDeclaration(Decl decl)
        {
            switch (decl)
            {
                case ArrayVarDecl arrayVarDecl:
                    if (arrayVarDecl.Parent is null)
                    {
                        arrayVarDecl.Parent = this;
                    }
                    else if (arrayVarDecl.Parent != this)
                    {
                        throw new InternalErrorException("Different parent detacted");
                    }
                    Decls.Add(arrayVarDecl);
                    SymbolTable.AddArrayOrException(arrayVarDecl.Name, arrayVarDecl.Type, arrayVarDecl.Size);
                    break;
                case VarDecl varDecl:
                    if (varDecl.Parent is null)
                    {
                        varDecl.Parent = this;
                    }
                    else if (varDecl.Parent != this)
                    {
                        throw new InternalErrorException("Different parent detacted");
                    }
                    Decls.Add(varDecl);
                    SymbolTable.AddOrException(varDecl.Name, varDecl.Type);
                    break;
                default:
                    throw new SemanticErrorException($"Unexpected token {decl}, expected VarDecl");
            }
        }
        public override IList<SymbolTableItem> CollectSymbolTableItems()
        {
            List<SymbolTableItem> symbolTableItems = new();
            symbolTableItems.AddRange(SymbolTable.Symbols.Values);
            foreach (var stmt in Stmts)
            {
                symbolTableItems.AddRange(stmt.CollectSymbolTableItems());
            }
            return symbolTableItems;
        }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            foreach (var item in Stmts)
            {
                irList.AddRange(item.GenerateIR(parentFunction));
            }
            return irList;
        }
    }
}
