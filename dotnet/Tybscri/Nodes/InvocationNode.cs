﻿using System.Linq.Expressions;
using Tybscri.LinqExpressions;

namespace Tybscri.Nodes;

public class InvocationNode : IExpressionNode
{
    public InvocationNode(IExpressionNode target, IReadOnlyList<IExpressionNode> arguments)
    {
        Target = target;
        Arguments = arguments;

        Children = new[] { target }.Concat(arguments).ToArray();
    }

    public IExpressionNode Target { get; }

    public IReadOnlyList<IExpressionNode> Arguments { get; }

    public IReadOnlyCollection<INode> Children { get; }

    public Scope Scope { get; private set; } = Scope.Empty;

    public TybscriType ExpressionType { get; private set; } = UnknownType.Instance;

    public void SetupScopes(Scope scope)
    {
        Target.SetupScopes(scope);

        foreach (var arg in Arguments) {
            arg.SetupScopes(scope);
        }

        Scope = scope;
    }

    public void Resolve(ResolveContext context)
    {
        Target.Resolve(context);

        if (Target.ExpressionType is not FuncType funcType) {
            throw new TybscriException("Cannot invoke value of non func type");
        }

        var args = Arguments;
        // const args  = [
        // ...this.argumentList,
        // ...(this.trailingLambda?[this.trailingLambda] : []), ]
        // ;
        for (var i = 0; i < args.Count; i++) {
            var arg = args[i];
            var parameter = funcType.Parameters.ElementAtOrDefault(i);
            var expectedType = parameter?.Type;
            arg.Resolve(context with { ExpectedType = expectedType });

            if (expectedType is null || arg.ExpressionType is null) {
                throw new NotImplementedException("Should report error");
            }

            if (!expectedType.IsAssignableFrom(arg.ExpressionType)) {
            }
        }

        ExpressionType = funcType.ReturnType;
    }

    public Expression ToClrExpression(GenerateContext generateContext)
    {
        return new TybscriInvokeExpression(Target.ToClrExpression(generateContext),
            Arguments.Select(x => x.ToClrExpression(generateContext)));
    }
}