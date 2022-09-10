﻿using System.Linq.Expressions;
using System.Reflection;

namespace Tybscri.Nodes;

public class MemberAccessNode : IExpressionNode
{
    public MemberAccessNode(IExpressionNode instance, Token memberName)
    {
        Instance = instance;
        MemberName = memberName;
        Children = new[] { Instance };
    }

    private TybscriMember? _member;

    public IExpressionNode Instance { get; }

    public Token MemberName { get; }

    public IReadOnlyCollection<INode> Children { get; }

    public Scope Scope { get; private set; } = Scope.Empty;

    public TybscriType ExpressionType { get; private set; } = UnknownType.Instance;

    public void SetupScopes(Scope scope)
    {
        Instance.SetupScopes(scope);
        Scope = scope;
    }

    public void Resolve(ResolveContext context)
    {
        Instance.Resolve(context);

        var matchingMembers = Instance.ExpressionType.FindMembersByName(MemberName.Text);
        if (matchingMembers.Count == 0) {
            return;
        }

        if (matchingMembers.Count > 1) {
            return;
        }

        _member = matchingMembers.First();
        ExpressionType = _member.Type;
    }

    public Expression ToClrExpression(GenerateContext generateContext)
    {
        if (_member is null) {
            throw new InvalidOperationException("Unknown member");
        }

        var memberExpression = System.Linq.Expressions.Expression.Property(Instance.ToClrExpression(generateContext),
            (PropertyInfo)_member.MemberInfo);

        // if (memberExpression.Type.IsAssignableTo(_member.Type.ClrType)) {
        //     return memberExpression;
        // }
        
        return Expression.Convert(memberExpression, _member.Type.ClrType);
    }
}