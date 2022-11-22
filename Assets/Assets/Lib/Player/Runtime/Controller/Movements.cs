using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputs;

namespace FunkySheep.Player.Controller
{
    [AddComponentMenu("FunkySheep/Player/Controller/Movements")]
    public class Movements : MonoBehaviour
    {
        public float speed = 10;

        PlayerInputs playerInputs;
        private void Awake()
        {
            playerInputs = new PlayerInputs();
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
            transform.position += (Vector3.forward * movement.y + Vector3.right * movement.x) * Time.deltaTime * speed;
        }

    }
}
