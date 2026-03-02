using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightGridTile : MonoBehaviour
{
    // Store original properties (before any highlights)
    private Dictionary<GameObject, MaterialProperties> _originalProperties = new Dictionary<GameObject, MaterialProperties>();
    
    // Track active highlight layers per tile
    private Dictionary<GameObject, HashSet<HighlightLayer>> _activeHighlights = new Dictionary<GameObject, HashSet<HighlightLayer>>();

    public enum HighlightType
    {
        MoveRange,      // GridManager movement highlights (lowest priority)
        PathPreview,    // A* path preview (medium priority)
        Hover           // Mouse hover (highest priority)
    }

    private class MaterialProperties
    {
        public Color? color;
        public Color? baseColor;
        public Color? mainColor;
        public Color? tintColor;
        public Color? rimColor;
        public Color? rimLightColor;
        public Color? outlineColor;
        public float? rimPower;
        public float? rimAmount;
        public float? outlineWidth;
    }

    private class HighlightLayer
    {
        public HighlightType type;
        public Color color;
        public Color? rimColor;
        public float? rimPower;
    }

    private void StoreOriginalProperties(GameObject obj, Material material)
    {
        // Only store if not already stored (this is the TRUE original)
        if (_originalProperties.ContainsKey(obj)) return;

        MaterialProperties props = new MaterialProperties();

        // Store color properties
        if (material.HasProperty("_Color"))
            props.color = material.GetColor("_Color");
        if (material.HasProperty("_BaseColor"))
            props.baseColor = material.GetColor("_BaseColor");
        if (material.HasProperty("_MainColor"))
            props.mainColor = material.GetColor("_MainColor");
        if (material.HasProperty("_TintColor"))
            props.tintColor = material.GetColor("_TintColor");

        // Store rim light properties
        if (material.HasProperty("_RimColor"))
            props.rimColor = material.GetColor("_RimColor");
        if (material.HasProperty("_RimLightColor"))
            props.rimLightColor = material.GetColor("_RimLightColor");
        if (material.HasProperty("_RimPower"))
            props.rimPower = material.GetFloat("_RimPower");
        if (material.HasProperty("_RimAmount"))
            props.rimAmount = material.GetFloat("_RimAmount");

        // Store outline properties
        if (material.HasProperty("_OutlineColor"))
            props.outlineColor = material.GetColor("_OutlineColor");
        if (material.HasProperty("_OutlineWidth"))
            props.outlineWidth = material.GetFloat("_OutlineWidth");

        _originalProperties[obj] = props;
    }

    /// <summary>
    /// Highlight a tile with specified type and color
    /// </summary>
    public void HighlightTile(GameObject tile, Color highlightColor, HighlightType type = HighlightType.MoveRange)
    {
        if (tile == null) return;

        Renderer renderer = tile.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning($"HighlightTile: No Renderer on {tile.name}");
            return;
        }

        Material material = renderer.material;

        // Store ORIGINAL properties (only on first highlight)
        StoreOriginalProperties(tile, material);

        // Track this highlight layer
        if (!_activeHighlights.ContainsKey(tile))
            _activeHighlights[tile] = new HashSet<HighlightLayer>();

        // Remove existing highlight of same type
        _activeHighlights[tile].RemoveWhere(h => h.type == type);

        // Add new highlight layer
        _activeHighlights[tile].Add(new HighlightLayer 
        { 
            type = type, 
            color = highlightColor 
        });

        // Apply the highest priority highlight
        ApplyTopHighlight(tile, material);
    }

    /// <summary>
    /// Highlight with rim light (for hover effects)
    /// </summary>
    public void HighlightTileWithRim(GameObject tile, Color highlightColor, Color rimColor, float rimPower, HighlightType type = HighlightType.Hover)
    {
        if (tile == null) return;

        Renderer renderer = tile.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning($"HighlightTile: No Renderer on {tile.name}");
            return;
        }

        Material material = renderer.material;

        // Store ORIGINAL properties (only on first highlight)
        StoreOriginalProperties(tile, material);

        // Track this highlight layer
        if (!_activeHighlights.ContainsKey(tile))
            _activeHighlights[tile] = new HashSet<HighlightLayer>();

        // Remove existing highlight of same type
        _activeHighlights[tile].RemoveWhere(h => h.type == type);

        // Add new highlight layer with rim
        _activeHighlights[tile].Add(new HighlightLayer 
        { 
            type = type, 
            color = highlightColor,
            rimColor = rimColor,
            rimPower = rimPower
        });

        // Apply the highest priority highlight
        ApplyTopHighlight(tile, material);
    }

    /// <summary>
    /// Remove a specific highlight type from a tile
    /// </summary>
    public void RemoveHighlight(GameObject tile, HighlightType type)
    {
        if (tile == null || !_activeHighlights.ContainsKey(tile)) return;

        // Remove the highlight layer of this type
        _activeHighlights[tile].RemoveWhere(h => h.type == type);

        Renderer renderer = tile.GetComponent<Renderer>();
        if (renderer == null) return;

        Material material = renderer.material;

        // If no highlights remain, restore original
        if (_activeHighlights[tile].Count == 0)
        {
            RestoreOriginalProperties(tile, material);
            _activeHighlights.Remove(tile);
        }
        else
        {
            // Apply the next highest priority highlight
            ApplyTopHighlight(tile, material);
        }
    }

    /// <summary>
    /// Apply the highest priority highlight that's active on this tile
    /// </summary>
    private void ApplyTopHighlight(GameObject tile, Material material)
    {
        if (!_activeHighlights.ContainsKey(tile) || _activeHighlights[tile].Count == 0)
            return;

        // Get highest priority highlight (Hover > PathPreview > MoveRange)
        HighlightLayer topLayer = _activeHighlights[tile]
            .OrderByDescending(h => (int)h.type)
            .First();

        // First, restore original properties to clear any previous effects
        if (_originalProperties.ContainsKey(tile))
        {
            MaterialProperties original = _originalProperties[tile];
            
            // Restore rim properties to original FIRST (in case lower priority doesn't have rim)
            if (original.rimColor.HasValue && material.HasProperty("_RimColor"))
                material.SetColor("_RimColor", original.rimColor.Value);
            if (original.rimLightColor.HasValue && material.HasProperty("_RimLightColor"))
                material.SetColor("_RimLightColor", original.rimLightColor.Value);
            if (original.rimPower.HasValue && material.HasProperty("_RimPower"))
                material.SetFloat("_RimPower", original.rimPower.Value);
            if (original.rimAmount.HasValue && material.HasProperty("_RimAmount"))
                material.SetFloat("_RimAmount", original.rimAmount.Value);
        }

        // Apply the new highlight color
        TrySetProperty("_Color", topLayer.color, material);
        TrySetProperty("_BaseColor", topLayer.color, material);
        TrySetProperty("_MainColor", topLayer.color, material);
        TrySetProperty("_TintColor", topLayer.color, material);

        // Apply rim if this layer has it
        if (topLayer.rimColor.HasValue)
        {
            TrySetProperty("_RimColor", topLayer.rimColor.Value, material);
            TrySetProperty("_RimLightColor", topLayer.rimColor.Value, material);
            if (topLayer.rimPower.HasValue)
                TrySetFloat("_RimPower", topLayer.rimPower.Value, material);
            TrySetFloat("_RimAmount", 0.7f, material);
        }
    }

    /// <summary>
    /// Clear all highlights of a specific type
    /// </summary>
    public void ClearHighlightsByType(HighlightType type)
    {
        var tilesToUpdate = _activeHighlights.Keys.ToList();

        foreach (GameObject tile in tilesToUpdate)
        {
            RemoveHighlight(tile, type);
        }
    }

    /// <summary>
    /// Clear ALL highlights from ALL tiles
    /// </summary>
    public void ClearAllHighlights()
    {
        var tilesToClear = _originalProperties.Keys.ToList();

        foreach (GameObject tile in tilesToClear)
        {
            if (tile == null) continue;

            Renderer renderer = tile.GetComponent<Renderer>();
            if (renderer == null) continue;

            Material material = renderer.material;
            RestoreOriginalProperties(tile, material);
        }

        _activeHighlights.Clear();
        _originalProperties.Clear();
    }

    private void RestoreOriginalProperties(GameObject tile, Material material)
    {
        if (!_originalProperties.ContainsKey(tile)) return;

        MaterialProperties props = _originalProperties[tile];

        // Restore color properties
        if (props.color.HasValue && material.HasProperty("_Color"))
            material.SetColor("_Color", props.color.Value);
        if (props.baseColor.HasValue && material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", props.baseColor.Value);
        if (props.mainColor.HasValue && material.HasProperty("_MainColor"))
            material.SetColor("_MainColor", props.mainColor.Value);
        if (props.tintColor.HasValue && material.HasProperty("_TintColor"))
            material.SetColor("_TintColor", props.tintColor.Value);

        // Restore rim light properties
        if (props.rimColor.HasValue && material.HasProperty("_RimColor"))
            material.SetColor("_RimColor", props.rimColor.Value);
        if (props.rimLightColor.HasValue && material.HasProperty("_RimLightColor"))
            material.SetColor("_RimLightColor", props.rimLightColor.Value);
        if (props.rimPower.HasValue && material.HasProperty("_RimPower"))
            material.SetFloat("_RimPower", props.rimPower.Value);
        if (props.rimAmount.HasValue && material.HasProperty("_RimAmount"))
            material.SetFloat("_RimAmount", props.rimAmount.Value);

        // Restore outline properties
        if (props.outlineColor.HasValue && material.HasProperty("_OutlineColor"))
            material.SetColor("_OutlineColor", props.outlineColor.Value);
        if (props.outlineWidth.HasValue && material.HasProperty("_OutlineWidth"))
            material.SetFloat("_OutlineWidth", props.outlineWidth.Value);

        _originalProperties.Remove(tile);
    }

    private void TrySetProperty(string propertyName, Color value, Material material)
    {
        if (material.HasProperty(propertyName))
            material.SetColor(propertyName, value);
    }

    private void TrySetFloat(string propertyName, float value, Material material)
    {
        if (material.HasProperty(propertyName))
            material.SetFloat(propertyName, value);
    }

    private void OnDestroy()
    {
        ClearAllHighlights();
    }
}
