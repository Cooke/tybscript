﻿using System.Linq.Expressions;
using Tybscri.Utils;

namespace Tybscri.Nodes;

internal class IfNode : Node
{
    public Node Exp { get; }

    public Node Then { get; }

    public Node? ElseNode { get; }

    public IfNode(Token ifToken, Token lparen, Node exp, Token rparen, Node then, Node? elseNode) : base(exp, then)
    {
        Exp = exp;
        Then = then;
        ElseNode = elseNode;
    }
    
    public override void SetupScopes(ScopeContext scopeContext)
    {
        
    }

    public override void ResolveTypes(AnalyzeContext context)
    {
    }

    public override void Resolve(CompileContext context)
    {
        Exp.Resolve(context);
        Then.Resolve(context);
        ElseNode?.Resolve(context);
        Scope = context.Scope;
    }

    public override Expression ToClrExpression(GenerateContext generateContext)
    {
        var elseExp = ElseNode?.ToClrExpression(generateContext) ?? Expression.Constant(null, typeof(object));
        var thenExp = Then.ToClrExpression(generateContext);
        return Expression.Condition(Exp.ToClrExpression(generateContext), thenExp, elseExp,
            ClrTypeUtils.FindCommonType(thenExp.Type, elseExp.Type));
    }
}