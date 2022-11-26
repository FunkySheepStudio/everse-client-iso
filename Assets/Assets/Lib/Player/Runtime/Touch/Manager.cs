using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FunkySheep.Player.Touch
{
    [AddComponentMenu("FunkySheep/Player/Touch Manager")]
    public class Manager : MonoBehaviour
    {
        PlayerInputs playerInputs;

        private void Awake()
        {
            playerInputs = new PlayerInputs();
        }

        private void Start()
        {
            playerInputs.Touch.Hold.performed += ctx => OnScreenTouched(ctx);
            playerInputs.Mouse.Click.performed += ctx => OnScreenLeftClicked(ctx);
        }

        void OnScreenTouched(InputAction.CallbackContext callbackContext)
        {
            CheckWorldCollision(callbackContext.ReadValue<Vector2>());
        }

        void OnScreenLeftClicked(InputAction.CallbackContext callbackContext)
        {
            CheckWorldCollision(Input.mousePosition);
        }

        void CheckWorldCollision(Vector3 position)
        {
            Ray ray = Camera.main.ScreenPointToRay(position, Camera.MonoOrStereoscopicEye.Mono);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.gameObject.layer == 10)
            {
                SceneManager.LoadSceneAsync("Assets/Data/Buildings/Editor", LoadSceneMode.Additive);
            }
        }

        private void OnEnable()
        {
            playerInputs.Enable();
        }

        private void OnDisable()
        {
            playerInputs.Disable();
        }
    }
}

