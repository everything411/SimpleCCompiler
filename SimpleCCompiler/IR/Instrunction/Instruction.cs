using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR.Instrunction
{
    public enum Operation
    {
        Unknown,
        Jmp,
        Jz,
        Jnz,
        Jg,
        Jge,
        Jl,
        Jle,
        Je,
        Jne,
        Return,
        Call,
        Add,
        Sub,
        Cmp,
        Mul,
        Div,
        Mod,
        Inc,
        Dec,
        Mov
    }
public class Instruction
    {
        public Operation Operation { get; set; }
        public Variable Argument1 { get; set; }
        public Variable Argument2 { get; set; }
        public Variable Result { get; set; }

        public virtual string GenerateAssembly()
        {
            throw new NotImplementedException();
        }
    }
}
