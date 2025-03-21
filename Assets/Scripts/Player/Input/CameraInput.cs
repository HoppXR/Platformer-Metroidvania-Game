using Cinemachine;
using UnityEngine;

namespace Player.Input
{
    public class CameraInput : MonoBehaviour
    {
        [SerializeField] private InputReader input;
        [SerializeField] private CinemachineFreeLook freeLookCam;

        [SerializeField] private float mouseSens = 0.25f;
        [SerializeField] private float controllerSens = 4f;

        private Vector2 _inputValue;
    
        private void Awake()
        {
            if (freeLookCam != null)
            {
                freeLookCam.m_XAxis.m_InputAxisName = "";
                freeLookCam.m_YAxis.m_InputAxisName = "";
            }

            input.LookEvent += HandleInput;
        }

        private void Update()
        {
            if (!freeLookCam) return;
        
            float mouseX = _inputValue.x * mouseSens;
            float mouseY = _inputValue.y * mouseSens;
            float controllerX = _inputValue.x * controllerSens;
            float controllerY = _inputValue.y * controllerSens;
        
            bool usingMouse = Mathf.Abs(mouseX) > 0.1f || Mathf.Abs(mouseY) > 0.1f;

            if (usingMouse)
            {
                freeLookCam.m_XAxis.m_InputAxisValue = mouseX;
                freeLookCam.m_YAxis.m_InputAxisValue = mouseY;
            }
            else
            {
                freeLookCam.m_XAxis.m_InputAxisValue = controllerX;
                freeLookCam.m_YAxis.m_InputAxisValue = controllerY;
            }
        }
        
        public void SetSensitivity(float newMouseSens, float newControllerSens)
        {
            mouseSens = newMouseSens;
            controllerSens = newControllerSens;
        }

        private void OnDisable()
        {
            input.LookEvent -= HandleInput;
        }

        private void HandleInput(Vector2 dir)
        {
            _inputValue = dir;
        }
    }
}
