using UnityEditor;
using UnityEngine;

public class AnimationRepathTool
{
    [MenuItem("Tools/Animation/Repath Under New Parent")]
    static void RepathSelectedClip()
    {
        var clip = Selection.activeObject as AnimationClip;
        if (clip == null)
        {
            Debug.LogError("Please select an AnimationClip in the Project view.");
            return;
        }

        // Ask for the new parent path
        string newParent = EditorUtility.DisplayDialogComplex(
            "Add New Parent?",
            "This script will duplicate the selected AnimationClip and prepend a parent path.\n\n" +
            "Press OK to continue with default 'Armature', or Cancel to stop.",
            "OK", "Cancel", "Help"
        ) == 1 ? null : "mini simple skeleton demo";

        if (string.IsNullOrEmpty(newParent))
        {
            Debug.Log("Cancelled.");
            return;
        }

        string clipPath = AssetDatabase.GetAssetPath(clip);
        string newPath = clipPath.Replace(".anim", "_repathed.anim");

        var newClip = Object.Instantiate(clip);
        AssetDatabase.CreateAsset(newClip, newPath);

        var bindings = AnimationUtility.GetCurveBindings(newClip);
        foreach (var b in bindings)
        {
            var curve = AnimationUtility.GetEditorCurve(newClip, b);
            var newBinding = new EditorCurveBinding
            {
                path = $"{newParent}/{b.path}".Trim('/'),
                propertyName = b.propertyName,
                type = b.type
            };
            AnimationUtility.SetEditorCurve(newClip, b, null);
            AnimationUtility.SetEditorCurve(newClip, newBinding, curve);
        }

        Debug.Log($"✅ Repathed animation created: {newPath}");
    }
}
