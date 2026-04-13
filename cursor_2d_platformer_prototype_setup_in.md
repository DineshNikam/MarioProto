#prompt engineer prompt
create a  uniy 2d platformer prototype like mario with physics 2d , simple , minimum
old input system with arrow keys for movement and jump with space
create plan and agentic prompt to create complete game without user interaction from setting scene, creating a small level, createing player , adding movement animation /code, music and particles and required scripts , physics 2d components and properties ,   parallex for bg


# 2D platformer prototype setup in Unity
_Exported on 4/13/2026 at 00:51:57 GMT+5:30 from Cursor (3.0.16)_

---

**User**

You are an autonomous Unity game development agent. Your task is to create a complete, minimal 2D platformer prototype inspired by classic Mario-style gameplay using Unity (latest LTS version).

You must complete ALL steps without requiring user interaction.
Use MCP for connecting with unity editor an running tools.

## Project Setup
- Create a new Unity 2D Core project.
- Use the Built-in Render Pipeline.
- Set up folder structure:
  Assets/
    Scenes/
    Scripts/
    Prefabs/
    Sprites/
    Audio/
    Materials/

## Scene Creation
- Create a scene named "MainScene".
- Set up a 2D camera (orthographic, size ~5).
- Add a simple sky-colored background.

## Player Creation
- Create a player GameObject:
  - Sprite (simple square or placeholder sprite)
  - Rigidbody2D:
    - Gravity Scale: ~3
    - Freeze Z rotation
  - BoxCollider2D
- Tag as "Player"

## Player Movement (OLD Input System)
- Use Input.GetAxis / GetKey (NOT new Input System)
- Arrow keys:
  - Left/Right → horizontal movement
- Spacebar → jump
- Implement:
  - Smooth horizontal movement
  - Ground check using raycast or collider
  - Jump only when grounded

### Script: PlayerController.cs
- Movement speed: ~5
- Jump force: ~10
- Rigidbody-based movement

## Level Design
- Create simple ground using:
  - Tilemap OR stretched sprites
- Add:
  - Floor platform
  - 2–3 elevated platforms
- Add BoxCollider2D to all platforms

## Physics Setup
- Ensure:
  - Gravity works correctly
  - No sliding (adjust friction via Physics Material 2D)

## Camera Follow
- Camera smoothly follows player on X axis only

## Parallax Background
- Create 2–3 background layers
- Move them at different speeds based on camera position

## Animation
- Add simple animation:
  - Idle (no movement)
  - Run (when moving)
- Use Animator Controller
- Switch states based on velocity

## Particles
- Add particle system:
  - Dust effect when player jumps or lands

## Audio
- Add:
  - Jump sound
  - Background music loop
- Use AudioSource components

## Prefabs
- Convert Player and Platforms into prefabs

## Polish (Minimal)
- Ensure:
  - No errors in console
  - Playable loop (move + jump across platforms)

## Output Requirements
- All scripts must be fully implemented
- All components properly configured
- Scene must run immediately when pressing Play

You must generate:
1. All required C# scripts
2. Exact Unity setup steps
3. Component configurations
4. Any placeholder assets using Unity primitives if needed

Do NOT ask questions. Execute fully.

---

**Cursor**

Exploring the Unity project to see what's present, then drafting a plan. Plan mode prevents execution until you approve the plan.



Noting the repo is a Unity 6–style project (URP in manifest, Quality uses URP; legacy input is off). Planning migration to the Built-in pipeline, legacy input, and full scene setup via MCP.



---

**User**

@Assets/Settings/brackeys_platformer_assets I have added this assets for chatacter , ground and other things , also added sounds so please check and add to plan to use them

---

**Cursor**

Exploring the Brackeys asset folder to identify contents, then updating the plan to reference them.



Updating the plan to use the Brackeys pack: `knight` sprites, `platforms` / `world_tileset`, `jump.wav`, and `time_for_adventure.mp3`.



Here’s what changed in the plan after inspecting [e:\MarioProto\Assets\Settings\brackeys_platformer_assets](e:\MarioProto\Assets\Settings\brackeys_platformer_assets):

