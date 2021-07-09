using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCCompiler.AST.Decl;
using SimpleCCompiler.AST.Expr;
using SimpleCCompiler.IR.Instrunction;

namespace SimpleCCompiler.AST.Stmt
{
    public class ForStmt : Stmt
    {
        public List<VarDecl> Decls { get; set; } = new();
        public IExpr InitExpr { get; set; }
        public IExpr ConditionalExpr { get; set; } 
        public IExpr EndExpr { get; set; }
        public IStmt LoopBodyStmt { get; set; }
        public SymbolTable SymbolTable { get; set; } = new();
        // for IR generation
        public LabelInstruction StartLabel { get; set; } = new();
        public LabelInstruction EndExprLabel { get; set; } = new();
        public LabelInstruction EndLabel { get; set; } = new();
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
            if (LoopBodyStmt is CompoundStmt)
            {
                symbolTableItems.AddRange(LoopBodyStmt.CollectSymbolTableItems());
            }
            return symbolTableItems;
        }
    }
}
