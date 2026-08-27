

namespace JANOARG.Chartmaker.Utils.Math
{
    public abstract class Expression
    {
        public abstract double Evaluate(ExpressionContext context);
    }

    internal class VariableExpression : Expression
    {
        public string Name;
        public override double Evaluate(ExpressionContext context)
        {
            if (context == null || !context.HasVariable(Name))
            {
                throw new ExpressionException($"Unknown variable '{Name}'");
            }
            return context.GetVariable(Name);
        }
    }

    internal class ConstantExpression : Expression
    {
        public double Value;
        public override double Evaluate(ExpressionContext context)
        {
            return Value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal class PrefixOperatorExpression : Expression
    {
        public Operator Operator;
        public Expression RightExpression;
        public override double Evaluate(ExpressionContext context)
        {
            return Operator.PrefixFunction(RightExpression.Evaluate(context));
        }
        public override string ToString()
        {
            return $"({Operator} {RightExpression})";
        }
    }

    internal class InfixOperatorExpression : Expression
    {
        public Operator Operator;
        public Expression LeftExpression;
        public Expression RightExpression;
        public override double Evaluate(ExpressionContext context)
        {
            return Operator.InfixFunction(LeftExpression.Evaluate(context), RightExpression.Evaluate(context));
        }
        public override string ToString()
        {
            return $"({Operator} {LeftExpression} {RightExpression})";
        }
    }
}