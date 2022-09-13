using UnityEngine;

namespace VoxelCraft.Entity
{
    public class Player : Entity
    {
        public Transform camera;
        private World.World world;

        public float baseWalkSpeed;
        public float speedMultiplier;
        public float basicJumpForce;
        public float baseGravity = -9.8f;

        private float horizontal;
        private float vertical;
        private float mouseHorizontal;
        private float mouseVertical;
        private Vector3 velocity;

        public void Start()
        {
            
        }

        public void Update()
        {
            GetPlayerInputs();

            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.deltaTime * (baseWalkSpeed * speedMultiplier);
            velocity += Vector3.up * baseGravity * Time.deltaTime;

            transform.Rotate(Vector3.up * mouseHorizontal);
            camera.Rotate(Vector3.right * -mouseVertical);
            transform.Translate(velocity, Space.World);
        }

        private void GetPlayerInputs()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            mouseHorizontal = Input.GetAxis("Mouse X");
            mouseVertical = Input.GetAxis("Mouse Y");
        }
    }
}
