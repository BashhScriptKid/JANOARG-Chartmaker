using System.Collections.Generic;
using JANOARG.Shared.Data.ChartInfo;
using JANOARG.Chartmaker.Utils;
using Unity.Profiling;
using UnityEngine;

namespace JANOARG.Chartmaker.Behaviors.Chartmaker
{
    public class ChartmakerLanePlayer : MonoBehaviour
    {
        public LaneManager    CurrentLane;
        public Transform      Holder;
        public MeshRenderer   Renderer;
        public MeshFilter     Filter;
        public MeshCollider   Collider;
        public MeshRenderer   JudgeLine;
        public MeshRenderer[] JudgeEnds;

        public List<ChartmakerHitPlayer> HitPlayers { get; private set; } = new();

        static void SetActiveIfChanged(GameObject go, bool value)
        {
            if (go.activeSelf != value) go.SetActive(value);
        }

        static readonly ProfilerMarker sr_LaneState  = new("LanePlayer: Lane State");
        static readonly ProfilerMarker sr_HitPlayers = new("LanePlayer: Hit Players");

        public void UpdateObjects(LaneManager lane)
        {
            sr_LaneState.Begin();

            CurrentLane = lane;
        
            // Use the lane's own Position/Rotation, not FinalPosition/FinalRotation.
            // The group's transform is now applied by the parent ChartmakerLaneGroupPlayer GO.
            transform.SetLocalPositionAndRotation(
                lane.Current.Position,
                Quaternion.Euler(lane.Current.Rotation)
            );
        
            Holder.localPosition = Vector3.back * lane.CurrentDistance;
        
            List<LaneStyleManager> styles = PlayerView.main.Manager.PalleteManager.LaneStyles;
        
            int index = lane.Current.StyleIndex;
        
            // Kept in a local rather than round-tripped through Collider.enabled: that setter
            // goes through PhysX, and this value is read three times below purely as state.
            bool inRange = lane.Steps.Count >= 2 && PlayerView.main.CurrentTime < lane.Steps[^1].Offset;
            bool visible = inRange && index >= 0 && index < styles.Count;

            if (Collider.enabled != inRange)
                Collider.enabled = inRange;

            if (Renderer.enabled != visible)
                Renderer.enabled = visible;

            Material laneMaterial = visible ? styles[index].LaneMaterial : null;

            if (Renderer.sharedMaterial != laneMaterial)
                Renderer.sharedMaterial = laneMaterial;

            if ((PlayerView.main.MainCamera.activeTexture || !inRange) && Collider.sharedMesh)
                Collider.sharedMesh = null;

            Mesh laneMesh = visible ? lane.CurrentMesh : null;

            if (Filter.sharedMesh != laneMesh)
                Filter.sharedMesh = laneMesh;

            // `visible` first so Steps is known to hold at least two entries before indexing.
            bool judgeVisible = visible
                && PlayerView.main.CurrentTime >= lane.Steps[0].Offset
                && PlayerView.main.CurrentTime < lane.Steps[^1].Offset;

            SetActiveIfChanged(JudgeLine.gameObject,    judgeVisible);
            SetActiveIfChanged(JudgeEnds[0].gameObject, judgeVisible);
            SetActiveIfChanged(JudgeEnds[1].gameObject, judgeVisible);

            if (judgeVisible)
            {
                Material judgeMaterial = styles[index].JudgeMaterial;

                if (JudgeLine.sharedMaterial != judgeMaterial)
                    JudgeLine.sharedMaterial = JudgeEnds[0].sharedMaterial =
                        JudgeEnds[1].sharedMaterial = judgeMaterial;

                JudgeEnds[0].transform.localPosition = lane.StartPosLocal;
                JudgeEnds[1].transform.localPosition = lane.EndPosLocal;

                JudgeLine.transform.localPosition    = (lane.StartPosLocal + lane.EndPosLocal) / 2;
                JudgeLine.transform.localScale       = new (Vector3.Distance(lane.StartPosLocal, lane.EndPosLocal), .05f, .05f);
                JudgeLine.transform.localEulerAngles = Vector3.back * Vector2.SignedAngle(lane.EndPosLocal - lane.StartPosLocal, Vector2.left);
            }
        
            sr_LaneState.End();
            sr_HitPlayers.Begin();

            int count = 0;

            foreach (HitObjectManager hitobject in lane.Objects)
            {
                if (hitobject.TimeEnd < PlayerView.main.CurrentTime) 
                    continue;
            
                if (hitobject.Position.z > lane.CurrentDistance + 250) 
                    break;
            
                if (HitPlayers.Count <= count)
                    HitPlayers.Add(Instantiate(PlayerView.main.HitPlayerSample, Holder));
            
                HitPlayers[count].UpdateObjects(hitobject);
                count++;
            }
        
            while (HitPlayers.Count > count)
            {
                Destroy(HitPlayers[count].gameObject);
                HitPlayers.RemoveAt(count);
            }

            sr_HitPlayers.End();
        }
    }
}