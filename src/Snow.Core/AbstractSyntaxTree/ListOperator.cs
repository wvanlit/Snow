using Snow.Core.Extensions;

namespace Snow.Core.AbstractSyntaxTree;

using static Constants.Syntax;

public static class ListOperations
{
    public static readonly Dictionary<string, ListOperation> NumberOperations = new()
    {
        {Plus, new ReducibleListOperation<double>((sum, curr) => sum + curr)},
        {Minus, new ReducibleListOperation<double>((sum, curr) => sum - curr)},
        {Divide, new ReducibleListOperation<double>((sum, curr) => sum / curr)},
        {Multiply, new ReducibleListOperation<double>((sum, curr) => sum * curr)},

        {LesserThan, new ConditionalListOperation<double>((prev, curr) => prev < curr)},
        {LesserThanOrEqual, new ConditionalListOperation<double>((prev, curr) => prev <= curr)},
        {GreaterThan, new ConditionalListOperation<double>((prev, curr) => prev > curr)},
        {GreaterThanOrEqual, new ConditionalListOperation<double>((prev, curr) => prev >= curr)},

        {EqualsTo, new ConditionalListOperation<double>((prev, curr) => Math.Abs(prev - curr) < 0.0001)},
        {NotEqualsTo, new ConditionalListOperation<double>((prev, curr) => Math.Abs(prev - curr) > 0.0001)}
    };

    public static readonly Dictionary<string, ListOperation> BooleanOperations = new()
    {
        {EqualsTo, new ConditionalListOperation<bool>((prev, curr) => prev == curr)},
        {NotEqualsTo, new ConditionalListOperation<bool>((prev, curr) => prev != curr)},

        {And, new ConditionalListOperation<bool>((prev, curr) => prev && curr)},
        {Or, new ConditionalListOperation<bool>((prev, curr) => prev || curr)}
    };
}

public abstract class ListOperation
{
    public abstract Value EvalList(IEnumerable<Value> values);

    protected static IEnumerable<T> ToType<T>(IEnumerable<Value> values) => values.Select(v => v.GetValue<T>());
}

public class ReducibleListOperation<T> : ListOperation
{
    public ReducibleListOperation(Func<T, T, T> evaluator)
    {
        Evaluator = evaluator;
    }

    private Func<T, T, T> Evaluator { get; }

    public override Value EvalList(IEnumerable<Value> values) =>
        new Value(ToType<T>(values).Aggregate(Evaluator)).TryConvert();
}

public class ConditionalListOperation<T> : ListOperation
{
    public ConditionalListOperation(Func<T, T, bool> evaluator)
    {
        Evaluator = evaluator;
    }

    private Func<T, T, bool> Evaluator { get; }

    public override Value EvalList(IEnumerable<Value> values) =>
        new Value(ToType<T>(values).ToList().SlidingWindow(2).All(pair => Evaluator(pair[0], pair[1]))).TryConvert();
}
