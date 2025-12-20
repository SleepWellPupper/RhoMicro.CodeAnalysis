// SPDX-License-Identifier: MPL-2.0

namespace VisitorExample;

using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using RhoMicro.CodeAnalysis;


internal class Program
{
    private static void Main()
    {
        var program = new StatementList()
        {
            Statements =
            [
                new ExpressionStatement()
                {
                    Expression = new AssignmentExpression()
                    {
                        Name = "foo",
                        Expression = new AdditionExpression()
                        {
                            Lhs = new LiteralExpression() { Value = 6 },
                            Rhs = new LiteralExpression() { Value = 5 }
                        }
                    }
                },
                new ExpressionStatement()
                {
                    Expression = new AssignmentExpression(){
                        Name = "bar",
                        Expression =new SubtractionExpression(){
                        Lhs = new DivisionExpression()
                        {
                            Lhs = new VariableExpression(){Name = "foo"},
                            Rhs = new MultiplicationExpression(){
                                Lhs = new LiteralExpression(){ Value = 6 },
                                Rhs = new LiteralExpression(){ Value = 0.5 }
                            }
                        },
                            Rhs = new DivisionExpression(){
                                Lhs= new LiteralExpression(){ Value = 2},
                                Rhs = new LiteralExpression(){Value = 3}
                            }
                        }
                    }
                }
            ]
        };

        program.Accept(Printer.Instance);

        var result = program.Accept(new Interpreter());

        Console.WriteLine(result);

        program.Accept(Printer.Instance);

        result = program.Accept(new Interpreter());

        Console.WriteLine(result);
    }
}

internal sealed class Printer : SyntaxNodeVisitor
{
    private Printer() { }

    public static Printer Instance { get; } = new();

    public override void OnBeforeVisitAdditionExpression(AdditionExpression target, CancellationToken cancellationToken = default)
        => Console.Write('(');
    public override void TraverseAdditionExpression(AdditionExpression target, CancellationToken cancellationToken = default)
    {
        target.Lhs.Accept(this, cancellationToken);
        Console.Write(" + ");
        target.Rhs.Accept(this, cancellationToken);
    }
    public override void OnAfterVisitAdditionExpression(AdditionExpression target, CancellationToken cancellationToken = default)
        => Console.Write(')');

    public override void OnBeforeVisitSubtractionExpression(SubtractionExpression target, CancellationToken cancellationToken = default)
        => Console.Write('(');
    public override void TraverseSubtractionExpression(SubtractionExpression target, CancellationToken cancellationToken = default)
    {
        target.Lhs.Accept(this, cancellationToken);
        Console.Write(" - ");
        target.Rhs.Accept(this, cancellationToken);
    }
    public override void OnAfterVisitSubtractionExpression(SubtractionExpression target, CancellationToken cancellationToken = default)
        => Console.Write(')');

    public override void OnBeforeVisitMultiplicationExpression(MultiplicationExpression target, CancellationToken cancellationToken = default)
        => Console.Write('(');
    public override void TraverseMultiplicationExpression(MultiplicationExpression target, CancellationToken cancellationToken = default)
    {
        target.Lhs.Accept(this, cancellationToken);
        Console.Write(" * ");
        target.Rhs.Accept(this, cancellationToken);
    }
    public override void OnAfterVisitMultiplicationExpression(MultiplicationExpression target, CancellationToken cancellationToken = default)
        => Console.Write(')');

    public override void OnBeforeVisitDivisionExpression(DivisionExpression target, CancellationToken cancellationToken = default)
        => Console.Write('(');
    public override void TraverseDivisionExpression(DivisionExpression target, CancellationToken cancellationToken = default)
    {
        target.Lhs.Accept(this, cancellationToken);
        Console.Write(" / ");
        target.Rhs.Accept(this, cancellationToken);
    }
    public override void OnAfterVisitDivisionExpression(DivisionExpression target, CancellationToken cancellationToken = default)
        => Console.Write(')');

