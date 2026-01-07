using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class GameCamera : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin _noise;

    private void Awake()
    {
        _noise = GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _noise.m_AmplitudeGain = 0f;
    }
}
