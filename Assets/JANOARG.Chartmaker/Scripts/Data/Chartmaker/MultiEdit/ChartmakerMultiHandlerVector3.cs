using JANOARG.Chartmaker.Utils.Math;
using JANOARG.Shared.Data.ChartInfo;
using UnityEngine;

namespace JANOARG.Chartmaker.Data.Chartmaker.MultiEdit
{
    public class ChartmakerMultiHandlerVector3: ChartmakerLerpableMultiHandler<Vector3>
    {
        public int Axis = 0;

        public override ExpressionContext GetExpressionContext()
        {
            return new ExpressionContext().SetVariable("x", 0).SetVariable("y", 0).SetVariable("z", 0).SetVariable("i", 0).SetVariable("t", 0);
        }

        public override Vector3 Get(Vector3 from, object source, int index = 0) {
            float to = LerpField == null 
                ? LerpTo : Mathf.InverseLerp(LerpFrom, LerpTo, 
                    LerpField.FieldType == typeof(BeatPosition) ? (BeatPosition)LerpField.GetValue(source) : (float)LerpField.GetValue(source));
        
            if (Operation == LerpableOperation.Expression)
            {
                CustomExpressionContext
                    .SetVariable("x", from.x)
                    .SetVariable("y", from.y)
                    .SetVariable("z", from.z)
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