using UnityEngine;
namespace FunkySheep.Cam
{
    [AddComponentMenu("FunkySheep/Camera/Rotate around from inputs")]
    public class RotateAroundFromInputs : MonoBehaviour
    {
        public GameObject target;
        public PlayerInputs playerInputs;

        private void Awake()
        {
            playerInputs = new PlayerInputs();
        }
    }
}
