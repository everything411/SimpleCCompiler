using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;

namespace SimpleCCompiler.IR
{
    public class Function : IIR
    {
        // private int 
        public string Name { get; set; }
        public IR.Type Type { get; set; }
        public IRSymbolTable IRSymbolTable { get; set; } = new();
        public int AutoVariableCount { get; set; } = 0;
        public List<IInstruction> IRs { get; set; } = new();
        public List<ReturnInstrunction> ReturnInstrunctions { get; set; } = new();
        public List<IRStringLiteral> StringLiterals { get; set; } = new();
        public void AddAutoVariable(Variable v)
        {
            AutoVariableCount += 1;
            v.OffsetEBP = - (AutoVariableCount + 1) * 4;
            IRSymbolTable.VariableDictionary.Add(v.Guid, v);
        }
        public void AddAutoArrayVariable(ArrayVariable v)
        {
            AutoVariableCount += v.Size;
            v.OffsetEBP = - (AutoVariableCount + 1) * 4;
            IRSymbolTable.VariableDictionary.Add(v.Guid, v);
        }
        public Variable AllocateTempVariable(IR.Type type)
        {
            Variable v = new();
            v.Guid = Guid.NewGuid();
            v.Type = type;
            v.Name = "V_" + v.Guid.ToString("N").Substring(0, 8);
            AutoVariableCount += 1;
            v.OffsetEBP = - (AutoVariableCount + 1) * 4;
            IRSymbolTable.VariableDictionary.Add(v.Guid, v);
            return v;
        }
    }
}
