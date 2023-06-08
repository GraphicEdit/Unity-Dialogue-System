using System;
using System.Collections.Generic;
using UnityEngine;

namespace Insignia.DialogueSystem.Data.Save
{
    using Enumerations;
    using Events;
    
    [Serializable]
    public class DialogueNodeSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public List<DialogueChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public DialogueEventSO DialogueEvent { get; set; }
        [field: SerializeField] public AudioClip DialogueAudio { get; set; }
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public DialogueType DialogueType { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}