### What’s in the pack (used in the plan)
- **Character:** `sprites/knight.png` (many slices `knight_0`, `knight_1`, …) for **idle/run** Animator clips and default `SpriteRenderer`.
- **Level:** `sprites/platforms.png` (`platforms_0` …) for platforms; `sprites/world_tileset.png` for **floor tilemap** and/or **parallax** layers.
- **Audio:** `sounds/jump.wav` for jump; `music/time_for_adventure.mp3` for looping BGM. Other SFX (`coin`, `hurt`, etc.) are noted as optional later.
- **License:** [LICENSE & CREDITS.txt](e:\MarioProto\Assets\Settings\brackeys_platformer_assets\LICENSE%20&%20CREDITS.txt) called out for attribution.

### Plan updates (in the doc)
- New section **“Art and audio source (Brackeys pack)”** with a mapping table and “use in place” under `Assets/Settings/brackeys_platformer_assets`.
- **Removed** procedural `AudioClip.Create` audio; replaced with **`PlayerAudio`** using the real **`.wav` / `.mp3`** files.
- **Animator** section now specifies **knight sprite frames** (plus **flipX** for direction) instead of color/scale hacks.
- **Scene / parallax / platforms** section now builds from **world_tileset** and **platforms** instead of white quads.
- **Todos** updated: `scripts-core` and `scene-prefabs` mention Brackeys assets.

