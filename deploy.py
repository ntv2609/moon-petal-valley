import os
import sys
import shutil
import zipfile
from datetime import datetime
from pathlib import Path

STEP_NAME = "Step_13_NPCAnimations"

FILES = {
    "Assets/Editor/MoonpetalAssetPostprocessor.cs": r"""using UnityEditor;
using UnityEngine;

namespace MoonpetalValley.EditorTools
{
    public class MoonpetalAssetPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Art/Sprites/")) return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;

            bool isAnimSprite = assetPath.StartsWith("Assets/Art/Sprites/Animations/");
            if (assetPath.StartsWith("Assets/Art/Sprites/Animations/Wen/") ||
                assetPath.StartsWith("Assets/Art/Sprites/Animations/Lumi/") ||
                assetPath.StartsWith("Assets/Art/Sprites/Animations/NPCs/"))
            {
                importer.spritePixelsPerUnit = 20;
            }
            else if (assetPath.StartsWith("Assets/Art/Sprites/Animations/GloomSlime/"))
            {
                importer.spritePixelsPerUnit = 32;
            }
            else
            {
                importer.spritePixelsPerUnit = 16;
            }

            if (isAnimSprite)
            {
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.spriteAlignment = (int)SpriteAlignment.Center;
                settings.spriteGenerateFallbackPhysicsShape = false;
                importer.SetTextureSettings(settings);
            }
        }
    }
}
""",

    "Assets/Scripts/Narrative/NPCController.cs": r"""using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Data;
using MoonpetalValley.Player;

namespace MoonpetalValley.Narrative
{
    public class NPCController : MonoBehaviour
    {
        [SerializeField] private NPCData npcData;
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 2.1f;
        [SerializeField] private Vector2 helperStandPosition;

        [Header("Animation Frames (32x32 FullRect)")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] cheerFrames;
        [SerializeField] private float idleFps = 2.4f;
        [SerializeField] private float cheerFps = 6.0f;

        private PlayerController2D _player;
        private GameInputReader _input;
        private bool _movedToFlowerbed;
        private float _animTimer;
        private int _frameIndex;
        private float _cheerTimer;

        public NPCData Data => npcData;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            GameEventBus.OnStoryFlagChanged += HandleFlagChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnStoryFlagChanged -= HandleFlagChanged;
        }

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
            // Lệch pha ngẫu nhiên để 4 NPC không chớp mắt cùng lúc
            _animTimer = Random.Range(0f, 0.35f);
            _frameIndex = Random.Range(0, 4);
            CheckMoveToFlowerbed();
            UpdateSortingOrder();
        }

        private void HandleFlagChanged(string flagId, bool value)
        {
            if (value) CheckMoveToFlowerbed();
        }

        private void CheckMoveToFlowerbed()
        {
            if (_movedToFlowerbed || npcData == null || string.IsNullOrEmpty(npcData.helperReadyStoryFlag)) return;
            if (GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                GameRoot.Instance.Flags.HasFlag(npcData.helperReadyStoryFlag))
            {
                _movedToFlowerbed = true;
                if (helperStandPosition != Vector2.zero)
                {
                    transform.position = new Vector3(helperStandPosition.x, helperStandPosition.y, 0f);
                    UpdateSortingOrder();
                }
            }
        }

        private void UpdateSortingOrder()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
            }
        }

        private void Update()
        {
            TickAnimation(Time.deltaTime);

            if (GameRoot.Instance != null && GameRoot.Instance.State != null && !GameRoot.Instance.State.IsGameplayActive)
            {
                if (promptObject != null && promptObject.activeSelf) promptObject.SetActive(false);
                return;
            }

            if (_player == null)
            {
                _player = UnityEngine.Object.FindAnyObjectByType<PlayerController2D>();
                if (_player != null) _input = _player.GetComponent<GameInputReader>();
                else return;
            }

            bool inRange = Vector2.Distance(transform.position, _player.transform.position) <= interactRadius;
            if (promptObject != null && promptObject.activeSelf != inRange) promptObject.SetActive(inRange);

            if (inRange && _input != null && _input.InteractPressed)
            {
                TriggerDialogue();
            }
        }

        private void TickAnimation(float dt)
        {
            if (spriteRenderer == null) return;

            if (_cheerTimer > 0f) _cheerTimer -= dt;
            bool celebratingVictory = GameRoot.Instance != null && GameRoot.Instance.Flags != null &&
                                      GameRoot.Instance.Flags.HasFlag("flag_nv1_completed");
            bool playCheer = (_cheerTimer > 0f || celebratingVictory) && cheerFrames != null && cheerFrames.Length > 0 && cheerFrames[0] != null;

            Sprite[] frames = playCheer ? cheerFrames : idleFrames;
            if (frames == null || frames.Length == 0 || frames[0] == null) return;

            float fps = playCheer ? cheerFps : idleFps;
            float dur = fps > 0.01f ? (1f / fps) : 0.35f;
            _animTimer += dt;

            if (_animTimer >= dur)
            {
                _animTimer -= dur;
                _frameIndex = (_frameIndex + 1) % frames.Length;
            }

            int idx = Mathf.Clamp(_frameIndex, 0, frames.Length - 1);
            if (frames[idx] != null) spriteRenderer.sprite = frames[idx];
        }

        private void TriggerDialogue()
        {
            if (npcData == null || GameRoot.Instance == null || GameRoot.Instance.Dialogue == null) return;
            _cheerTimer = 2.0f;
            _frameIndex = 0;
            _animTimer = 0f;

            DialogueData chosen = ResolveDialogue();
            if (chosen != null)
            {
                GameRoot.Instance.Dialogue.StartDialogue(chosen);
            }
        }

        private DialogueData ResolveDialogue()
        {
            var flags = GameRoot.Instance.Flags;
            var inv = GameRoot.Instance.Inventory;

            if (npcData.conditionalDialogues != null)
            {
                for (int i = 0; i < npcData.conditionalDialogues.Count; i++)
                {
                    var entry = npcData.conditionalDialogues[i];
                    if (entry == null || entry.dialogue == null) continue;

                    if (!string.IsNullOrEmpty(entry.requiredStoryFlag) && !flags.HasFlag(entry.requiredStoryFlag))
                        continue;
                    if (!string.IsNullOrEmpty(entry.blockedByStoryFlag) && flags.HasFlag(entry.blockedByStoryFlag))
                        continue;

                    if (entry.dialogue.consumesRequiredItem != null && entry.dialogue.consumesAmount > 0)
                    {
                        if (inv == null || !inv.HasItem(entry.dialogue.consumesRequiredItem.itemID, entry.dialogue.consumesAmount))
                            continue;
                    }

                    if (entry.dialogue.dialogueID == "dlg_bach_read_letters")
                    {
                        if (!flags.HasFlag("flag_hana_bread_done") || !flags.HasFlag("flag_toby_cat_saved") || !flags.HasFlag("flag_otis_mailbox_fixed"))
                            continue;
                    }

                    return entry.dialogue;
                }
            }
            return npcData.defaultDialogue;
        }
    }
}
""",

    "Assets/Editor/AutoBuilder_Step13.cs": r"""using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MoonpetalValley.Data;
using MoonpetalValley.Narrative;

namespace MoonpetalValley.EditorTools
{
    public static class AutoBuilder_Step13
    {
        private const string WorldScenePath = "Assets/Scenes/MoonpetalValley_MVP.unity";
        private const string NpcAnimRoot = "Assets/Art/Sprites/Animations/NPCs";

        private static readonly (string objName, string npcId)[] NpcEntries =
        {
            ("NPC_Bach", "npc_bach"),
            ("NPC_Hana", "npc_hana"),
            ("NPC_Toby", "npc_toby"),
            ("NPC_Otis", "npc_otis")
        };

        [MenuItem("Moonpetal/Step 13/1. Wire 4 NPCs Animations (Bach, Hana, Toby, Otis)")]
        public static void BuildStep13()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                Debug.LogWarning("[MV] Không thể chạy AutoBuilder khi đang Play Mode hoặc đang biên dịch.");
                return;
            }

            Debug.Log("[MV] === BẮT ĐẦU AUTOBUILDER STEP 13: ĐỒNG BỘ 4 NPC ===");
            AssetDatabase.Refresh();
            ForceReimportNpcSprites();

            var scene = EditorSceneManager.OpenScene(WorldScenePath, OpenSceneMode.Single);

            foreach (var entry in NpcEntries)
            {
                Sprite[] idleF = LoadFrames(entry.npcId, "idle", 4);
                Sprite[] cheerF = LoadFrames(entry.npcId, "cheer", 4);

                // Cập nhật NPCData asset
                var npcData = AssetDatabase.LoadAssetAtPath<NPCData>("Assets/Data/NPCs/NPC_" + entry.npcId + ".asset");
                if (npcData != null && idleF[0] != null)
                {
                    npcData.worldSprite = idleF[0];
                    EditorUtility.SetDirty(npcData);
                }

                // Cập nhật GameObject trong scene
                GameObject go = GameObject.Find(entry.objName);
                if (go == null)
                {
                    Debug.LogError("[MV] Không tìm thấy " + entry.objName + " trong scene!");
                    continue;
                }

                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null && idleF[0] != null) sr.sprite = idleF[0];

                var ctrl = go.GetComponent<NPCController>();
                if (ctrl != null)
                {
                    var so = new SerializedObject(ctrl);
                    so.FindProperty("spriteRenderer").objectReferenceValue = sr;
                    AssignArray(so.FindProperty("idleFrames"), idleF);
                    AssignArray(so.FindProperty("cheerFrames"), cheerF);
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MV] === HOÀN TẤT STEP 13! Chạy 'Moonpetal/Step 13/Validate' để kiểm chứng. ===");
        }

        [MenuItem("Moonpetal/Step 13/Validate")]
        public static void ValidateStep13()
        {
            int errors = 0;
            Debug.Log("[MV] === BẮT ĐẦU VALIDATE STEP 13 ===");

            int framesOk = 0;
            foreach (var entry in NpcEntries)
            {
                foreach (string anim in new[] { "idle", "cheer" })
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string path = NpcAnimRoot + "/" + entry.npcId + "/" + anim + "_" + i.ToString("00") + ".png";
                        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        if (s == null)
                        {
                            Debug.LogError("[MV] THIẾU sprite NPC: " + path);
                            errors++;
                        }
                        else if (Mathf.RoundToInt(s.rect.width) != 32 || Mathf.RoundToInt(s.rect.height) != 32)
                        {
                            Debug.LogError("[MV] SAI kích thước FullRect trên " + path + ": " + s.rect.width + "x" + s.rect.height);
                            errors++;
                        }
                        else
                        {
                            framesOk++;
                        }
                    }
                }

                GameObject go = GameObject.Find(entry.objName);
                var ctrl = go != null ? go.GetComponent<NPCController>() : null;
                if (ctrl == null)
                {
                    Debug.LogError("[MV] Thiếu NPCController trên " + entry.objName);
                    errors++;
                }
                else
                {
                    var so = new SerializedObject(ctrl);
                    if (so.FindProperty("idleFrames").arraySize < 4 || so.FindProperty("cheerFrames").arraySize < 4)
                    {
                        Debug.LogError("[MV] " + entry.objName + " chưa được nối đủ 4 frame idle & 4 frame cheer.");
                        errors++;
                    }
                }
            }

            if (errors == 0)
            {
                Debug.Log("[MV] VALIDATE STEP 13: PASS (0 lỗi). Cả 4 NPC (" + framesOk + "/32 frames FullRect 32x32) đã sẵn sàng!");
            }
            else
            {
                Debug.LogError("[MV] VALIDATE STEP 13: FAIL (" + errors + " lỗi).");
            }
        }

        private static void ForceReimportNpcSprites()
        {
            if (!Directory.Exists(NpcAnimRoot)) return;
            foreach (string file in Directory.GetFiles(NpcAnimRoot, "*.png", SearchOption.AllDirectories))
            {
                string assetPath = file.Replace("\\", "/");
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 20;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.alphaIsTransparency = true;
                    importer.mipmapEnabled = false;

                    var settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    settings.spriteAlignment = (int)SpriteAlignment.Center;
                    settings.spriteGenerateFallbackPhysicsShape = false;
                    importer.SetTextureSettings(settings);
                    importer.SaveAndReimport();
                }
            }
        }

        private static Sprite[] LoadFrames(string npcId, string anim, int count)
        {
            var list = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                string path = NpcAnimRoot + "/" + npcId + "/" + anim + "_" + i.ToString("00") + ".png";
                list.Add(AssetDatabase.LoadAssetAtPath<Sprite>(path));
            }
            return list.ToArray();
        }

        private static void AssignArray(SerializedProperty prop, Sprite[] sprites)
        {
            if (prop == null) return;
            prop.ClearArray();
            for (int i = 0; i < sprites.Length; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            }
        }
    }
}
"""
}


