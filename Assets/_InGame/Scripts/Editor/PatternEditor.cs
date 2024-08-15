using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pattern))]
public class PatternEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector layout

        Pattern patternScript = (Pattern)target;

        if (GUILayout.Button("Generate Pattern"))
        {
            patternScript.GeneratePatternInEditor();
        }

        if (GUILayout.Button("Destroy Existing Pattern"))
        {
            patternScript.DestroyPatternInEditor();
        }
    }
}
