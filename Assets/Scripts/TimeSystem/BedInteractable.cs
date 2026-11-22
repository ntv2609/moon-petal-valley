using UnityEngine;
using MoonpetalValley.Core;
using MoonpetalValley.Player;

namespace MoonpetalValley.TimeSystem
{
    public class BedInteractable : MonoBehaviour
    {
        [SerializeField] private GameObject promptObject;
        [SerializeField] private float interactRadius = 1.9f;

        private PlayerController2D _player;
        private GameInputReader _input;

        private void Start()
        {
            if (promptObject != null) promptObject.SetActive(false);
        }

        private void Update()
        {
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

            float dist = Vector2.Distance(transform.position, _player.transform.position);
            bool inRange = dist <= interactRadius;

            if (promptObject != null && promptObject.activeSelf != inRange)
            {
                promptObject.SetActive(inRange);
            }

            if (inRange && _input != null && _input.InteractPressed)
            {
                if (GameRoot.Instance != null && GameRoot.Instance.Sleep != null)
                {
                    GameRoot.Instance.Sleep.TriggerNormalSleep();
                }
            }
        }
    }
}
