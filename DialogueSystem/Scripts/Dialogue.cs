using UnityEngine;

namespace Insignia.DialogueSystem
{
    using ScriptableObjects;
    
    public class Dialogue : MonoBehaviour
    {
        /* Dialogue Scriptable Objects */
        [SerializeField] private DialogueContainerSO dialogueContainer;
        [SerializeField] private DialogueGroupSO dialogueGroup;
        [SerializeField] private DialogueSO dialogue;

        /* Filters */
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;
        
        /* Indexes */
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;
    }
}
