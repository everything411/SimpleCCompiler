using SimpleCCompiler.AST;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SimpleCCompiler
{
    public class ParameterSymbolTableItem : SymbolTableItem
    {
        // hack for function argument
        public int ParameterNumber { get; set; }
        public ParameterSymbolTableItem(AST.Type type, string name, int n) : base(type, name)
        {
            ParameterNumber = n;
        }
    }
    public class ArraySymbolTableItem : SymbolTableItem
    {
        public int ArraySize { get; set; }
        public ArraySymbolTableItem(AST.Type type, string name, int size) : base(type, name)
        {
            ArraySize = size;
        }
        public override string ToString()
        {
            return $"{base.ToString()}:{ArraySize}";
        }
    }
    public class SymbolTableItem
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public AST.Type Type { get; set; }
        public string Name { get; set; }
        public SymbolTableItem(AST.Type type, string name)
        {
            Type = type;
            Name = name;
        }
        public override string ToString()
        {
            return $"{base.ToString()}:{Type}:{Name}";
        }
    }
    public class SymbolTable
    {
        public Dictionary<string, SymbolTableItem> Symbols { get; set; } = new();

        private void Add(string name, AST.Type type)
        {
            Symbols.Add(name, new SymbolTableItem(type, name));
        }
        public void AddOrException(string name, AST.Type type)
        {
            if (Symbols.GetValueOrDefault(name) is not null)
            {
                throw new SemanticErrorException($"Symbol {name} redefined");
            }
            Add(name, type);
        }
        public void AddParameterOrException(string name, AST.Type type, int n)
        {
            if (Symbols.GetValueOrDefault(name) is not null)
            {
                throw new SemanticErrorException($"Symbol {name} redefined");
            }
            Symbols.Add(name, new ParameterSymbolTableItem(type, name, n));
        }
        public void AddArrayOrException(string name, AST.Type type, int n)
        {
            if (Symbols.GetValueOrDefault(name) is not null)
            {
                throw new SemanticErrorException($"Symbol {name} redefined");
            }
            Symbols.Add(name, new ArraySymbolTableItem(type, name, n));
        }
        public bool TryGetValue(string name, out SymbolTableItem symbolTableItem)
        {
            return Symbols.TryGetValue(name, out symbolTableItem);
        }
        public override string ToString() => $"{base.ToString()}:{JsonSerializer.Serialize(Symbols)}";
    }
}
