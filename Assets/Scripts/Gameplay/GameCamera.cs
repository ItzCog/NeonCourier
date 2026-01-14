using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class GameCamera : MonoBehaviour
{
    public static GameCamera Instance { get; private set; }

    private CinemachineBasicMultiChannelPerlin _noise;

    private void Awake()
    {
        _noise = GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _noise.m_AmplitudeGain = 0f;
        Instance = this;
    }

    public void Shake(float intensity, float duration)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        _noise.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        _noise.m_AmplitudeGain = 0f;
    }
}
