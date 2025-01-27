using UnityEngine;

namespace Platformer
{
    public class ThirdPersonCam : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform player;
        [SerializeField] private Transform playerObj;

        [SerializeField] private float rotationSpeed;

        [SerializeField] private Transform combatLookAt;

        [SerializeField] private GameObject thirdPersonCam;
        [SerializeField] private GameObject combatCam;
        [SerializeField] private GameObject topDownCam;
        
        public CameraStyle currentStyle;

        public enum CameraStyle
        {
            Basic,
            Combat,
            Topdown
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // change cam style (remove later)
            if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Combat);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCameraStyle(CameraStyle.Topdown);
            
            // rotate orientation
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;
            
            // rotate player object
            if (currentStyle is CameraStyle.Basic or CameraStyle.Topdown)
            {
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

                if (inputDir != Vector3.zero)
                    playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
            
            else if (currentStyle == CameraStyle.Combat)
            {
                Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
                orientation.forward = dirToCombatLookAt.normalized;

                playerObj.forward = dirToCombatLookAt.normalized;
            }
        }

        private void SwitchCameraStyle(CameraStyle newStyle)
        {
            combatCam.SetActive(false);
            thirdPersonCam.SetActive(false);
            topDownCam.SetActive(false);
            
            if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
            if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);
            if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);

            currentStyle = newStyle;
        }
    }
}
