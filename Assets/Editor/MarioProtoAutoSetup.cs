#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[InitializeOnLoad]
public static class MarioProtoAutoSetup
{
    static MarioProtoAutoSetup()
    {
        CompilationPipeline.compilationFinished += _ => EditorApplication.delayCall += MaybeBuild;
        EditorApplication.delayCall += MaybeBuild;
    }

    static bool SceneNeedsPopulation()
    {
        var sceneFullPath = Path.Combine(Application.dataPath, "Scenes/MainScene.unity");
        if (!File.Exists(sceneFullPath))
            return true;
        var text = File.ReadAllText(sceneFullPath);
        if (text.Contains("m_Roots: []"))
            return true;
        if (text.Contains("m_Name: Floor") && text.Contains("m_LocalScale: {x: 28, y: 1.2"))
            return true;
        return false;
    }

    static void MaybeBuild()
    {
        if (EditorApplication.isCompiling)
        {
            EditorApplication.delayCall += MaybeBuild;
            return;
        }

        if (!SceneNeedsPopulation())
            return;

        PlatformerSceneBuilder.Build();
    }
}
#endif
