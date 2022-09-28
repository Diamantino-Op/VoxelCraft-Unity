using Mirror;
using UnityEngine;
using VoxelCraft.Input;

namespace VoxelCraft
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Vector2 cameraVelocity = new Vector2(4f, 4f);
        [SerializeField] private Transform playerTransform = null;
        [SerializeField] private GameObject playerCamera = null;

        private PlayerControls playerControls;
        private PlayerControls PlayerControls
        {
            get
            {
                if (playerControls != null) { return playerControls; }
                return playerControls = new PlayerControls();
            }
        }

        public override void OnStartAuthority()
        {
            playerCamera.SetActive(true);

            enabled = true;

            PlayerControls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());

            Cursor.lockState = CursorLockMode.Locked;
        }

        [ClientCallback]
        private void OnEnable() => PlayerControls.Enable();
        [ClientCallback]
        private void OnDisable() => PlayerControls.Disable();

        private void Look(Vector2 lookAxis)
        {
            playerTransform.Rotate(0f, lookAxis.x * cameraVelocity.x, 0f);

            playerCamera.transform.Rotate(-lookAxis.y * cameraVelocity.y, 0f, 0f);

            if (playerCamera.transform.rotation.x > 90f)
            {
                float diffX = playerCamera.transform.rotation.x - 90f;

                playerCamera.transform.Rotate(-diffX, 0, 0);
            }
            else if (playerCamera.transform.rotation.x < -90f)
            {
                float diffX = playerCamera.transform.rotation.x + 90f;

                playerCamera.transform.Rotate(diffX, 0, 0);
            }
        }
    }
}
