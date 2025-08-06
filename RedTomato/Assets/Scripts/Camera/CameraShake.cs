using System.Collections;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineBasicMultiChannelPerlin noise;
    private float defaultAmplitude;
    private float defaultFrequency;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        Instance = this;

        // 1) VirtualCamera component’ini al
        var vcam = GetComponent<CinemachineVirtualCamera>();
        // 2) Noise extension’ını al (Noise slot’u üzerinden)
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogWarning("Noise slot’una atanan Cinemachine Basic Multi-Channel Perlin bulunamadı!");
            return;
        }

        // Mevcut değerleri sakla
        defaultAmplitude = noise.m_AmplitudeGain;
        defaultFrequency = noise.m_FrequencyGain;
    }

    public void Shake(float amplitude = 3f, float duration = 0.2f)
    {
        if (noise == null) return;
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(DoShake(amplitude, duration));
    }

    private IEnumerator DoShake(float amplitude, float duration)
    {
        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = defaultFrequency; // veya istediğin shakeFrequency
        yield return new WaitForSeconds(duration);
        noise.m_AmplitudeGain = defaultAmplitude;
        noise.m_FrequencyGain = defaultFrequency;
    }
}
