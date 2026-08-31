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
    public class TimelineItemTailHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler
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

            // A dragger belongs to one tail, so a selection only comes along when it holds that tail's item -
            // otherwise grabbing this handle would resize something else entirely, off where the selection is
            IList timestamps = InspectorPanel.main.CurrentTimestamp;
            IList list = timestamps?.Count > 0 && timestamps.Contains(Item)
                ? timestamps
                : InspectorPanel.main.CurrentObject is IList currentObjectList && currentObjectList.Contains(Item)
                    ? currentObjectList : null;

            // Item is the anchor: the pointer snaps this tail's end to the grid, and the rest of the selection
            // moves with it by the same amount
            TimelinePanel.main.BeginDragDuration(list ?? new List<object> { Item }, Item, eventData);
        }

        /// <summary>
        /// Drag events go to the nearest ancestor implementing IDragHandler, and the panel does not reliably win
        /// that resolution from here - a press on this handle opens the drag and then never hears about the
        /// pointer moving. Handling it on the pressed object removes the question: the gesture is delivered where
        /// it started, and the panel's own drag logic runs from there.
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            TimelinePanel.main.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            TimelinePanel.main.EndDragDuration();
        }

        /// <summary>
        /// A press that never crossed the drag threshold gets no OnEndDrag, and the panel's own OnPointerUp never
        /// runs for a press this handle caught - so the drag it opened has to be dropped here or it outlives the
        /// gesture. Unity raises this before OnEndDrag, hence the guard: a real drag still ends the normal way.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            // Pointer-up is delivered to the object that was pressed - this one - whereas end-drag goes to whichever
            // ancestor Unity resolved as the drag handler. Closing the operation here means the gesture that opened
            // it also finishes it. A press that never moved commits nothing, since its delta is still zero.
            TimelinePanel.main.EndDragDuration();
        }
    }
}
