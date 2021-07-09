﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleCCompiler.AST.Decl
{
    public class Decl : IDecl
    {
        [JsonIgnore]
        public INode Parent { get; set; }
        public virtual SymbolTableItem LookupSymbolTable(string name)
        {
            throw new NotImplementedException();
        }
        public virtual void AddDeclaration(IDecl decl)
        {
            throw new NotImplementedException();
        }
        public virtual IEnumerable<SymbolTableItem> CollectSymbolTableItems()
        {
            throw new NotImplementedException();
        }
    }
}
