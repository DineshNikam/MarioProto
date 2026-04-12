#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class PlatformerAnimatorBootstrap
{
    public const string ControllerPath = "Assets/Animations/PlayerAnimator.controller";
    const string KnightPath = "Assets/Settings/brackeys_platformer_assets/sprites/knight.png";
    const string IdleAnimPath = "Assets/Animations/Knight_Idle.anim";
    const string RunAnimPath = "Assets/Animations/Knight_Run.anim";

    [MenuItem("Tools/MarioProto/Create Knight Animator")]
    public static void CreateKnightAnimatorAssets()
    {
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath) != null)
            return;

        if (!AssetDatabase.IsValidFolder("Assets/Animations"))
            AssetDatabase.CreateFolder("Assets", "Animations");

        var sprites = AssetDatabase.LoadAllAssetsAtPath(KnightPath).OfType<Sprite>().ToDictionary(s => s.name);

        var idleFrames = new[] { "knight_0", "knight_1", "knight_2", "knight_3" }
            .Select(n => sprites[n]).ToArray();
        var runFrames = new[] { "knight_9", "knight_10", "knight_11", "knight_12", "knight_13", "knight_14", "knight_15" }
            .Select(n => sprites[n]).ToArray();

        var idleClip = BuildSpriteClip("Knight_Idle", idleFrames, 0.12f);
        var runClip = BuildSpriteClip("Knight_Run", runFrames, 0.08f);

        AssetDatabase.CreateAsset(idleClip, IdleAnimPath);
        AssetDatabase.CreateAsset(runClip, RunAnimPath);

        var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);

        var root = controller.layers[0].stateMachine;
        var idleState = root.AddState("Idle");
        idleState.motion = idleClip;
        var runState = root.AddState("Run");
        runState.motion = runClip;
        root.defaultState = idleState;

        var toRun = idleState.AddTransition(runState);
        toRun.hasExitTime = false;
        toRun.duration = 0.05f;
        toRun.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");

        var toIdle = runState.AddTransition(idleState);
        toIdle.hasExitTime = false;
        toIdle.duration = 0.05f;
        toIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static AnimationClip BuildSpriteClip(string clipName, IReadOnlyList<Sprite> frames, float frameTime)
    {
        var clip = new AnimationClip { name = clipName };
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        var binding = new EditorCurveBinding
        {
            path = "",
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };

        var keys = new ObjectReferenceKeyframe[frames.Count];
        for (var i = 0; i < frames.Count; i++)
        {
            keys[i] = new ObjectReferenceKeyframe
            {
                time = i * frameTime,
                value = frames[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        return clip;
    }
}
#endif
