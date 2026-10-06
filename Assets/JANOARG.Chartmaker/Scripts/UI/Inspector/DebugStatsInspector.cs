using System.Collections.Generic;
using JANOARG.Chartmaker.Behaviors.Chartmaker;
using JANOARG.Chartmaker.UI.Modal;
using JANOARG.Chartmaker.UI.Modal.ModalTypes;
using JANOARG.Chartmaker.UI.Tooltip;
using JANOARG.Chartmaker.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace JANOARG.Chartmaker.UI.Inspector
{
    public class DebugStatsInspector : MonoBehaviour
    {
        public TMP_Text LaneCountLabel;
        public TMP_Text HitCountLabel;
        public TMP_Text VertsLabel;
        public TMP_Text TrisLabel;

        public TMP_Text FPSLabel;
        public TMP_Text MSLabel;
        public TMP_Text MinFPSLabel;
        public TMP_Text MaxFPSLabel;
        public TMP_Text AverageFPSLabel;
        public Graphic  FPSGraph;

        float UpdateClock  = 0;
        float UpdateMin    = 0;
        float UpdateMax    = float.PositiveInfinity;
        int   UpdateFrames = 0;

        List<float> FrameHistory = new();
        List<float> FrameMin     = new();
        List<float> FrameMax     = new();
    
        [Space]
        public TMP_Text TotalMemoryLabel;
        public TMP_Text ReservedMemoryLabel;
        public TMP_Text AllocatedMemoryLabel;
        public TMP_Text ManagedMemoryLabel;
        public Graphic  MemoryGraph;

        List<float> TotalMemory     = new();
        List<float> ReservedMemory  = new();
        List<float> AllocatedMemory = new();
        List<float> ManagedMemory   = new();

        float MemoryScale;

        float PeakTotal, PeakReserved, PeakAllocated, PeakManaged;

        Material FPSGraphMaterial, MemoryGraphMaterial;

        void Start()
        {
            FPSGraphMaterial = new Material(FPSGraph.material);
            MemoryGraphMaterial = new Material(MemoryGraph.material);

            AddMemoryTooltip(TotalMemoryLabel);
            AddMemoryTooltip(ReservedMemoryLabel);
            AddMemoryTooltip(AllocatedMemoryLabel);
            AddMemoryTooltip(ManagedMemoryLabel);
        }

        // The memory labels are laid out with zero-height rects, so they have no
        // raycast area. Add an explicit transparent hit box (covering the value and
        // caption) and reuse the tooltip text authored on the caption in the prefab.
        static void AddMemoryTooltip(TMP_Text valueLabel)
        {
            if (valueLabel == null) return;
            Transform parent = valueLabel.transform.parent;
            if (parent == null) return;

            Transform caption = parent.Find("Label") ?? parent.Find("Total Label");
            TooltipTarget source = caption != null ? caption.GetComponent<TooltipTarget>() : null;
            if (source == null) return;

            if (valueLabel.transform.Find("Memory Tooltip Hitbox") != null) return;

            GameObject go = new("Memory Tooltip Hitbox", typeof(RectTransform));
            RectTransform rt = (RectTransform)go.transform;
            rt.SetParent(valueLabel.transform, false);
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(1, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(60, 20);
            rt.anchoredPosition = new Vector2(-30, 0);

            Image image = go.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
            image.raycastTarget = true;

            TooltipTarget tooltip = go.AddComponent<TooltipTarget>();
            tooltip.Text = source.Text;
            tooltip.Delay = 0.2f;
            tooltip.PositionMode = TooltipPositionMode.Cursor;
        }

        void OnDestroy()
        {
            Destroy(FPSGraphMaterial);
            Destroy(MemoryGraphMaterial);
        }

        const int SeriesLength = 64;

        static void Push(List<float> series, float value)
        {
            series.Add(value);
            while (series.Count > SeriesLength) series.RemoveAt(0);
        }

        static void FillSeries(List<float> series, float[] target, float scale)
        {
            int count = series.Count;
            for (int a = 0; a < SeriesLength; a++)
            {
                int i = count - SeriesLength + a;
                target[a] = i >= 0 ? series[i] / scale : -1e6f;
            }
        }

        static string FormatMemory(float value, bool valid)
        {
            return valid && value > 0 ? value.ToString("0.0") : "N/A";
        }

        void SampleMemoryPeaks()
        {
            bool profilerValid = Profiler.supported;

            if (ProcessMemory.TryGet(out long workingSet, out _))
                PeakTotal = Mathf.Max(PeakTotal, workingSet / 1048576f);

            if (profilerValid)
            {
                PeakReserved  = Mathf.Max(PeakReserved,  Profiler.GetTotalReservedMemoryLong()  / 1048576f);
                PeakAllocated = Mathf.Max(PeakAllocated, Profiler.GetTotalAllocatedMemoryLong() / 1048576f);
                PeakManaged   = Mathf.Max(PeakManaged,   Profiler.GetMonoUsedSizeLong()          / 1048576f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateClock += Time.unscaledDeltaTime;
            UpdateMin = Mathf.Max(UpdateMin, Time.unscaledDeltaTime);
            UpdateMax = Mathf.Min(UpdateMax, Time.unscaledDeltaTime);
            UpdateFrames++;

            SampleMemoryPeaks();

            float cutoffThres = 108 - WindowHandler.main.NavBar.anchoredPosition.y;

            if (UpdateClock > 0.1f) 
            {
                LaneCountLabel.text = PlayerView.main.Manager?.ActiveLaneCount.ToString() ?? "-";
                HitCountLabel.text = PlayerView.main.Manager?.ActiveHitCount.ToString() ?? "-";
                VertsLabel.text = PlayerView.main.Manager?.ActiveLaneVerts.ToString() ?? "-";
                TrisLabel.text = PlayerView.main.Manager?.ActiveLaneTris.ToString() ?? "-";

                float msAvg = UpdateClock / UpdateFrames;
                UpdateClock = UpdateFrames = 0;

                FPSLabel.text = (1 / msAvg).ToString("0.0");
                MSLabel.text = (msAvg * 1000).ToString("0.00");

                Push(FrameHistory, msAvg);
                Push(FrameMin, UpdateMin);
                Push(FrameMax, UpdateMax);
                float fpsheight = Mathf.Max(FrameMin.ToArray()); 
                float[] fpslist = new float[64], fpsmin = new float[64], fpsmax = new float[64];
                FillSeries(FrameHistory, fpslist, fpsheight);
                FillSeries(FrameMin, fpsmin, fpsheight);
                FillSeries(FrameMax, fpsmax, fpsheight);
                float fpsSum = 0;
                int frameCount = FrameHistory.Count;
                for (int a = 0; a < SeriesLength; a++) 
                {
                    int i = frameCount - SeriesLength + a;
                    if (i >= 0) fpsSum += FrameHistory[i];
                }
                FPSGraphMaterial.SetFloat("_CutoffThreshold", cutoffThres);
                FPSGraphMaterial.SetFloatArray("_Values", fpslist);
                FPSGraphMaterial.SetFloatArray("_ValuesMin", fpsmin);
                FPSGraphMaterial.SetFloatArray("_ValuesMax", fpsmax);
                FPSGraphMaterial.SetVector("_Resolution", new(FPSGraph.rectTransform.rect.width, FPSGraph.rectTransform.rect.height));
            
                FPSGraph.material = FPSGraphMaterial;
                FPSGraph.SetMaterialDirty();

                MinFPSLabel.text = (1 / Mathf.Max(FrameMin.ToArray())).ToString("0.0");
                MaxFPSLabel.text = (1 / Mathf.Min(FrameMax.ToArray())).ToString("0.0");
                AverageFPSLabel.text = (1 / fpsSum * FrameHistory.Count).ToString("0.0");
            
                UpdateMax = float.PositiveInfinity;
                UpdateMin = 0;
            

                bool profilerValid = Profiler.supported;

                Push(TotalMemory, PeakTotal);
                Push(ReservedMemory, PeakReserved);
                Push(AllocatedMemory, PeakAllocated);
                Push(ManagedMemory, PeakManaged);

                PeakTotal = PeakReserved = PeakAllocated = PeakManaged = 0;

                float memMax = Mathf.Max(
                    Mathf.Max(TotalMemory.ToArray()),
                    Mathf.Max(ReservedMemory.ToArray()),
                    Mathf.Max(AllocatedMemory.ToArray()),
                    Mathf.Max(ManagedMemory.ToArray()));
                MemoryScale = Mathf.Max(MemoryScale, Mathf.Ceil(memMax / 64f) * 64f);
                bool memValid = MemoryScale > 0.001f;
                float[] memTotal = new float[64], memres = new float[64], memall = new float[64], memman = new float[64];
                if (memValid)
                {
                    FillSeries(TotalMemory, memTotal, MemoryScale);
                    FillSeries(ReservedMemory, memres, MemoryScale);
                    FillSeries(AllocatedMemory, memall, MemoryScale);
                    FillSeries(ManagedMemory, memman, MemoryScale);
                }
                else
                {
                    for (int a = 0; a < SeriesLength; a++) memTotal[a] = memres[a] = memall[a] = memman[a] = -1e6f;
                }
                MemoryGraphMaterial.SetFloat("_CutoffThreshold", cutoffThres);
                MemoryGraphMaterial.SetFloatArray("_Values1", memTotal);
                MemoryGraphMaterial.SetFloatArray("_Values2", memres);
                MemoryGraphMaterial.SetFloatArray("_Values3", memall);
                MemoryGraphMaterial.SetFloatArray("_Values4", memman);
            
                MemoryGraph.material = MemoryGraphMaterial;
                MemoryGraph.SetMaterialDirty();
            
                TotalMemoryLabel.text     = FormatMemory(TotalMemory[^1], true);
                ReservedMemoryLabel.text  = FormatMemory(ReservedMemory[^1], profilerValid);
                AllocatedMemoryLabel.text = FormatMemory(AllocatedMemory[^1], profilerValid);
                ManagedMemoryLabel.text   = FormatMemory(ManagedMemory[^1], profilerValid);
            }
        }
    
        public void OpenLogger()
        {
            ModalHolder.main.Spawn<LoggerModal>();
        }
    }
}
