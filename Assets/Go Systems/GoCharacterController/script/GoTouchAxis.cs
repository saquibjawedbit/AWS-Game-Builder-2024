using UnityEngine;
using UnityEngine.EventSystems;
namespace GoSystem
{
    [GoSystem.GBehaviourAttribute("Touch Space",false)]
    public class GoTouchAxis : GoSystemsBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [HideInInspector]
        public Vector3 newCameraAxis;
        private Vector2 TouchCoord;
        private bool isTouching = false;
        private Vector2 lastTouchPosition;

        void Update()
        {
            if (isTouching)
            {
                Vector2 touchPosition = Input.mousePosition;
                touchPosition.x /= Screen.width;
                touchPosition.y /= Screen.height;
                TouchCoord = touchPosition;
                newCameraAxis = new Vector3(lastTouchPosition.x + TouchCoord.x, 0, lastTouchPosition.y + TouchCoord.y);

            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isTouching = true;
            TouchCoord = lastTouchPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isTouching = false;
            lastTouchPosition = TouchCoord;
        }
    }
}
