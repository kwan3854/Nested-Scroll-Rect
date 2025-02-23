using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdvancedScrollRect.Runtime
{
    [RequireComponent(typeof(RectTransform))]
    public class NestedScrollRect : ScrollRect
    {
        public ScrollRect parentScrollRect;

        private bool _shouldRouteToParent;

        private IInitializePotentialDragHandler[] _parentInitializePotentialDragHandlers;
        private IBeginDragHandler[] _parentBeginDragHandlers;
        private IDragHandler[] _parentDragHandlers;
        private IEndDragHandler[] _parentEndDragHandlers;

        protected override void Awake()
        {
            base.Awake();

            // If parentScrollRect is not set, try to find it in the parent hierarchy
            if (parentScrollRect == null)
            {
                parentScrollRect = GetComponentsInParent<ScrollRect>(true)
                    .FirstOrDefault(s => s != this);

                Debug.Assert(parentScrollRect != null, "Parent ScrollRect not found");
            }

            if (parentScrollRect == null) return;
            _parentInitializePotentialDragHandlers = parentScrollRect.GetComponents<IInitializePotentialDragHandler>();
            _parentBeginDragHandlers = parentScrollRect.GetComponents<IBeginDragHandler>();
            _parentDragHandlers = parentScrollRect.GetComponents<IDragHandler>();
            _parentEndDragHandlers = parentScrollRect.GetComponents<IEndDragHandler>();
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            // If the drag is in the same direction as the parent, route the event to the parent
            if (parentScrollRect != null)
            {
                foreach (var initializePotentialDragHandler in _parentInitializePotentialDragHandlers)
                {
                    initializePotentialDragHandler.OnInitializePotentialDrag(eventData);
                }
            }
            base.OnInitializePotentialDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            _shouldRouteToParent = ShouldRouteToParent(eventData);

            if (_shouldRouteToParent && parentScrollRect != null)
            {
                foreach (var beginDragHandler in _parentBeginDragHandlers)
                {
                    beginDragHandler.OnBeginDrag(eventData);
                }
            }
            else
            {
                base.OnBeginDrag(eventData);
            }
        }

        private bool ShouldRouteToParent(PointerEventData eventData)
        {
            // If the drag is not in the same direction as self, route the event to the parent
            return (!horizontal && Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
                   || (!vertical && Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y));
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_shouldRouteToParent && parentScrollRect != null)
            {
                foreach (var dragHandler in _parentDragHandlers)
                {
                    dragHandler.OnDrag(eventData);
                }
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_shouldRouteToParent && parentScrollRect != null)
            {
                foreach (var endDragHandler in _parentEndDragHandlers)
                {
                    endDragHandler.OnEndDrag(eventData);
                }
            }
            else
            {
                base.OnEndDrag(eventData);
            }

            _shouldRouteToParent = false;
        }
    }
}
