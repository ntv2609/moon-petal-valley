using System.Collections.Generic;
using UnityEngine;
using MoonpetalValley.Core;

namespace MoonpetalValley.Combat
{
    public class BlightEncounterZone : MonoBehaviour
    {
        [SerializeField] private string encounterId;
        [SerializeField] private string completionFlag;
        [SerializeField] private string completionToast;
        [SerializeField] private List<GloomSlimeController> slimes = new List<GloomSlimeController>();

        private bool _completed;

        private void OnEnable()
        {
            GameEventBus.OnSaveLoaded += SyncWithStoryFlags;
        }

        private void OnDisable()
        {
            GameEventBus.OnSaveLoaded -= SyncWithStoryFlags;
        }

        private void Start()
        {
            SyncWithStoryFlags();
        }

        public void SyncWithStoryFlags()
        {
            if (string.IsNullOrEmpty(completionFlag)) return;
            if (GameRoot.Instance == null || GameRoot.Instance.Flags == null) return;

            if (GameRoot.Instance.Flags.HasFlag(completionFlag))
            {
                _completed = true;
                for (int i = 0; i < slimes.Count; i++)
                {
                    if (slimes[i] != null)
                    {
                        slimes[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        public void NotifySlimeDefeated(GloomSlimeController defeated)
        {
            if (_completed) return;

            int alive = 0;
            for (int i = 0; i < slimes.Count; i++)
            {
                if (slimes[i] != null && !slimes[i].IsDefeated && slimes[i] != defeated)
                {
                    alive++;
                }
            }

            if (alive == 0)
            {
                _completed = true;
                if (!string.IsNullOrEmpty(completionFlag) && GameRoot.Instance != null && GameRoot.Instance.Flags != null)
                {
                    GameRoot.Instance.Flags.SetFlag(completionFlag, true);
                }
                if (!string.IsNullOrEmpty(completionToast))
                {
                    GameEventBus.RaiseToastRequested(completionToast);
                }
            }
            else
            {
                GameEventBus.RaiseToastRequested("Còn " + alive + " Bọt Sương Đen trong khu vực này!");
            }
        }
    }
}
