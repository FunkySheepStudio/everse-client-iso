using UnityEngine;

namespace FunkySheep.Player.Controller
{
    [AddComponentMenu("FunkySheep/Player/Controller/Movements")]
    public class Movements : MonoBehaviour
    {
        public float speed = 10;
        CharacterController controller;

        PlayerInputs playerInputs;
        private void Awake()
        {
            playerInputs = new PlayerInputs();
            controller = GetComponent<CharacterController>();
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
            Vector2 movement = playerInputs.Movements.Move.ReadValue<Vector2>();
            controller.Move((Vector3.forward * movement.y + Vector3.right * movement.x) * UnityEngine.Time.deltaTime * speed);
        }

    }
}
