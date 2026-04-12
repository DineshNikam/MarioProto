#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlatformerSceneBuilder
{
    public const string ScenePath = "Assets/Scenes/MainScene.unity";
    const string KnightTex = "Assets/Settings/brackeys_platformer_assets/sprites/knight.png";
    const string PlatformsTex = "Assets/Settings/brackeys_platformer_assets/sprites/platforms.png";
    const string JumpWav = "Assets/Settings/brackeys_platformer_assets/sounds/jump.wav";
    const string BgmPath = "Assets/Settings/brackeys_platformer_assets/music/time_for_adventure.mp3";
    const string PhysMatPath = "Assets/Materials/PlatformFriction.physicsMaterial2D";

    [MenuItem("Tools/MarioProto/Build Platformer Prototype")]
    public static void Build()
    {
        EnsureFolders();
        if (AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(PlatformerAnimatorBootstrap.ControllerPath) == null)
            PlatformerAnimatorBootstrap.CreateKnightAnimatorAssets();

        var physMat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(PhysMatPath);
        var sceneFull = Path.Combine(Application.dataPath, "Scenes/MainScene.unity");
        Scene scene;
        if (File.Exists(sceneFull))
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
                Object.DestroyImmediate(root);
        }
        else
            scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 4f;
        cam.transform.position = new Vector3(0f, -2.5f, -10f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.53f, 0.81f, 0.98f);
        camGo.AddComponent<AudioListener>();

        var groundRoot = new GameObject("Ground");
        CreateFloor(groundRoot.transform, physMat);
        CreatePlatformPiece(groundRoot.transform, "Platform_Mid", "platforms_1", new Vector3(6f, -2f, 0f), physMat);
        CreatePlatformPiece(groundRoot.transform, "Platform_High", "platforms_2", new Vector3(-4f, -0.5f, 0f), physMat);
        CreatePlatformPiece(groundRoot.transform, "Platform_Small", "platforms_3", new Vector3(11f, -1.25f, 0f), physMat);

        var player = CreatePlayer(physMat);
        var follow = camGo.AddComponent<CameraFollow>();
        var soFollow = new SerializedObject(follow);
        soFollow.FindProperty("target").objectReferenceValue = player.transform;
        soFollow.ApplyModifiedPropertiesWithoutUndo();

        CreateMusic();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);

        PrefabUtility.SaveAsPrefabAsset(player, "Assets/Prefabs/Player.prefab");
        var platRef = GameObject.Find("Platform_Mid");
        if (platRef != null)
            PrefabUtility.SaveAsPrefabAsset(platRef, "Assets/Prefabs/Platform.prefab");

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true),
            new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", false)
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("MarioProto: MainScene and prefabs built.");
    }

    static void EnsureFolders()
    {
        void Ensure(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }

        Ensure("Assets", "Scripts");
        Ensure("Assets", "Prefabs");
        Ensure("Assets", "Sprites");
        Ensure("Assets", "Audio");
        Ensure("Assets", "Materials");
        Ensure("Assets", "Animations");
        Ensure("Assets", "Editor");
        AssetDatabase.Refresh();
    }

    static void CreateFloor(Transform parent, PhysicsMaterial2D physMat)
    {
        var go = new GameObject("Floor");
        go.transform.SetParent(parent);
        go.transform.position = new Vector3(0f, -4f, 0f);
        go.transform.localScale = Vector3.one;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite(PlatformsTex, "platforms_0");
        sr.sortingOrder = 0;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(28f, 1.25f);
        go.layer = LayerMask.NameToLayer("Ground");
        var box = go.AddComponent<BoxCollider2D>();
        box.sharedMaterial = physMat;
        box.size = sr.size;
    }

    static void CreatePlatformPiece(Transform parent, string objName, string spriteName, Vector3 pos, PhysicsMaterial2D physMat)
    {
        var go = new GameObject(objName);
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = Vector3.one;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite(PlatformsTex, spriteName);
        sr.sortingOrder = 1;
        sr.drawMode = SpriteDrawMode.Simple;
        go.layer = LayerMask.NameToLayer("Ground");
        var box = go.AddComponent<BoxCollider2D>();
        box.sharedMaterial = physMat;
        if (sr.sprite != null)
            box.size = sr.sprite.bounds.size;
    }

    static GameObject CreatePlayer(PhysicsMaterial2D _)
    {
        var go = new GameObject("Player");
        go.tag = "Player";
        go.transform.position = new Vector3(-2.5f, -2.85f, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite(KnightTex, "knight_0");
        sr.sortingOrder = 10;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var box = go.AddComponent<BoxCollider2D>();
        if (sr.sprite != null)
        {
            var b = sr.sprite.bounds;
            box.size = new Vector2(b.size.x * 0.92f, b.size.y * 0.95f);
            box.offset = new Vector2(b.center.x, b.center.y);
        }
        else
        {
            box.size = new Vector2(0.9f, 1.25f);
            box.offset = new Vector2(0f, 0.62f);
        }

        var gc = new GameObject("GroundCheck").transform;
        gc.SetParent(go.transform, false);
        gc.localPosition = new Vector3(0f, -0.04f, 0f);

        var anim = go.AddComponent<Animator>();
        anim.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(PlatformerAnimatorBootstrap.ControllerPath);

        var pc = go.AddComponent<PlayerController>();
        var soPc = new SerializedObject(pc);
        soPc.FindProperty("groundLayers").intValue = LayerMask.GetMask("Ground");
        soPc.FindProperty("groundCheck").objectReferenceValue = gc;
        soPc.FindProperty("groundCheckSize").vector2Value = new Vector2(0.55f, 0.12f);
        soPc.FindProperty("animator").objectReferenceValue = anim;
        soPc.FindProperty("spriteRenderer").objectReferenceValue = sr;
        soPc.ApplyModifiedPropertiesWithoutUndo();

        go.AddComponent<PlayerFeetDust>();
        var pa = go.AddComponent<PlayerAudio>();
        var jumpClip = AssetDatabase.LoadAssetAtPath<AudioClip>(JumpWav);
        var soPa = new SerializedObject(pa);
        soPa.FindProperty("jumpClip").objectReferenceValue = jumpClip;
        soPa.ApplyModifiedPropertiesWithoutUndo();

        var dustGo = new GameObject("FootDust");
        dustGo.transform.SetParent(go.transform, false);
        dustGo.transform.localPosition = new Vector3(0f, 0.08f, 0f);
        var ps = dustGo.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = 0.35f;
        main.startLifetime = 0.25f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.16f);
        main.startColor = new Color(0.7f, 0.65f, 0.55f, 0.6f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 24;
        var em = ps.emission;
        em.rateOverTime = 0f;
        var burst = new ParticleSystem.Burst(0f, 8, 14);
        em.SetBursts(new[] { burst });
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.2f;
        var renderer = dustGo.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 9;

        var soDust = new SerializedObject(go.GetComponent<PlayerFeetDust>());
        soDust.FindProperty("dustParticles").objectReferenceValue = ps;
        soDust.ApplyModifiedPropertiesWithoutUndo();

        return go;
    }

    static void CreateMusic()
    {
        var go = new GameObject("GameAudio");
        var src = go.AddComponent<AudioSource>();
        src.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(BgmPath);
        src.loop = true;
        src.playOnAwake = true;
        src.volume = 0.35f;
    }

    static Sprite LoadSprite(string texturePath, string spriteName)
    {
        foreach (var o in AssetDatabase.LoadAllAssetsAtPath(texturePath))
        {
            if (o is Sprite sp && sp.name == spriteName)
                return sp;
        }

        Debug.LogError($"Sprite not found: {spriteName} in {texturePath}");
        return null;
    }
}
#endif