def find_latest_npc_zip():
    search_dirs = [
        Path.cwd(),
        Path.cwd() / "_art_zips",
        Path.cwd() / "Assets",
        Path.home() / "Downloads",
        Path.home() / "Desktop",
    ]
    found = []
    for d in search_dirs:
        if not d.is_dir():
            continue
        for p in d.glob("npc_animations_pack*.zip"):
            if p.is_file():
                found.append(p)
    if not found:
        return None
    found.sort(key=lambda x: x.stat().st_mtime, reverse=True)
    return found[0]


def extract_npc_zip():
    zpath = find_latest_npc_zip()
    if zpath is None:
        print("\n[CẢNH BÁO] Chưa tìm thấy npc_animations_pack.zip! Hãy tải file zip ở trên vào Downloads hoặc thư mục gốc dự án rồi chạy lại `python deploy.py`.")
        return False

    print(f"\n[OK] Tìm thấy {zpath}")
    dest_root = Path("Assets/Art/Sprites/Animations/NPCs")
    if dest_root.exists():
        shutil.rmtree(dest_root)

    valid_npcs = ("npc_bach", "npc_hana", "npc_toby", "npc_otis")
    count = 0
    with zipfile.ZipFile(zpath, "r") as zf:
        for info in zf.infolist():
            if info.is_dir():
                continue
            parts = info.filename.replace("\\", "/").split("/")
            if len(parts) == 2 and parts[0] in valid_npcs and (parts[1].startswith("idle_0") or parts[1].startswith("cheer_0")):
                dest = dest_root / parts[0] / parts[1]
                dest.parent.mkdir(parents=True, exist_ok=True)
                dest.write_bytes(zf.read(info.filename))
                count += 1
    print(f"    -> Đã trích xuất {count}/32 frame cho 4 NPC vào {dest_root}")
    return count == 32


