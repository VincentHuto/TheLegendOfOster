using System.IO;
using UnityEditor;
using UnityEngine;

public class ExportAnim : MonoBehaviour
{
    [MenuItem("Assets/Extract Animation")]
    private static void ExtractAnimation()
    {
        foreach (var obj in Selection.objects)
        {
            var fbx = AssetDatabase.GetAssetPath(obj);
            var directory = Path.GetDirectoryName(fbx);
            CreateAnim(fbx, directory);
        }
    }

    static void CreateAnim(string fbx, string target)
    {
        var fileName = Path.GetFileNameWithoutExtension(fbx);
        var filePath = $"{target}/{fileName}.anim";
        AnimationClip src = AssetDatabase.LoadAssetAtPath<AnimationClip>(fbx);
        AnimationClip temp = new AnimationClip();
        EditorUtility.CopySerialized(src, temp);
        AssetDatabase.CreateAsset(temp, filePath);
        AssetDatabase.SaveAssets();
    }
}
