using JANOARG.Chartmaker.Utils.Math;
using JANOARG.Shared.Data.ChartInfo;
using UnityEngine;

namespace JANOARG.Chartmaker.Data.Chartmaker.MultiEdit
{
    public class ChartmakerMultiHandlerColor: ChartmakerLerpableMultiHandler<Color>
    {
        public int Axis = 0;

        public override ExpressionContext GetExpressionContext()
        {
            return new ExpressionContext().SetVariable("r", 0).SetVariable("g", 0).SetVariable("b", 0).SetVariable("a", 0).SetVariable("i", 0).SetVariable("t", 0);
        }

        public override Color Get(Color from, object source, int index = 0) {
            float to = LerpField == null 
                ? LerpTo : Mathf.InverseLerp(LerpFrom, LerpTo, 
                    LerpField.FieldType == typeof(BeatPosition) ? (BeatPosition)LerpField.GetValue(source) : (float)LerpField.GetValue(source));
        
            if (Operation == LerpableOperation.Expression)
            {
                CustomExpressionContext
                    .SetVariable("r", from.r)
                    .SetVariable("g", from.g)
                    .SetVariable("b", from.b)
                    .SetVariable("a", from.a)
                    .SetVariable("i", index)
                    .SetVariable("t", to)
                ;
                from[Axis] = LerpableOperations.Get[Operation](from[Axis], to);
            }
            else
            {
                to = float.IsFinite(From) ? Mathf.Lerp(From, To, LerpEasing.Get(to)) : To;

                from[Axis] = LerpableOperations.Get[Operation](from[Axis], to);
            }
        
            return from;
        }
    }
}