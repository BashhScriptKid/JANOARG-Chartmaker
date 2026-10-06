using System.Collections.Generic;
using JANOARG.Chartmaker.Behaviors.Chartmaker;
using JANOARG.Chartmaker.UI.Modal;
using JANOARG.Chartmaker.UI.Modal.ModalTypes;
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
        public TMP_Text AllocatedMemoryLabel;
        public TMP_Text ReservedMemoryLabel;
        public TMP_Text MonoMemoryLabel;
        public Graphic  MemoryGraph;

        List<float> AllocatedMemory = new();
        List<float> ReservedMemory  = new();
        List<float> MonoMemory      = new();

        Material FPSGraphMaterial, MemoryGraphMaterial;

        void Start()
        {
            FPSGraphMaterial = new Material(FPSGraph.material);
            MemoryGraphMaterial = new Material(MemoryGraph.material);
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

        // Update is called once per frame
        void Update()
        {
            UpdateClock += Time.unscaledDeltaTime;
            UpdateMin = Mathf.Max(UpdateMin, Time.unscaledDeltaTime);
            UpdateMax = Mathf.Min(UpdateMax, Time.unscaledDeltaTime);
            UpdateFrames++;

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
            

                Push(AllocatedMemory, Profiler.GetTotalAllocatedMemoryLong() / 1048576f);
                Push(ReservedMemory, Profiler.GetTotalReservedMemoryLong() / 1048576f);
                Push(MonoMemory, Profiler.GetMonoUsedSizeLong() / 1048576f);
                float memheight = Mathf.Max(ReservedMemory.ToArray()); 
                bool memValid = memheight > 0.001f;
                float[] memall = new float[64], memres = new float[64], memmono = new float[64];
                if (memValid)
                {
                    FillSeries(AllocatedMemory, memall, memheight);
                    FillSeries(ReservedMemory, memres, memheight);
                    FillSeries(MonoMemory, memmono, memheight);
                }
                else
                {
                    for (int a = 0; a < SeriesLength; a++) memall[a] = memres[a] = memmono[a] = -1e6f;
                }
                MemoryGraphMaterial.SetFloat("_CutoffThreshold", cutoffThres);
                MemoryGraphMaterial.SetFloatArray("_Values1", memall);
                MemoryGraphMaterial.SetFloatArray("_Values2", memmono);
                MemoryGraphMaterial.SetFloatArray("_Values3", memres);
            
                MemoryGraph.material = MemoryGraphMaterial;
                MemoryGraph.SetMaterialDirty();
            
                AllocatedMemoryLabel.text = memValid ? AllocatedMemory[^1].ToString("0.0") : "N/A";
                ReservedMemoryLabel.text = memValid ? ReservedMemory[^1].ToString("0.0") : "N/A";
                MonoMemoryLabel.text = memValid ? MonoMemory[^1].ToString("0.0") : "N/A";
            }
        }
    
        public void OpenLogger()
        {
            ModalHolder.main.Spawn<LoggerModal>();
        }
    }
}
