using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MoonpetalValley.Audio
{
    public class MusicDebugOverlay : MonoBehaviour
    {
        private const string OWNER_DEBUG = "debug_override";
        private bool _visible;
        private MusicManager _manager;
        private MusicSignalProbe _probe;

        private void Awake()
        {
            _manager = GetComponent<MusicManager>();
            _probe = GetComponent<MusicSignalProbe>();
        }

        private void Update()
        {
            if (!Application.isEditor && !Debug.isDebugBuild) return;
            if (WasF2Pressed()) _visible = !_visible;
        }

        private bool WasF2Pressed()
        {
            bool pressed = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.f2Key.wasPressedThisFrame) pressed = true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.F2)) pressed = true;
#endif
            return pressed;
        }

        private void OnGUI()
        {
            if (!_visible || _manager == null || (!Application.isEditor && !Debug.isDebugBuild)) return;

            GUILayout.BeginArea(new Rect(14, 14, 340, 300), "MV Audio Debug [F2]", GUI.skin.window);
            GUILayout.Label("Context: " + _manager.ActiveContext + " | Track: " + _manager.ActiveTrackId);
            GUILayout.Label(string.Format("Time: {0:0.0}s / {1:0.0}s | Rest: {2}", _manager.GetActiveClipTime(), _manager.GetActiveClipLength(), _manager.IsResting));
            GUILayout.Label(string.Format("VolScale: {0:0.00} | Duck: {1:0.00} | Muted: {2}", _manager.EffectiveMasterScale, _manager.CurrentDuckMultiplier, _manager.EffectiveMuted));
            if (_probe != null)
            {
                GUILayout.Label("Scene: " + _probe.CurrentSceneName);
                GUILayout.Label(string.Format("Mist:{0} Combat:{1} Dlg:{2} Cut:{3}", _probe.IsInMist, _probe.IsInCombat, _probe.IsDialogueActive, _probe.IsCutsceneActive));
            }
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Day")) _manager.PushContext(OWNER_DEBUG, MusicContext.Safe_Day, 200);
            if (GUILayout.Button("Night")) _manager.PushContext(OWNER_DEBUG, MusicContext.Safe_Night, 200);
            if (GUILayout.Button("Mist")) _manager.PushContext(OWNER_DEBUG, MusicContext.Mist_Zone, 200);
            if (GUILayout.Button("Combat")) _manager.PushContext(OWNER_DEBUG, MusicContext.Combat, 200);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Title")) _manager.PushContext(OWNER_DEBUG, MusicContext.Title, 200);
            if (GUILayout.Button("Clear Override")) _manager.PopContext(OWNER_DEBUG);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("S1 Petal")) _manager.PlayStinger(StingerType.PetalObtained);
            if (GUILayout.Button("S2 Quest")) _manager.PlayStinger(StingerType.QuestComplete);
            if (GUILayout.Button("S3 KO")) _manager.PlayStinger(StingerType.KnockedOut);
            if (GUILayout.Button("S4 Day")) _manager.PlayStinger(StingerType.NewDay);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
