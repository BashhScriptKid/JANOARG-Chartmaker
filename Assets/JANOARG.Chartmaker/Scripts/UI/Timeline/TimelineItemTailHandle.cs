using System.Collections;
using System.Collections.Generic;
using JANOARG.Chartmaker.Behaviors.Chartmaker;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JANOARG.Chartmaker.UI.Timeline
{
    /// <summary>
    /// The dragger sitting on the right edge of a timeline tail. Lives on its own game object so the press is only
    /// caught on the dragger itself - a handler on the tail root would swallow every press across the tail's body.
    /// </summary>
    public class TimelineItemTailHandle : MonoBehaviour, IPointerDownHandler
    {
        /// <summary>
        /// The item this dragger currently resizes, or null when the tail it belongs to is drawn by a mode that
        /// has no dragging behaviour yet. Reassigned every frame by the pool, since tails are shared across modes.
        /// </summary>
        public object Item;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || Item == null)
                return;

            IList list = InspectorPanel.main.CurrentObject is IList currentObjectList && currentObjectList.Contains(Item)
                ? currentObjectList : null;

            TimelinePanel.main.BeginDragHoldLength(list ?? new List<object> { Item }, eventData);
        }
    }
}
