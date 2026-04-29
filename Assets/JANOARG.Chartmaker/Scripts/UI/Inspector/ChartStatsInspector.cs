using JANOARG.Shared.Data.ChartInfo;
using TMPro;
using UnityEngine;

namespace JANOARG.Chartmaker.UI.Inspector
{
    public class ChartStatsInspector : MonoBehaviour
    {
        public Chart Chart;

        [Header( "LaneGroup Stats" )]
        public TMP_Text LaneCount;
        public TMP_Text LaneGroupCount;
        public TMP_Text LaneCountRecursive;
        public TMP_Text MaxNestingCount;

        [Header( "Lane Step" )]
        public TMP_Text LaneStep;

        [Header( "Hit Objects" )]
        public TMP_Text TotalHitObjects;
        public TMP_Text Taps;
        public TMP_Text Catches;
        public TMP_Text Flickables;
        public TMP_Text Holds;
        
        // Get Max Streak
        // Get all notes 
            // Normal
            // Hold Ticks
            // Catch
            // Omnidirectional Flicks
            // Directional Flicks
        // EX Score 
        // Get lane count
        // Get lane group count

        private void Update()
        {
            

        }
    }
    
}