using System;

namespace SimpleCCompiler.AST
{
    public class SyntaxErrorException : Exception 
    {
        public SyntaxErrorException(string e) : base(e) { }
    }
    public class SemanticErrorException : Exception 
    { 
        public SemanticErrorException(string e) : base(e) { }
    }
    public class InternalErrorException : Exception
    {
        public InternalErrorException(string e) : base(e) { }
    }
}