The updated plan file is [2d_mario-style_prototype_c62b15e5.plan.md](file:///c:/Users/Admin/.cursor/plans/2d_mario-style_prototype_c62b15e5.plan.md). When you want this built in the editor, say to **execute the plan** and we can run it end-to-end with MCP.

---

**User**

Implement the plan as specified, it is attached for your reference. Do NOT edit the plan file itself.

To-do's from the plan have already been created. Do not create them again. Mark them as in_progress as you work, starting with the first one. Don't stop until you have completed all the to-dos.

---

**Cursor**

Implementing the plan: starting pipeline migration and reading project files we'll need to modify.



Creating folders, physics material, gameplay scripts, and an Editor bootstrap that builds the animator and MainScene (more reliable than manual YAML).



Unity is open on the project, so batch mode can't run. Adding a one-time `EditorApplication.delayCall` setup that builds `MainScene` when it's missing so the next domain reload (or opening the editor) completes the scene.



Creating script `.meta` files with fixed GUIDs and a hand-authored `MainScene.unity` so the project is playable without waiting for the Editor build script. Checking Unity 6 YAML for `Rigidbody2D` and `BoxCollider2D`.



Switching auto-setup to run after compilation finishes so the scene builds reliably.



Updating `PlatformerSceneBuilder` to open the empty `MainScene`, clear roots, and populate it. Updating `MarioProtoAutoSetup` to detect an empty scene (`m_Roots: []`) and trigger a build.



Renaming `ParallaxLayer` to `ParallaxBackground` to match the plan.



Here’s what was implemented and how it fits together.

### Render pipeline & input
- Removed **URP** from [`Packages/manifest.json`](e:\MarioProto\Packages\manifest.json).
- Set **`customRenderPipeline: {fileID: 0}`** on all quality levels in [`ProjectSettings/QualitySettings.asset`](e:\MarioProto\ProjectSettings\QualitySettings.asset).
- Cleared **`m_RenderPipelineGlobalSettingsMap`** in [`ProjectSettings/GraphicsSettings.asset`](e:\MarioProto\ProjectSettings\GraphicsSettings.asset).
- Set **`activeInputHandler: 0`** (Input Manager only) in [`ProjectSettings/ProjectSettings.asset`](e:\MarioProto\ProjectSettings\ProjectSettings.asset).
- Cleared Input System **`m_configObjects`** in [`ProjectSettings/EditorBuildSettings.asset`](e:\MarioProto\ProjectSettings\EditorBuildSettings.asset) and set **MainScene** first (enabled), **SampleScene** disabled.

### Tags, layers, physics
- **Player** tag and **Ground** layer in [`ProjectSettings/TagManager.asset`](e:\MarioProto\ProjectSettings\TagManager.asset).
- [`Assets/Materials/PlatformFriction.physicsMaterial2D`](e:\MarioProto\Assets\Materials\PlatformFriction.physicsMaterial2D) (high friction, no bounce) for player and platforms.

### Runtime scripts ([`Assets/Scripts/`](e:\MarioProto\Assets\Scripts))
- **`PlayerController`** – `Rigidbody2D`, horizontal smoothing (~5), jump (~10), **Space**, **Horizontal** axis + arrow keys, **Ground** `OverlapBox`, events for dust/audio, **Animator `Speed`**, `flipX`.
- **`CameraFollow`** – smooth follow on **X** only.
- **`ParallaxBackground`** – 2D parallax from camera motion (replaces the earlier `ParallaxLayer` name).
- **`PlayerFeetDust`** – particle burst on jump/land.
- **`PlayerAudio`** – `jump.wav` one-shot.

### Editor tooling
- [`PlatformerAnimatorBootstrap.cs`](e:\MarioProto\Assets\Editor\PlatformerAnimatorBootstrap.cs) – **Knight_Idle** / **Knight_Run** clips + **`PlayerAnimator.controller`** (`knight_0–3` idle, `knight_9–15` run).
- [`PlatformerSceneBuilder.cs`](e:\MarioProto\Assets\Editor\PlatformerSceneBuilder.cs) – **Tools → MarioProto → Build Platformer Prototype**: builds/opens **`MainScene`**, Brackeys sprites/audio, parallax, floor + 3 platforms, player, BGM, prefabs, build settings. If **`MainScene` already exists**, it **clears root objects** and rebuilds.
- [`MarioProtoAutoSetup.cs`](e:\MarioProto\Assets\Editor\MarioProtoAutoSetup.cs) – After compile / on delay, if **`MainScene` has empty `m_Roots`**, runs **`PlatformerSceneBuilder.Build()`** (this is what filled the scene after the empty MCP `scene-create`).

### Scene & prefabs
- **[`Assets/Scenes/MainScene.unity`](e:\MarioProto\Assets\Scenes\MainScene.unity)** – orthographic camera (~5, sky color), **Brackeys** knight/platforms/world tiles, parallax, **jump** + **time_for_adventure** music, dust particles, animator.
- Prefabs: [`Assets/Prefabs/Player.prefab`](e:\MarioProto\Assets\Prefabs\Player.prefab), [`Assets/Prefabs/Platform.prefab`](e:\MarioProto\Assets\Prefabs\Platform.prefab).

### MCP / Unity
- **`scene-create`** was used to add **`MainScene`**; **`assets-refresh`** was used to compile/refresh.
- Unity logged **`MarioProto: MainScene and prefabs built.`** and **`MainScene`** is open with **5 roots** and **build index 0**.

### What you should do locally
1. Open **MainScene** and press **Play**.
2. If anything is ever empty again: **Tools → MarioProto → Build Platformer Prototype**.

**Note:** [`SampleScene`](e:\MarioProto\Assets\Scenes\SampleScene.unity) is still the old URP-style scene; use **MainScene** for this prototype. You may see a **physics material .meta** import warning once; letting Unity reimport clears it.

---

**User**

using MCP check whats missing 
also check why everything like floar and player is so small , and I cant move player 

also image  you used for prallex are for are actually a sprite atlas which include multple environment elements , so for now disable background and parallax 

focus on floor , platform and player to set right , with movement and physics 

floor and platform not tilling right 

as shown in image thats how its looking right now
 

---

**Cursor**

Inspecting the scene and scripts with MCP, then fixing scale, movement, tiling, and disabling the parallax background.



Identifying issues:
1. Sprites use PPU 100 — Brackeys tiles are tiny in world space (~0.15×0.21 for the knight).
2. Floor uses huge transform scale on a small sprite — stretched atlas look; collider/visual mismatch.
3. Parallax used full atlas slices scaled up — wrong use of tileset art.
4. Console: Particle velocity curve mode errors — can break/simulate particles badly.
5. Player movement: Using `linearVelocity` on `Rigidbody2D` may be invalid in some Unity versions; high friction on the player collider (shared physics material) can fight horizontal motion.

Implementing fixes: updating texture import settings, rewriting the scene builder (no parallax, tiled floor, proper scale), fixing `PlayerController` and particles, and disabling the Background in the current scene.



Here’s what MCP and the project files showed, and what was changed.

### MCP / console
- **`gameobject-find`** failed (MCP returned no structured payload), so the hierarchy was checked via the scene file and scripts instead.
- **Errors:** `Particle Velocity curves must all be in the same mode` from **`velocityOverLifetime`** on the dust `ParticleSystem` — that’s removed in the builder so those errors stop.
- Other errors were MCP hub noise, not gameplay.

### Why everything looked tiny
- **`spritePixelsToUnits: 100`** on Brackeys art made sprites **~0.15×0.21** world units for the knight — tiny with **orthographic size 5**.
- **Fix:** Set **`spritePixelsToUnits: 16`** on  
  [`knight.png.meta`](e:\MarioProto\Assets\Settings\brackeys_platformer_assets\sprites\knight.png.meta) and [`platforms.png.meta`](e:\MarioProto\Assets\Settings\brackeys_platformer_assets\sprites\platforms.png.meta) so scale matches typical pixel art.

### Floor / “tiling” / atlas look
- The floor used **huge transform scale** on one small sprite → stretched atlas strip, not tiles.
- **Fix in [`PlatformerSceneBuilder`](e:\MarioProto\Assets\Editor\PlatformerSceneBuilder.cs):** floor uses **`SpriteDrawMode.Tiled`** and **`sr.size = (28, 1.25)`** with **scale (1,1,1)**, and **`BoxCollider2D.size`** matches the tiled size.
- **`wrapU` / `wrapV`** on `platforms.png` set to **Repeat (0)** so tiled drawing can repeat correctly.

### Parallax / background
- Parallax used **whole tileset slices** scaled up → wrong.
- **Parallax is no longer created** by the builder.
- **`Background`** in [`MainScene.unity`](e:\MarioProto\Assets\Scenes\MainScene.unity) is set to **`m_IsActive: 0`** so it stays off even before a rebuild.

### Movement / physics
- **Input** is still **`activeInputHandler: 0`** (legacy) — fine.
- **`linearVelocity`** is correct for **Unity 6** (`velocity` is obsolete); that’s what **`PlayerController`** uses again, with slightly stronger horizontal acceleration.
- **Important:** the **player no longer uses the high-friction `PhysicsMaterial2D`** on their collider (only **Ground** does). Shared friction on the player was a likely reason horizontal motion felt stuck.

### Player setup in the builder
- **`GroundCheck`** is created in the editor and **wired** on `PlayerController`, with a sensible **`groundCheckSize`**.
- **`Animator`** and **`SpriteRenderer`** are assigned via `SerializedObject` so idle/run works reliably.

### Camera / layout
- **Orthographic size 4**, camera **`(0, -2.5, -10)`** so the view matches the new world scale.
- **Player / platform positions** updated for the new floor height (`Floor` center **y = -4**, tiled height **1.25**).

### What you should do in Unity
1. Let Unity **reimport** `knight.png` and `platforms.png` (PPU + wrap changed).
2. Run **`Tools → MarioProto → Build Platformer Prototype`** once so the scene matches the new layout (or rely on **auto-rebuild** when it detects the old floor `m_LocalScale: {x: 28, y: 1.2` in [`MarioProtoAutoSetup`](e:\MarioProto\Assets\Editor\MarioProtoAutoSetup.cs)).
3. Press **Play** on **MainScene** — **A/D or arrows**, **Space** to jump.

If anything still feels off after rebuild, say whether the player is too fast/slow or the floor collider feels wrong and we can tune numbers only.

---

**User**

player cant move still and implement parallex nicely

