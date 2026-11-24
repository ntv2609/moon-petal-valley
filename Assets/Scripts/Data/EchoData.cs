using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonpetalValley.Data
{
    [Serializable]
    public class EchoBeat
    {
        public string speakerName;
        public string speakerId;
        [TextArea(2, 4)] public string bodyText;
        public float durationSeconds = 4.2f;
        public bool kaelPlaysAction;
        public bool maraPlaysAction;
        public Color overlayTint = new Color(0.18f, 0.34f, 0.52f, 0.36f);
    }

    [CreateAssetMenu(fileName = "Echo_New", menuName = "Moonpetal/Data/EchoData")]
    public class EchoData : ScriptableObject
    {
        public string echoID;
        public string echoTitle;
        public string requiredFlag;
        public string requiredItemId;
        public string requiredSecondaryItemId;
        public string completedFlag;
        public bool showKaelGhost = true;
        public bool showMaraGhost = true;
        [Header("Step50: Custom Ghost Prefabs")]
        public GameObject customGhostAPrefab;
        public GameObject customGhostBPrefab;
        public List<EchoBeat> beats = new List<EchoBeat>();
    }
}
