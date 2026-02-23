#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Logger))]
public class LoggerEditor : Editor
{
    private string searchFilter = "";
    private Vector2 scrollPosition;
    private bool showCategoryFoldout = true;
    private bool showCommonScriptsFoldout = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(10);
        
        // Global toggle
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        SerializedProperty globalEnabled = serializedObject.FindProperty("globalLoggingEnabled");
        EditorGUILayout.PropertyField(globalEnabled, new GUIContent("Global Logging"));
        
        if (GUILayout.Button(globalEnabled.boolValue ? "Disable All Logging" : "Enable All Logging", GUILayout.Height(30)))
        {
            globalEnabled.boolValue = !globalEnabled.boolValue;
            serializedObject.ApplyModifiedProperties();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // Category section
        DrawCategorySection();

        EditorGUILayout.Space(10);

        // Common scripts section
        DrawCommonScriptsSection();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCategorySection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showCategoryFoldout = EditorGUILayout.Foldout(showCategoryFoldout, "Categories", true, EditorStyles.foldoutHeader);
        
        if (showCategoryFoldout)
        {
            SerializedProperty categories = serializedObject.FindProperty("categories");
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All Categories"))
            {
                for (int i = 0; i < categories.arraySize; i++)
                {
                    categories.GetArrayElementAtIndex(i).FindPropertyRelative("enabled").boolValue = true;
                }
                serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("Disable All Categories"))
            {
                for (int i = 0; i < categories.arraySize; i++)
                {
                    categories.GetArrayElementAtIndex(i).FindPropertyRelative("enabled").boolValue = false;
                }
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Draw category items
            for (int i = 0; i < categories.arraySize; i++)
            {
                SerializedProperty category = categories.GetArrayElementAtIndex(i);
                SerializedProperty categoryName = category.FindPropertyRelative("categoryName");
                SerializedProperty enabled = category.FindPropertyRelative("enabled");

                EditorGUILayout.BeginHorizontal();
                
                // Toggle with label
                EditorGUI.BeginChangeCheck();
                bool newValue = EditorGUILayout.ToggleLeft(categoryName.stringValue, enabled.boolValue);
                if (EditorGUI.EndChangeCheck())
                {
                    enabled.boolValue = newValue;
                    serializedObject.ApplyModifiedProperties();
                    GUI.changed = true;
                }

                // Remove button
                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    categories.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    GUIUtility.ExitGUI();
                }
                
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Add Category"))
            {
                categories.InsertArrayElementAtIndex(categories.arraySize);
                SerializedProperty newCategory = categories.GetArrayElementAtIndex(categories.arraySize - 1);
                newCategory.FindPropertyRelative("categoryName").stringValue = "NewCategory";
                newCategory.FindPropertyRelative("enabled").boolValue = true;
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawCommonScriptsSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showCommonScriptsFoldout = EditorGUILayout.Foldout(showCommonScriptsFoldout, "Common Scripts", true, EditorStyles.foldoutHeader);
        
        if (showCommonScriptsFoldout)
        {
            SerializedProperty commonScripts = serializedObject.FindProperty("commonScripts");

            // Search bar
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchFilter = EditorGUILayout.TextField(searchFilter);
            if (GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                searchFilter = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            // Bulk operations
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All"))
            {
                for (int i = 0; i < commonScripts.arraySize; i++)
                {
                    commonScripts.GetArrayElementAtIndex(i).FindPropertyRelative("enabled").boolValue = true;
                }
                serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("Disable All"))
            {
                for (int i = 0; i < commonScripts.arraySize; i++)
                {
                    commonScripts.GetArrayElementAtIndex(i).FindPropertyRelative("enabled").boolValue = false;
                }
                serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("+ Add Script"))
            {
                commonScripts.InsertArrayElementAtIndex(commonScripts.arraySize);
                SerializedProperty newScript = commonScripts.GetArrayElementAtIndex(commonScripts.arraySize - 1);
                newScript.FindPropertyRelative("scriptName").stringValue = "ScriptName";
                newScript.FindPropertyRelative("enabled").boolValue = true;
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Scrollable list
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            
            int filteredCount = 0;
            List<int> toRemove = new List<int>();
            
            for (int i = 0; i < commonScripts.arraySize; i++)
            {
                SerializedProperty script = commonScripts.GetArrayElementAtIndex(i);
                SerializedProperty scriptName = script.FindPropertyRelative("scriptName");
                SerializedProperty enabled = script.FindPropertyRelative("enabled");

                // Filter by search
                if (!string.IsNullOrEmpty(searchFilter) && 
                    !scriptName.stringValue.ToLower().Contains(searchFilter.ToLower()))
                {
                    continue;
                }

                filteredCount++;

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                
                // Toggle with better interaction
                EditorGUI.BeginChangeCheck();
                bool newValue = EditorGUILayout.Toggle(GUIContent.none, enabled.boolValue, GUILayout.Width(20));
                if (EditorGUI.EndChangeCheck())
                {
                    enabled.boolValue = newValue;
                    serializedObject.ApplyModifiedProperties();
                    GUI.changed = true;
                }
                
                // Script name
                EditorGUI.BeginChangeCheck();
                string newName = EditorGUILayout.TextField(scriptName.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    scriptName.stringValue = newName;
                    serializedObject.ApplyModifiedProperties();
                }

                // Remove button
                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    toRemove.Add(i);
                }
                
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // Remove scripts (done outside loop)
            if (toRemove.Count > 0)
            {
                for (int i = toRemove.Count - 1; i >= 0; i--)
                {
                    commonScripts.DeleteArrayElementAtIndex(toRemove[i]);
                }
                serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }

            // Show count
            EditorGUILayout.LabelField($"Showing {filteredCount} of {commonScripts.arraySize} scripts", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();
    }
}
#endif