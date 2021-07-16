using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;

namespace SimpleCCompiler.AST
{
    public class ForStmt : Stmt
    {
        public List<VarDecl> Decls { get; set; } = new();
        public Expr InitExpr { get; set; }
        public Expr ConditionalExpr { get; set; } 
        public Expr EndExpr { get; set; }
        public Stmt LoopBodyStmt { get; set; }
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
        public override void AddDeclaration(Decl decl)
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
        public override IList<SymbolTableItem> CollectSymbolTableItems()
        {
            List<SymbolTableItem> symbolTableItems = new();
            symbolTableItems.AddRange(SymbolTable.Symbols.Values);
            if (LoopBodyStmt is CompoundStmt)
            {
                symbolTableItems.AddRange(LoopBodyStmt.CollectSymbolTableItems());
            }
            return symbolTableItems;
        }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            irList.AddRange(InitExpr.GenerateIR(parentFunction));
            irList.Add(StartLabel);
            var cmpIRList = ConditionalExpr.GenerateIR(parentFunction);
            irList.AddRange(cmpIRList);
            var cmpIR = irList[^1] as CmpInstruction;
            Instruction jumpInstruction = cmpIR.BinaryOperator switch
            {
                BinaryOperator.Less => new JgeInstruction(EndLabel),
                BinaryOperator.Greater => new JlInstruction(EndLabel),
                BinaryOperator.LessEqual => new JgInstruction(EndLabel),
                BinaryOperator.GreaterEqual => new JlInstruction(EndLabel),
                BinaryOperator.Equal => new JneInstruction(EndLabel),
                BinaryOperator.NotEqual => new JeInstruction(EndLabel),
                _ => throw new NotImplementedException($"{cmpIR.BinaryOperator} not implemented"),
            };
            irList.Add(jumpInstruction);
            irList.AddRange(LoopBodyStmt.GenerateIR(parentFunction));
            irList.Add(EndExprLabel);
            irList.AddRange(EndExpr.GenerateIR(parentFunction));
            irList.Add(new JmpInstruction(StartLabel));
            irList.Add(EndLabel);
            return irList;
        }
    }
}