def main():
    if not Path("Assets").is_dir():
        print("[LỖI] Hãy chạy deploy.py tại thư mục gốc dự án Unity (nơi có Assets/).")
        sys.exit(1)

    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_root = Path("_deploy_backup") / f"{STEP_NAME}_{timestamp}"

    for rel_path, content in FILES.items():
        target = Path(rel_path)
        if target.exists():
            bk = backup_root / rel_path
            bk.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(target, bk)
        target.parent.mkdir(parents=True, exist_ok=True)
        with open(target, "w", encoding="utf-8", newline="\n") as f:
            f.write(content.lstrip("\n"))
        print(f"[GHI] {rel_path}")

    if extract_npc_zip():
        print("\n=== HOÀN TẤT DEPLOY STEP 13 ===")
        print("Trong Unity 6.6:")
        print("1. Chờ biên dịch xong (0 lỗi).")
        print("2. Chạy menu: Moonpetal -> Step 13 -> 1. Wire 4 NPCs Animations (Bach, Hana, Toby, Otis)")
        print("3. Chạy menu: Moonpetal -> Step 13 -> Validate (phải báo PASS 32/32 frames FullRect 32x32).")
        print("4. Bấm Play để gặp 4 NPC sống động trong làng & khi tụ họp quanh Bồn Hoa!")


if __name__ == "__main__":
    main()