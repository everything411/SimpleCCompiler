using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.IR.Instrunction
{
    public class LabelInstruction : Instruction
    {
        public string Name { get; set; } = "L_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        public override string EmitAssembly()
        {
            return Name + ":";
        }
        public override string ToString()
        {
            return $"(Label, {Name}, , )";
        }
    }
}
