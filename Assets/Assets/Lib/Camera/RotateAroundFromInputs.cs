using UnityEngine;
using Cinemachine;
using UnityEditor.ShaderGraph.Internal;

namespace FunkySheep.Cam
{
    [AddComponentMenu("FunkySheep/Camera/Rotate around from inputs")]
    public class RotateAroundFromInputs : MonoBehaviour
    {
        public PlayerInputs playerInputs;
        CinemachineFreeLook _cinemachineFreeLook;
        public GameObject _target;

        private void Awake()
        {
            playerInputs = new PlayerInputs();
            _cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        }

        private void Update()
        {
            SetTarget(_target);
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
            _cinemachineFreeLook.Follow = _target.transform;
            _cinemachineFreeLook.LookAt = _target.transform;

            Mesh mesh = target.GetComponent<MeshFilter>().mesh;

            float orthosize = Mathf.Max(mesh.bounds.max.x * _target.transform.localScale.x, mesh.bounds.max.z * _target.transform.localScale.z);

            _cinemachineFreeLook.m_Orbits[0].m_Radius = orthosize * 2;
            _cinemachineFreeLook.m_Orbits[1].m_Radius = orthosize * 2;
            _cinemachineFreeLook.m_Orbits[2].m_Radius = orthosize * 2;

            _cinemachineFreeLook.m_Orbits[0].m_Height = mesh.bounds.max.y * _target.transform.localScale.y * 4;
            _cinemachineFreeLook.m_Orbits[1].m_Height = mesh.bounds.center.y * _target.transform.localScale.y * 2;
            _cinemachineFreeLook.m_Orbits[2].m_Height = mesh.bounds.min.y * _target.transform.localScale.y;

            _cinemachineFreeLook.m_Lens.OrthographicSize = orthosize;
        }
    }
}
