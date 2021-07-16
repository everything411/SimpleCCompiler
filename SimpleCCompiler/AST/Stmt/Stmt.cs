using System;
using System.Collections.Generic;

namespace SimpleCCompiler.AST
{
    public class Stmt : Node
    {
        public ForStmt GetIterationStmtOrException()
        {
            if (this is ForStmt)
            {
                return this as ForStmt;
            }
            else if (Parent is Stmt)
            {
                return (Parent as Stmt).GetIterationStmtOrException();
            }
            else
            {
                throw new SemanticErrorException($"Unexpected 'break' in {this}, not in Iteration");
            }
        }
        public override IList<SymbolTableItem> CollectSymbolTableItems()
        {
            List<SymbolTableItem> symbolTableItems = new(0);
            return symbolTableItems;
        }
    }
}
