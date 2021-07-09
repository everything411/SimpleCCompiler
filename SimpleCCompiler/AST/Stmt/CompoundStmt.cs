using SimpleCCompiler.AST.Decl;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Stmt
{
    public class CompoundStmt : Stmt
    {
        public SymbolTable SymbolTable { get; set; } = new();
        public List<VarDecl> Decls { get; set; } = new();
        // TODO: hack
        // public List<object> Stmts { get; set; } = new();
        public List<IStmt> Stmts { get; set; } = new();
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
                    Decls.Add(varDecl);
                    SymbolTable.AddOrException(varDecl.Name, varDecl.Type);
                    break;
                default:
                    throw new SemanticErrorException($"Unexpected token {decl}, expected VarDecl");
            }
        }
        public override IEnumerable<SymbolTableItem> CollectSymbolTableItems()
        {
            List<SymbolTableItem> symbolTableItems = new();
            symbolTableItems.AddRange(SymbolTable.Symbols.Values);
            foreach (var stmt in Stmts)
            {
                symbolTableItems.AddRange(stmt.CollectSymbolTableItems());
            }
            return symbolTableItems;
        }
    }
}