    public override void VisitLiteralExpression(LiteralExpression target, CancellationToken cancellationToken = default)
        => Console.Write(target.Value.ToString(CultureInfo.InvariantCulture));
    public override void OnAfterVisitExpressionStatement(ExpressionStatement target, CancellationToken cancellationToken = default)
        => Console.WriteLine();
    public override void VisitAssignmentExpression(AssignmentExpression target, CancellationToken cancellationToken = default)
    {
        Console.Write($"{target.Name} = ");
        target.Expression.Accept(this, cancellationToken);
    }
    public override void VisitVariableExpression(VariableExpression target, CancellationToken cancellationToken = default)
        => Console.Write(target.Name);
}

internal sealed class Interpreter : SyntaxNodeVisitor<Double>
{
    protected override Double GetDefault() => 0;

    private readonly Dictionary<String, Double> _variables = [];
    private Double _lastStatementValue;

    public override Double VisitLiteralExpression(LiteralExpression target, CancellationToken cancellationToken = default)
        => target.Value;
    public override Double VisitAdditionExpression(AdditionExpression target, CancellationToken cancellationToken = default)
        => target.Lhs.Accept(this, cancellationToken) + target.Rhs.Accept(this, cancellationToken);
    public override Double VisitSubtractionExpression(SubtractionExpression target, CancellationToken cancellationToken = default)
        => target.Lhs.Accept(this, cancellationToken) - target.Rhs.Accept(this, cancellationToken);
    public override Double VisitMultiplicationExpression(MultiplicationExpression target, CancellationToken cancellationToken = default)
        => target.Lhs.Accept(this, cancellationToken) * target.Rhs.Accept(this, cancellationToken);
    public override Double VisitDivisionExpression(DivisionExpression target, CancellationToken cancellationToken = default)
        => target.Lhs.Accept(this, cancellationToken) / target.Rhs.Accept(this, cancellationToken);
    public override Double VisitAssignmentExpression(AssignmentExpression target, CancellationToken cancellationToken = default)
        => _variables[target.Name] = target.Expression.Accept(this, cancellationToken);
    public override Double VisitVariableExpression(VariableExpression target, CancellationToken cancellationToken = default)
        => _variables.GetValueOrDefault(target.Name);
    public override Double VisitExpressionStatement(ExpressionStatement target, CancellationToken cancellationToken = default)
        => _lastStatementValue = target.Expression.Accept(this, cancellationToken);
    public override Double VisitStatementList(StatementList target, CancellationToken cancellationToken = default)
    {
        foreach (var statement in target.Statements)
            _lastStatementValue = statement.Accept(this, cancellationToken);

        return _lastStatementValue;
    }
}

internal sealed partial class AssignmentExpression : Expression
{
    public required String Name { get; init; }
    public required Expression Expression { get; init; }
}

internal sealed partial class ExpressionStatement : Statement
{
    public required Expression Expression { get; init; }
}

internal sealed partial class VariableExpression : Expression
{
    public required String Name { get; init; }
}

internal sealed partial class LiteralExpression : Expression
{
    public required Double Value { get; init; }
}

internal sealed partial class AdditionExpression : BinaryExpression;
internal sealed partial class SubtractionExpression : BinaryExpression;
internal sealed partial class MultiplicationExpression : BinaryExpression;
internal sealed partial class DivisionExpression : BinaryExpression;

internal abstract partial class BinaryExpression : Expression
{
    public required Expression Lhs { get; init; }
    public required Expression Rhs { get; init; }
}

internal sealed partial class StatementList : SyntaxNode
{
    public required IEnumerable<Statement> Statements { get; init; }
}

internal abstract partial class Expression : SyntaxNode;

internal abstract partial class Statement : SyntaxNode;

[GenerateVisitor<
    AdditionExpression,
    SubtractionExpression,
    MultiplicationExpression,
    DivisionExpression,
    LiteralExpression,
    AssignmentExpression,
    VariableExpression,
    StatementList>,
 GenerateVisitor<
    ExpressionStatement>]
internal abstract partial class SyntaxNode;
