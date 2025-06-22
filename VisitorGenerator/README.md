# VisitorGenerator

Generates visitor pattern implementation for tree structures.

## Example

The following example code illustrates using the generated visitor implementation by implementing 
a printer and interpreter for a simple language.

### Define Tree Node Types

All node types to be included in the visitor implementations must be defined by annotating the abstract root node type with instances of the `GenerateVisitorAttribute`.

```cs
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

internal abstract class BinaryExpression : Expression
{
    public required Expression Lhs { get; init; }
    public required Expression Rhs { get; init; }
}

internal sealed partial class StatementList : SyntaxNode
{
    public required IEnumerable<Statement> Statements { get; init; }
}

internal abstract class Expression : SyntaxNode;

internal abstract class Statement : SyntaxNode;
```

### Implement the Printer

The printer visitor is intended to simply print expressions to the console, without producing any results.
Note that it hooks into `OnBeforeVisit` and `OnAfterVisit` template methods in order to surround only binary expressions with parentheses.
Literals and variable expressions are not parenthesized.
```cs
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
```

### Implement the Interpreter

The interpreter is a simple treewalking interpreter that ambiently stores variable assignments in a hashmap. 
The last expression in the list of statements visited determines the result of the interpreter.

```cs
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
        foreach(var statement in target.Statements)
            _lastStatementValue = statement.Accept(this, cancellationToken);

        return _lastStatementValue;
    }
}
```

### Define Ast

Define an Ast to test our visitor implementation:
```cs
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
```

### Observe Output

The first two lines are the result of our printer implementation, while the last line represents the interpreter result value.

```
foo = (6 + 5)
bar = ((foo / (6 * 0.5)) - (2 / 3))
3
```

## Restrictions

- node base type must be abstract
- node base type must be class or record class
- node types must inherit base node
- nested types are disallowed

## TODO

- implement analyzer to reflect restrictions
