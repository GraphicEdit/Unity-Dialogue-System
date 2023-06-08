using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Insignia.DialogueSystem.Windows
{
    using Elements;
    using Enumerations;
    using Events;
    
    public class DialogueSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView graphView;
        private Texture2D indentationIcon;
        public void Initialize(DialogueGraphView _graphView)
        {
            graphView = _graphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Text Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice Text Node", indentationIcon))
                {
                    level = 2,
                    userData = DialogueType.SingleChoice
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice Text Node", indentationIcon))
                {
                    level = 2,
                    userData = DialogueType.MultipleChoice
                },

                new SearchTreeGroupEntry(new GUIContent("Dialogue Audio Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Audio Node", indentationIcon))
                {
                    level = 2,
                    userData = DialogueType.Audio
                },
                new SearchTreeEntry(new GUIContent("Single Choice Text/Audio Node", indentationIcon))
                {
                    level = 2,
                    userData = DialogueType.SingleChoice
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice Text/Audio Node", indentationIcon))
                {
                    level = 2,
                    userData = DialogueType.MultipleChoice
                },
                
                new SearchTreeGroupEntry(new GUIContent("Other Dialogue Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Event Node", indentationIcon))
                {
                    level = 2,
                    userData = DialogueType.Event
                },

                new SearchTreeGroupEntry(new GUIContent("Dialogue Group"), 1),
                new SearchTreeEntry(new GUIContent("Dialogue Group", indentationIcon))
                {
                    level = 2,
                    userData = new Group()
                }
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);
            
            switch (SearchTreeEntry.userData)
            {
                case DialogueType.SingleChoice:
                {
                    DialogueSingleChoiceNode singleChoiceNode =
                        (DialogueSingleChoiceNode)graphView.CreateNode("DialogueName", DialogueType.SingleChoice, localMousePosition);
                    
                    graphView.AddElement(singleChoiceNode);
                    
                    return true;
                }
                
                case DialogueType.MultipleChoice:
                {
                    DialogueMultipleChoiceNode multipleChoiceNode =
                        (DialogueMultipleChoiceNode)graphView.CreateNode("DialogueName", DialogueType.MultipleChoice, localMousePosition);
                    
                    graphView.AddElement(multipleChoiceNode);
                    
                    return true;
                }

                case DialogueType.Event:
                {
                    DialogueEventNode eventNode = (DialogueEventNode)graphView.CreateNode("DialogueName", DialogueType.Event, localMousePosition, true, typeof(DialogueEventSO));

                    graphView.AddElement(eventNode);
                    
                    return true;
                }
                
                case DialogueType.Audio:
                    DialogueAudioNode audioNode = (DialogueAudioNode)graphView.CreateNode("DialogueName", DialogueType.Audio, localMousePosition, true, typeof(AudioSource));

                    graphView.AddElement(audioNode);
                    
                    return true;
                
                case Group _:
                {
                    graphView.CreateGroup("DialogueGroup", localMousePosition);

                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}
