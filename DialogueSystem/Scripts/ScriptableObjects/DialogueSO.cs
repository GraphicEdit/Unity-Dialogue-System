using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Insignia.DialogueSystem.ScriptableObjects
{
    using Data;
    using Enumerations;
    using Events;
    
    public class DialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<DialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DialogueEventSO DialogueEvent { get; set; }
        [field: SerializeField] public AudioClip DialogueAudio { get; set; }
        [field: SerializeField] public DialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }

        public void Initalize(string dialogueName, string text, DialogueType dialogueType,
            bool isStartingDialogue, List<DialogueChoiceData> choices = null, DialogueEventSO dialogueEvent = null, AudioClip dialogueAudio = null)
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            DialogueEvent = dialogueEvent;
            DialogueAudio = dialogueAudio;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
        }
    }
}
