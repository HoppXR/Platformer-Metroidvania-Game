using Cinemachine;
using Player.Input;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private CinemachineFreeLook freeLookCam;

    [SerializeField] private float mouseSens = 1f;
    [SerializeField] private float controllerSens = 2f;
    
    private string _mouseXAxis = "Mouse X";
    private string _mouseYAxis = "Mouse Y";

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
        if (freeLookCam == null) return;
        
        float mouseX = Input.GetAxis(_mouseXAxis) * mouseSens;
        float mouseY = Input.GetAxis(_mouseYAxis) * mouseSens;
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

    private void HandleInput(Vector2 dir)
    {
        _inputValue = dir;
    }
}
