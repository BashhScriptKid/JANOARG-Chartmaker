using System.Collections;
using System.Collections.Generic;
using JANOARG.Shared.Data.ChartInfo;

namespace JANOARG.Chartmaker.Data.Chartmaker.Actions
{
    public class ChartmakerTimelineDragHoldLengthAction: IChartmakerAction
    {
        public IList  Targets = new List<object>();
        public string Keyword;
        public float  Value;

        public string GetName() => 
            "Resize " + Behaviors.Chartmaker.Chartmaker.GetItemName(Targets);

        public void Undo() 
        {
            Do(-Value);
        }
        public void Redo() 
        {
            Do(Value);
        }

        void Do(float value) 
        {
            foreach (object item in Targets)
                if (item is HitObject hit)
                    hit.HoldLength += value;
        }
    }
}
