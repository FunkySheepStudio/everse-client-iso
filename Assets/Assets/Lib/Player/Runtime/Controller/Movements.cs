using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Player.Controller
{
    [AddComponentMenu("FunkySheep/Player/Controller/Movements")]
    public class Movements : MonoBehaviour
    {
        public float speed = 100;
        public float gyroSpeed = 500;
        public float gyroRotationSpeed = 1000;
        CharacterController controller;

        PlayerInputs playerInputs;
        float initialYAcceleration;
        Vector3 initialGyroRotation;

        private void Awake()
        {
            playerInputs = new PlayerInputs();
            controller = GetComponent<CharacterController>();
            Input.gyro.enabled = true;
            initialYAcceleration = Input.acceleration.y;
        }

        private void Start()
        {
            playerInputs.Movements.Enable();
        }

        private void Update()
        {
            Move();
        }

        void Move()
        {
            // Move keyboard
            Vector2 movement = playerInputs.Movements.Move.ReadValue<Vector2>();
            controller.Move((Vector3.forward * movement.y + Vector3.right * movement.x) * UnityEngine.Time.deltaTime * speed);

            // Move gyro
            if (math.abs(Input.acceleration.y - initialYAcceleration) > 0.1)
            {
                controller.Move(transform.forward * (Input.acceleration.y - initialYAcceleration) * UnityEngine.Time.deltaTime * gyroSpeed);
            }

            //Rotate Gyro
            if (math.abs(Input.acceleration.x) > 0.1)
            {
                transform.Rotate(Vector3.up * Input.acceleration.x * UnityEngine.Time.deltaTime * gyroSpeed);
            }
        }

    }
}
