﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Expr
{
    public class UnaryOperatorExpr : Expr
    {
        public UnaryOperator Operator { get; set; }
        public IExpr Expression { get; set; }
    }
}