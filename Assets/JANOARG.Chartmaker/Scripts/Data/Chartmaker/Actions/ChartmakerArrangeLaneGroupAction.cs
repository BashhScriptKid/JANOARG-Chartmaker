using System.Collections.Generic;
using System.Linq;
using JANOARG.Shared.Data.ChartInfo;
using JANOARG.Chartmaker.Utils;

namespace JANOARG.Chartmaker.Data.Chartmaker.Actions
{
    public class ChartmakerArrangeLaneGroupAction: IChartmakerAction
    {
        public LaneGroup Target;

        public LaneGroup BeforeAdjacent;
        public ulong     BeforeAdjacentUuid;
        public string    BeforeGroup;
        public LaneGroup AfterAdjacent;
        public ulong     AfterAdjacentUuid;
        public string    AfterGroup;

        public string GetName()
        {
            return "Arrange Lane Group";
        }

        public void Do(LaneGroup adjacent, ulong adjacentUuid, string group) 
        {
            List<LaneGroup> list = Behaviors.Chartmaker.Chartmaker.main.CurrentChart.Groups;
      
            Target.Group = group;
       
            list.Remove(Target);

            int index = adjacent != null
                ? list.IndexOf(adjacent)
                : list.FindIndex(g => g.UUID == adjacentUuid);

            list.Insert(index + 1, Target);
        }

        public void Redo()
        {
            Do(AfterAdjacent, AfterAdjacentUuid, AfterGroup);
        }

        public void Undo()
        {
            Do(BeforeAdjacent, BeforeAdjacentUuid, BeforeGroup);
        }
    }
}