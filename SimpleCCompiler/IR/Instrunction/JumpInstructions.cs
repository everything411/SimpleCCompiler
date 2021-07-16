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
        public override string GenerateAssembly()
        {
            return $";{ToString()}\njmp {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JmpInstruction, {Label.Name}, , )";
        }
    }
    public class JgInstruction : JumpInstruction
    {
        public JgInstruction(LabelInstruction label) : base(label)
        {
            Operation = Operation.Jg;
        }
        public override string GenerateAssembly()
        {
            return $";{ToString()}\njg {Label.Name}";
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
        public override string GenerateAssembly()
        {
            return $";{ToString()}\njl {Label.Name}";
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
        public override string GenerateAssembly()
        {
            return $";{ToString()}\njge {Label.Name}";
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
        public override string GenerateAssembly()
        {
            return $";{ToString()}\njle {Label.Name}";
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
        public override string GenerateAssembly()
        {
            return $";{ToString()}\nje {Label.Name}";
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
        public override string GenerateAssembly()
        {
            return $";{ToString()}\njne {Label.Name}";
        }
        public override string ToString()
        {
            return $"(JneInstruction, {Label.Name}, , )";
        }
    }
}
