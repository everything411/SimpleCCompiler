using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class IfStmt : Stmt
    {
        public Expr ConditionalExpr { get; set; }
        public Stmt IfTrueBody { get; set; }
        public Stmt IfFalseBody { get; set; } = new EmptyStmt();
        public LabelInstruction IfEndLabel { get; set; } = new();
        public LabelInstruction IfFalseLabel { get; set; } = new();
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
        public override IList<SymbolTableItem> CollectSymbolTableItems()
        {
            return IfTrueBody.CollectSymbolTableItems();
        }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            var cmpIRList = ConditionalExpr.GenerateIR(parentFunction);
            irList.AddRange(cmpIRList);
            var cmpIR = irList[^1] as CmpInstruction;
            Instruction jumpInstruction = cmpIR.BinaryOperator switch
            {
                BinaryOperator.Less => new JgeInstruction(IfFalseLabel),
                BinaryOperator.Greater => new JlInstruction(IfFalseLabel),
                BinaryOperator.LessEqual => new JgInstruction(IfFalseLabel),
                BinaryOperator.GreaterEqual => new JlInstruction(IfFalseLabel),
                BinaryOperator.Equal => new JneInstruction(IfFalseLabel),
                BinaryOperator.NotEqual => new JeInstruction(IfFalseLabel),
                _ => throw new NotImplementedException($"{cmpIR.BinaryOperator} not implemented"),
            };
            irList.Add(jumpInstruction);
            irList.AddRange(IfTrueBody.GenerateIR(parentFunction));
            irList.Add(new JmpInstruction(IfEndLabel));
            irList.Add(IfFalseLabel);
            irList.AddRange(IfFalseBody.GenerateIR(parentFunction));
            irList.Add(IfEndLabel);
            return irList;
        }
    }
}
