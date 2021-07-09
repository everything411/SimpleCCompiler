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
        public List<IIR> IRs { get; set; } = new();
        public List<IRStringLiteral> StringLiterals { get; set; } = new();
        public void AddAutoVariable(Variable v)
        {
            v.OffsetEBP = - ++AutoVariableCount * 4;
            IRSymbolTable.VariableDictionary.Add(v.Guid, v);
        }
        public void AddAutoArrayVariable(ArrayVariable v)
        {
            v.OffsetEBP = - (AutoVariableCount + 1) * 4;
            AutoVariableCount += v.Size;
            IRSymbolTable.VariableDictionary.Add(v.Guid, v);
            // IRSymbolTable.ArrayDictionary.Add(v.Guid, v);
        }
        public Variable AllocateTempVariable(IR.Type type)
        {
            Variable v = new();
            v.Guid = Guid.NewGuid();
            v.Type = type;
            v.Name = "V_" + v.Guid.ToString("N").Substring(0, 8);
            v.OffsetEBP = - ++AutoVariableCount * 4;
            IRSymbolTable.VariableDictionary.Add(v.Guid, v);
            return v;
        }
    }
}
