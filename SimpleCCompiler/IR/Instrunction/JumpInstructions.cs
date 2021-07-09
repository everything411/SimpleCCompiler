namespace SimpleCCompiler.IR.Instrunction
{
    /*
        Jmp,
        Jz,
        Jnz,
        Jg,
        Jge,
        Jl,
        Jle,
     */
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
    }
    public class JzInstruction : JumpInstruction
    {
        public JzInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jz;
        }
    }
    public class JnzInstruction : JumpInstruction
    {
        public JnzInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jnz;
        }
    }
    public class JgInstruction : JumpInstruction
    {
        public JgInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jg;
        }
    }
    public class JlInstruction : JumpInstruction
    {
        public JlInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jl;
        }
    }
    public class JgeInstruction : JumpInstruction
    {
        public JgeInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jge;
        }
    }
    public class JleInstruction : JumpInstruction
    {
        public JleInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jle;
        }
    }
    public class JeInstruction : JumpInstruction
    {
        public JeInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Je;
        }
    }
    public class JneInstruction : JumpInstruction
    {
        public JneInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jne;
        }
    }
}
