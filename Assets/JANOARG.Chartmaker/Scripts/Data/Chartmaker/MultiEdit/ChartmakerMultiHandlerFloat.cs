using JANOARG.Chartmaker.Utils.Math;
using JANOARG.Shared.Data.ChartInfo;
using UnityEngine;

namespace JANOARG.Chartmaker.Data.Chartmaker.MultiEdit
{
    public class ChartmakerMultiHandlerFloat: ChartmakerLerpableMultiHandler<float>
    {

        public override ExpressionContext GetExpressionContext()
        {
            return new ExpressionContext().SetVariable("x", 0).SetVariable("i", 0).SetVariable("t", 0);
        }

        public override float Get(float from, object source, int index = 0) 
        {
            float to = LerpField == null
                ? LerpTo : Mathf.InverseLerp(LerpFrom, LerpTo, LerpField.FieldType == typeof(BeatPosition) 
                    ? (BeatPosition)LerpField.GetValue(source) : (float)LerpField.GetValue(source));

            if (Operation == LerpableOperation.Expression)
            {
                CustomExpressionContext ??= GetExpressionContext();
                CustomExpressionContext
                    .SetVariable("x", from)
                    .SetVariable("i", index)
                    .SetVariable("t", to)
                ;
                return (float)CustomExpression.Evaluate(CustomExpressionContext);
            }
            else
            {
                to = float.IsFinite(From) ? Mathf.Lerp(From, To, LerpEasing.Get(to)) : To;

                return LerpableOperations.Get[Operation](from, to);
            }
        }
    }
}