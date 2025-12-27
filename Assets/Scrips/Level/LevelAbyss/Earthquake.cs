using UnityEngine;

public class Earthquake : MonoBehaviour
{
    [Header("Global")]
    [Range(0, 1)] public float intensity = 0f;

    [Header("Camera")]
    public Transform cameraRig;
    public float cameraMoveStrength = 0.4f;
    public float cameraSpeed = 0.3f;

    [Header("Environment")]
    public Transform environmentRoot;
    public float envMoveStrength = 0.05f;

    Vector3 camOrigin;
    Vector3 envOrigin;

    void Start()
    {
        camOrigin = cameraRig.localPosition;
        envOrigin = environmentRoot.localPosition;
    }

    void LateUpdate()
    {
        if (intensity <= 0f)
        {
            cameraRig.localPosition = camOrigin;
            return;
        }

        // ① 低频整体晃动（慢）
        float slowT = Time.unscaledTime * 0.6f;
        Vector3 slowOffset = new Vector3(
            Mathf.PerlinNoise(slowT, 0f) - 0.5f,
            0,
            Mathf.PerlinNoise(0f, slowT) - 0.5f
        );

        // ② 突然冲击（快）
        float fastT = Time.unscaledTime * 8f;
        Vector3 hitOffset = new Vector3(
            Mathf.PerlinNoise(fastT, 10f) - 0.5f,
            0,
            Mathf.PerlinNoise(10f, fastT) - 0.5f
        );

        Vector3 finalOffset =
            slowOffset * 0.6f +
            hitOffset * 0.4f;

        cameraRig.localPosition =
            camOrigin +
            finalOffset * cameraMoveStrength * intensity;
    }
}
