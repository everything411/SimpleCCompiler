using Antlr4.Runtime.Tree;
using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Stmt
{
    public class Stmt : IStmt
    {
        public INode Parent
        {
            get; set;
        }

        public ForStmt GetIterationStmtOrException()
        {
            // Console.WriteLine($"this is {this}, Parent is {Parent}");
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

        public virtual IList<IInstruction> EmitIR()
        {
            throw new NotImplementedException();
        }

        public virtual SymbolTableItem LookupSymbolTable(string name)
        {
            throw new NotImplementedException();
        }

        public virtual void AddDeclaration(IDecl decl)
        {
            throw new NotImplementedException();
        }
    }
}
