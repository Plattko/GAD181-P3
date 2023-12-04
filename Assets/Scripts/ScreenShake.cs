using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Start()
    {
        virtualCamera = this.gameObject.GetComponent<CinemachineVirtualCamera>();
        
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        StartCoroutine(StopShake(0));
    }

    public void ShakeCamera(float intensity, float duration)
    {
        Debug.Log("Shake Camera");
        if (noise != null)
        {
            noise.m_AmplitudeGain = intensity;
            StartCoroutine(StopShake(duration));
        }
    }

    private IEnumerator StopShake(float duration)
    {
        yield return new WaitForSeconds(duration);
        noise.m_AmplitudeGain = 0;
    }
}
