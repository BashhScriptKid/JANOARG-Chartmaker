using System;
using JANOARG.Shared.Data.ChartInfo;
using TMPro;
using UnityEngine;

namespace JANOARG.Chartmaker.UI.Inspector
{
    public class ChartStatsInspector : MonoBehaviour
    {
        public Chart HightlightedChart;

        [Header("Palette")]
        public TMP_Text LaneStyleCount;
        public TMP_Text HitStyleCount;

        [Header( "Overall Lane Stats" )] 
        public TMP_Text LaneCount;
        public TMP_Text LaneGroupCount;
        public TMP_Text LaneStep;

        [Header( "Hit Objects" )]
        public TMP_Text HitObjectCount;
        public TMP_Text HitObjectTapCatchCount;
        public TMP_Text FlickCount;
        public TMP_Text FlickTapCatchCount;
        public TMP_Text FlickDirOmniCount;
        public TMP_Text HoldCount;
        public TMP_Text HoldTapCatchCount;
        public TMP_Text HoldTickCount;

        [Header("Score")]
        public TMP_Text EXScore;
        public TMP_Text MaxStreak;

        void Start()
        {
            UpdateStats();
        }

        void UpdateStats()
        {
            if (HightlightedChart == null)
            {
                LaneStyleCount.text = "-";
                HitStyleCount.text = "-";
                LaneCount.text = "-";
                LaneGroupCount.text = "-";
                LaneStep.text = "-";
                HitObjectCount.text = "-";
                HitObjectTapCatchCount.text = "-";
                FlickCount.text = "-";
                FlickTapCatchCount.text = "-";
                FlickDirOmniCount.text = "-";
                HoldCount.text = "-";
                HoldTapCatchCount.text = "-";
                HoldTickCount.text = "-";
                EXScore.text = "-";
                MaxStreak.text = "-";
                return;
            }

            // Palette
            LaneStyleCount.text = HightlightedChart.Palette.LaneStyles.Count.ToString();
            HitStyleCount.text = HightlightedChart.Palette.HitStyles.Count.ToString();

            // Overall Lane Stats
            LaneCount.text = HightlightedChart.Lanes.Count.ToString();
            LaneGroupCount.text = HightlightedChart.Groups.Count.ToString();

            int laneStepCount = 0;
            int totalHitObjects = 0;
            int taps = 0;
            int catches = 0;
            int tapFlicks = 0;
            int catchFlicks = 0;
            int omniFlicks = 0;
            int dirFlicks = 0;
            int tapHolds = 0;
            int catchHolds = 0;
            int holdTicks = 0;

            foreach (var lane in HightlightedChart.Lanes)
            {
                laneStepCount += lane.LaneSteps.Count; 
                
                var objects = lane.Objects;
                totalHitObjects += objects.Count;

                // Hit Object Count
                foreach (var obj in objects)
                {
                    if (obj.Type is HitObject.HitType.Normal)
                        taps++;
                    else if (obj.Type is HitObject.HitType.Catch)
                        catches++;
        
                    if (obj.Flickable)
                    {
                        if (float.IsFinite(obj.FlickDirection))
                            dirFlicks++;
                        else
                            omniFlicks++;

                        if (obj.Type is HitObject.HitType.Normal)
                            tapFlicks++;
                        else if (obj.Type is HitObject.HitType.Catch)
                            catchFlicks++;
                    }
                    if (obj.HoldLength > 0)
                    {
                        if (obj.Type is HitObject.HitType.Normal)
                            tapHolds++;
                        else if (obj.Type is HitObject.HitType.Catch)
                            catchHolds++;
                        holdTicks += Mathf.CeilToInt(obj.HoldLength / 0.5f);
                    }
                }
            }

            LaneStep.text = laneStepCount.ToString();
            HitObjectCount.text = totalHitObjects.ToString();
            HitObjectTapCatchCount.text =  $"({taps}+{catches})";
            FlickCount.text = (tapFlicks + catchFlicks).ToString();
            FlickTapCatchCount.text =  $"({tapFlicks}+{catchFlicks})";
            FlickDirOmniCount.text =  $"({dirFlicks}+{omniFlicks})";
            HoldCount.text = (tapHolds + catchHolds).ToString();
            HoldTapCatchCount.text = $"({tapHolds}+{catchHolds})";
            HoldTickCount.text = holdTicks.ToString();

            // Score
            // Get Max Streak
            // EX Score 

            EXScore.text = (
                (taps * 3) +
                catches +
                holdTicks +
                omniFlicks +
                (dirFlicks * 2)
            ).ToString();
            
            MaxStreak.text = (totalHitObjects + holdTicks).ToString();
        }
    }
    
}