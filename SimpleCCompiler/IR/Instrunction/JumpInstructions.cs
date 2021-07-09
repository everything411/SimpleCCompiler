using System.Text;

namespace SimpleCCompiler.IR.Instrunction
{
    public class JumpInstruction : Instruction
    {
        public LabelInstruction Label { get; set; }
        public JumpInstruction(LabelInstruction label)
        {
            Label = label;
        }
    }
    public class JmpInstruction : JumpInstruction
    {
        public JmpInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jmp;
        }
        public override string EmitAssembly()
        {
            return $"jmp {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JmpInstruction, {Label.Name}, , )";
        }
    }
    /*
    public class JzInstruction : JumpInstruction
    {
        public JzInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jz;
        }
        public override string EmitAssembly()
        {
            return $"jz {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JzInstruction, {Label.Name}, , )";
        }
    }
    public class JnzInstruction : JumpInstruction
    {
        public JnzInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jnz;
        }
        public override string EmitAssembly()
        {
            return $"jnz {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JnzInstruction, {Label.Name}, , )";
        }
    }
    */
    public class JgInstruction : JumpInstruction
    {
        public JgInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jg;
        }
        public override string EmitAssembly()
        {
            return $"jg {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JgInstruction, {Label.Name}, , )";
        }
    }
    public class JlInstruction : JumpInstruction
    {
        public JlInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jl;
        }
        public override string EmitAssembly()
        {
            return $"jl {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JlInstruction, {Label.Name}, , )";
        }
    }
    public class JgeInstruction : JumpInstruction
    {
        public JgeInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jge;
        }
        public override string EmitAssembly()
        {
            return $"jge {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JgeInstruction, {Label.Name}, , )";
        }
    }
    public class JleInstruction : JumpInstruction
    {
        public JleInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jle;
        }
        public override string EmitAssembly()
        {
            return $"jle {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JleInstruction, {Label.Name}, , )";
        }
    }
    public class JeInstruction : JumpInstruction
    {
        public JeInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Je;
        }
        public override string EmitAssembly()
        {
            return $"je {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JeInstruction, {Label.Name}, , )";
        }
    }
    public class JneInstruction : JumpInstruction
    {
        public JneInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jne;
        }
        public override string EmitAssembly()
        {
            return $"jne {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JneInstruction, {Label.Name}, , )";
        }
    }
}
