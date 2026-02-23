using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Logger : MonoBehaviour
{
    private static Logger _instance;
    public static Logger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Logger>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("Logger");
                    _instance = go.AddComponent<Logger>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [Header("Global Settings")]
    [SerializeField] private bool globalLoggingEnabled = true;
    
    [Header("Category Management")]
    [Tooltip("Enable/disable entire categories at once")]
    [SerializeField] private List<CategorySettings> categories = new List<CategorySettings>
    {
        new CategorySettings { categoryName = "AI", enabled = false },
        new CategorySettings { categoryName = "Player", enabled = false },
        new CategorySettings { categoryName = "Grid", enabled = false },
        new CategorySettings { categoryName = "UI", enabled = false },
        new CategorySettings { categoryName = "Combat", enabled = false },
        new CategorySettings { categoryName = "Debug", enabled = false },
        new CategorySettings { categoryName = "Turn", enabled = false }
    };

    [Header("Quick Toggles - Common Scripts")]
    [SerializeField] private List<ScriptLogSettings> commonScripts = new List<ScriptLogSettings>();

    private Dictionary<string, bool> scriptLoggingCache = new Dictionary<string, bool>();
    private Dictionary<string, bool> categoryCache = new Dictionary<string, bool>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        RebuildCache();
    }

    private void RebuildCache()
    {
        scriptLoggingCache.Clear();
        categoryCache.Clear();

        // Build category cache
        foreach (var category in categories)
        {
            if (!string.IsNullOrEmpty(category.categoryName))
            {
                categoryCache[category.categoryName] = category.enabled;
            }
        }

        // Build script cache
        foreach (var setting in commonScripts)
        {
            if (!string.IsNullOrEmpty(setting.scriptName))
            {
                scriptLoggingCache[setting.scriptName] = setting.enabled;
            }
        }
    }

    // Main logging methods
    public static void Log(object message, Object context = null)
    {
        if (Instance.ShouldLog(GetCallerName()))
        {
            Debug.Log($"[{GetCallerName()}] {message}", context);
        }
    }

    public static void LogWarning(object message, Object context = null)
    {
        if (Instance.ShouldLog(GetCallerName()))
        {
            Debug.LogWarning($"[{GetCallerName()}] {message}", context);
        }
    }

    public static void LogError(object message, Object context = null)
    {
        if (Instance.ShouldLog(GetCallerName()))
        {
            Debug.LogError($"[{GetCallerName()}] {message}", context);
        }
    }

    // Category-based logging
    public static void LogCategory(string category, object message, Object context = null)
    {
        if (Instance.ShouldLogCategory(category))
        {
            Debug.Log($"[{category}:{GetCallerName()}] {message}", context);
        }
    }

    public static void LogWarningCategory(string category, object message, Object context = null)
    {
        if (Instance.ShouldLogCategory(category))
        {
            Debug.LogWarning($"[{category}:{GetCallerName()}] {message}", context);
        }
    }

    public static void LogErrorCategory(string category, object message, Object context = null)
    {
        if (Instance.ShouldLogCategory(category))
        {
            Debug.LogError($"[{category}:{GetCallerName()}] {message}", context);
        }
    }

    // Explicit script name versions
    public static void Log(string scriptName, object message, Object context = null)
    {
        if (Instance.ShouldLog(scriptName))
        {
            Debug.Log($"[{scriptName}] {message}", context);
        }
    }

    public static void LogWarning(string scriptName, object message, Object context = null)
    {
        if (Instance.ShouldLog(scriptName))
        {
            Debug.LogWarning($"[{scriptName}] {message}", context);
        }
    }

    public static void LogError(string scriptName, object message, Object context = null)
    {
        if (Instance.ShouldLog(scriptName))
        {
            Debug.LogError($"[{scriptName}] {message}", context);
        }
    }

    // Check if a script should log
    private bool ShouldLog(string scriptName)
    {
        if (!globalLoggingEnabled) return false;

        // Check if script has specific setting
        if (scriptLoggingCache.TryGetValue(scriptName, out bool enabled))
        {
            return enabled;
        }

        // Default: allow logging if not explicitly configured
        return true;
    }

    // Check if a category should log
    private bool ShouldLogCategory(string category)
    {
        if (!globalLoggingEnabled) return false;

        if (categoryCache.TryGetValue(category, out bool enabled))
        {
            return enabled;
        }

        return true;
    }

    // Get the name of the calling script
    private static string GetCallerName()
    {
        var stackTrace = new System.Diagnostics.StackTrace();
        
        for (int i = 2; i < stackTrace.FrameCount; i++)
        {
            var method = stackTrace.GetFrame(i).GetMethod();
            var declaringType = method.DeclaringType;
            
            if (declaringType != null && declaringType != typeof(Logger))
            {
                return declaringType.Name;
            }
        }
        
        return "Unknown";
    }

    // Runtime API for scripts
    public static void EnableScript(string scriptName, bool enabled)
    {
        if (Instance.scriptLoggingCache.ContainsKey(scriptName))
        {
            Instance.scriptLoggingCache[scriptName] = enabled;
        }
        else
        {
            Instance.scriptLoggingCache.Add(scriptName, enabled);
        }

        var setting = Instance.commonScripts.Find(s => s.scriptName == scriptName);
        if (setting != null)
        {
            setting.enabled = enabled;
        }
        else
        {
            Instance.commonScripts.Add(new ScriptLogSettings { scriptName = scriptName, enabled = enabled });
        }
    }

    // Runtime API for categories
    public static void EnableCategory(string categoryName, bool enabled)
    {
        if (Instance.categoryCache.ContainsKey(categoryName))
        {
            Instance.categoryCache[categoryName] = enabled;
        }
        else
        {
            Instance.categoryCache.Add(categoryName, enabled);
        }

        var category = Instance.categories.Find(c => c.categoryName == categoryName);
        if (category != null)
        {
            category.enabled = enabled;
        }
        else
        {
            Instance.categories.Add(new CategorySettings { categoryName = categoryName, enabled = enabled });
        }
    }

    public static void SetGlobalLogging(bool enabled)
    {
        Instance.globalLoggingEnabled = enabled;
    }

    // Bulk operations
    public static void EnableAll()
    {
        Instance.globalLoggingEnabled = true;
        foreach (var category in Instance.categories)
        {
            category.enabled = true;
        }
        foreach (var script in Instance.commonScripts)
        {
            script.enabled = true;
        }
        Instance.RebuildCache();
    }

    public static void DisableAll()
    {
        Instance.globalLoggingEnabled = false;
    }

    public static void EnableOnlyCategory(string categoryName)
    {
        foreach (var category in Instance.categories)
        {
            category.enabled = (category.categoryName == categoryName);
        }
        Instance.RebuildCache();
    }

    private void OnValidate()
    {
        RebuildCache();
    }
}

[System.Serializable]
public class ScriptLogSettings
{
    public string scriptName;
    public bool enabled = true;
}

[System.Serializable]
public class CategorySettings
{
    public string categoryName;
    public bool enabled = true;
}