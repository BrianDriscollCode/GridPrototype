using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Canvas HealthBarCanvas;
    [SerializeField] private Image fillImage;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Make it face camera
        HealthBarCanvas.transform.forward = cam.forward;
    }

    public void SetHealth(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
}

