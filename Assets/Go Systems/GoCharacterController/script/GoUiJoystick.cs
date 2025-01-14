using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GoSystem.Control;
namespace GoSystem
{
    [GBehaviourAttributeAttribute("UI Joystick", false)]
    public class GoUiJoystick : GoSystemsBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        public Image joystickBackground;
        public Image joystickKnob;

        private Vector3 inputVector;

        private void Start()
        {
            // Initialize the joystick position to the center.
            joystickKnob.rectTransform.anchoredPosition = Vector3.zero;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector3.zero;
            joystickKnob.rectTransform.anchoredPosition = Vector3.zero;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
            {
                pos.x = (pos.x / joystickBackground.rectTransform.sizeDelta.x);
                pos.y = (pos.y / joystickBackground.rectTransform.sizeDelta.y);

                inputVector = new Vector3(pos.x * 2 + 1, 0, pos.y * 2 - 1);
                inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
                // Move the joystick knob.
                joystickKnob.rectTransform.anchoredPosition = new Vector3(inputVector.x * (joystickBackground.rectTransform.sizeDelta.x / 3), inputVector.z * (joystickBackground.rectTransform.sizeDelta.y / 3));
            }
        }

        public float Horizontal()
        {
            return inputVector.x;
        }

        public float Vertical()
        {
            return inputVector.z;
        }
    }
}