using UnityEngine;

public class FloorHighlightTemp_TileHighlight_IsolatedTestBed : MonoBehaviour
{
    [Header("Highlight Settings")]
    [SerializeField] private Color highlightColor = new Color(0.5f, 0f, 1f, 1f); // Purple
    [SerializeField] private Color rimLightColor = new Color(1f, 0.5f, 0f); // Orange glow
    [SerializeField] private float rimPower = 3f;

    private Renderer objectRenderer;
    private Material material;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        
        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer found on this GameObject!");
            return;
        }

        material = objectRenderer.material;

        // Print diagnostic info
        Debug.Log($"Shader: {material.shader.name}");
        PrintAllProperties();

        // Try common toon shader properties
        TrySetProperty("_Color", highlightColor);
        TrySetProperty("_BaseColor", highlightColor);
        TrySetProperty("_MainColor", highlightColor);
        TrySetProperty("_TintColor", highlightColor);
        
        // Toon shaders often use rim lighting for glow instead of emission
        TrySetProperty("_RimColor", rimLightColor);
        TrySetProperty("_RimLightColor", rimLightColor);
        TrySetFloat("_RimPower", rimPower);
        TrySetFloat("_RimAmount", 0.7f);
        
        // Some toon shaders use outline color
        TrySetProperty("_OutlineColor", rimLightColor);
        TrySetFloat("_OutlineWidth", 0.1f);

        Debug.Log("Highlight applied!");
    }

    private void TrySetProperty(string propertyName, Color value)
    {
        if (material.HasProperty(propertyName))
        {
            material.SetColor(propertyName, value);
            Debug.Log($" Set {propertyName} to {value}");
        }
    }

    private void TrySetFloat(string propertyName, float value)
    {
        if (material.HasProperty(propertyName))
        {
            material.SetFloat(propertyName, value);
            Debug.Log($" Set {propertyName} to {value}");
        }
    }

    private void PrintAllProperties()
    {
        Debug.Log("=== Material Properties ===");
        Shader shader = material.shader;
        int propertyCount = shader.GetPropertyCount();
        
        for (int i = 0; i < propertyCount; i++)
        {
            string propName = shader.GetPropertyName(i);
            var propType = shader.GetPropertyType(i);
            Debug.Log($"{propName} ({propType})");
        }
    }

    private void OnDestroy()
    {
        if (material != null)
        {
            Destroy(material);
        }
    }
}
