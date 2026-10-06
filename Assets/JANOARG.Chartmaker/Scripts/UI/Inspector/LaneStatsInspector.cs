using System;
using JANOARG.Shared.Data.ChartInfo;
using TMPro;
using UnityEngine;

namespace JANOARG.Chartmaker.UI.Inspector
{
    public class LaneStatsInspector : MonoBehaviour
    {
        public Lane HightlightedLane;
        [Header( "Lane Step" )]
        public TMP_Text LaneStep;
        [Header( "Hit Objects" )]
        public TMP_Text TotalHitObjects;
        public TMP_Text Taps;
        public TMP_Text Catches;
        public TMP_Text Flickables;
        public TMP_Text Holds;
        public TMP_Text FakeNotes;

        void Start()
        {
            UpdateStats();
        }

        void UpdateStats()
        {
            if (HightlightedLane == null)
            {
                LaneStep.text = "-";
                TotalHitObjects.text = "-";
                Taps.text = "-";
                Catches.text = "-";
                Flickables.text = "-";
                Holds.text = "-";
                if (FakeNotes) FakeNotes.text = "-";
                return;
            }

            LaneStep.text = HightlightedLane.LaneSteps.Count.ToString();

            var objects = HightlightedLane.Objects;

            int totalHitObjects = 0;
            int taps = 0;
            int catches = 0;
            int omniFlickables = 0;
            int directionalFlickables = 0;
            int holds = 0;
            int fakeNotes = 0;

            // Single pass through the collection
            foreach (var obj in objects)
            {
                if (obj.IsFake)
                {
                    fakeNotes++;
                    continue;
                }

                totalHitObjects++;

                if (obj.Type is HitObject.HitType.Normal)
                    taps++;
                else if (obj.Type is HitObject.HitType.Catch)
                    catches++;
    
                if (obj.Flickable)
                {
                    if (float.IsFinite(obj.FlickDirection))
                        directionalFlickables++;
                    else
                        omniFlickables++;
                }
    
                if (obj.HoldLength > 0)
                    holds++;
            }

            TotalHitObjects.text = totalHitObjects.ToString();
            Taps.text = taps.ToString();
            Catches.text = catches.ToString();
            Flickables.text = $"{omniFlickables}/{directionalFlickables}";
            Holds.text = holds.ToString();
            if (FakeNotes) FakeNotes.text = fakeNotes.ToString();
        }
    }
}