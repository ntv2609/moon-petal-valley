using UnityEngine;

namespace MoonpetalValley.Player
{
    public class GameInputReader : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public bool DashPressed { get; private set; }
        public bool ActionPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool LumiSkillPressed { get; private set; }
        public bool DiaryPressed { get; private set; }
        public bool InventoryPressed { get; private set; }
        public bool PausePressed { get; private set; }
        public int HotbarSlotPressed { get; private set; } = -1;

        private void Update()
        {
            Vector2 move = Vector2.zero;
            bool dash = false;
            bool action = false;
            bool interact = false;
            bool lumi = false;
            bool diary = false;
            bool inv = false;
            bool pause = false;
            int hotbar = -1;

#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (kb != null)
            {
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) move.x -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) move.x += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) move.y -= 1f;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) move.y += 1f;

                if (kb.spaceKey.wasPressedThisFrame || kb.leftShiftKey.wasPressedThisFrame) dash = true;
                if (kb.jKey.wasPressedThisFrame) action = true;
                if (kb.eKey.wasPressedThisFrame) interact = true;
                if (kb.qKey.wasPressedThisFrame) lumi = true;
                if (kb.bKey.wasPressedThisFrame) diary = true;
                if (kb.tabKey.wasPressedThisFrame || kb.iKey.wasPressedThisFrame) inv = true;
                if (kb.escapeKey.wasPressedThisFrame) pause = true;

                if (kb.digit1Key.wasPressedThisFrame) hotbar = 0;
                else if (kb.digit2Key.wasPressedThisFrame) hotbar = 1;
                else if (kb.digit3Key.wasPressedThisFrame) hotbar = 2;
                else if (kb.digit4Key.wasPressedThisFrame) hotbar = 3;
                else if (kb.digit5Key.wasPressedThisFrame) hotbar = 4;
                else if (kb.digit6Key.wasPressedThisFrame) hotbar = 5;
                else if (kb.digit7Key.wasPressedThisFrame) hotbar = 6;
                else if (kb.digit8Key.wasPressedThisFrame) hotbar = 7;
            }
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                action = true;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            move.x = Input.GetAxisRaw("Horizontal");
            move.y = Input.GetAxisRaw("Vertical");
            dash = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift);
            action = Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0);
            interact = Input.GetKeyDown(KeyCode.E);
            lumi = Input.GetKeyDown(KeyCode.Q);
            diary = Input.GetKeyDown(KeyCode.B);
            inv = Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I);
            pause = Input.GetKeyDown(KeyCode.Escape);

            for (int i = 0; i < 8; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    hotbar = i;
                    break;
                }
            }
#endif

            MoveInput = move.sqrMagnitude > 1f ? move.normalized : move;
            DashPressed = dash;
            ActionPressed = action;
            InteractPressed = interact;
            LumiSkillPressed = lumi;
            DiaryPressed = diary;
            InventoryPressed = inv;
            PausePressed = pause;
            HotbarSlotPressed = hotbar;
        }
    }
}
