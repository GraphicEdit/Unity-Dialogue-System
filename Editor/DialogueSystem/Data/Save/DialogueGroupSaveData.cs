using System;
using UnityEngine;

namespace Insignia.DialogueSystem.Data.Save
{
    [Serializable]
    public class DialogueGroupSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}
