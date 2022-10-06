using UnityEngine;

/// <summary>
/// 작성자 - 차영철
/// </summary>
[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public enum WaveStyle
    {
        Sin,
        Tri,
        Sqr,
        Saw,
        Inv,
        Noise,
        None
    }

    private delegate float WaveStyleDelegate(float x);

    private static WaveStyleDelegate[] waveStyles =
    {
        x => Mathf.Sin(x * 2 * Mathf.PI),                         // Sin
        x => x < 0.5 ? 4.0f * x - 1.0f : -4.0f * x + 3.0f,          // Tri
        x => x < 0.5 ? 1.0f : -1.0f,                                // Sqr
        x => x,                                                     // Saw
        x => 1.0f - x,                                              // Inv
        x => 1 - (UnityEngine.Random.value * 2),                    // Noise
        x => 1.0f
};

    [SerializeField, Tooltip("깜빡이는 방식")]
    private WaveStyle waveStyle;

    [SerializeField, Tooltip("시작값")]
    private float startValue = 0.0f;

    [SerializeField, Tooltip("amplitude of the wave")]
    private float amplitude = 1.0f;

    [SerializeField, Tooltip("start point inside on wave cycle")]
    private float phase = 0.0f;

    [SerializeField, Tooltip("cycle frequency per second")]
    private float frequency = 0.5f;

    private Color baseColor;

    private new Light light;


    private void Awake()
    {
        light = GetComponent<Light>();
    }

    void Update()
    {
        light.color = baseColor * EvalWave();
    }

    public void UpdateColor(Color color)
    {
        baseColor = color;
    }

    private float EvalWave()
    {
        float x = (Time.time + phase) * frequency;
        x = x - Mathf.Floor(x); // normalized value (0..1)

        return (waveStyles[(int)waveStyle](x) * amplitude) + startValue;
    }
}
