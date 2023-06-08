using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Insignia.DialogueSystem.Elements
{
    using Data.Save;
    using Utilities;
    using Windows;
    using Enumerations;
    using Events;
    
    public class DialogueNode : Node
    {
        public string ID { get; set; }
        public string DialogueName { get; set; }
        public List<DialogueChoiceSaveData> Choices { get; set; }
        public DialogueEventSO DialogueEvent { get; set; }
        public AudioClip DialogueAudio{ get; set; }
        public string Text { get; set; }
        public DialogueType DialogueType { get; set; }
        public DialogueGroup Group { get; set; }

        protected DialogueGraphView graphView;
        private Color defaultBackgroundColor;

        public virtual void Initialize(string nodeName, DialogueGraphView _graphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            Choices = new List<DialogueChoiceSaveData>();
            Text = "Dialogue Text.";

            graphView = _graphView;

            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
            
            SetPosition(new Rect(position, Vector2.zero));
            
            mainContainer.AddToClassList("dialogue-node__main-container");
            extensionContainer.AddToClassList("dialogue-node__extension-container");
        }

        public virtual void Draw(bool isObjectType = false, Type objectType = null)
        {
            /* TITLE CONTAINER */
            TextField dialogueNameTextField = DialogueElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                    {
                        ++graphView.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DialogueName))
                    {
                        --graphView.NameErrorsAmount;
                    }
                }

                if(Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    DialogueName = target.value;
                
                    graphView.AddUngroupedNode(this);
                    
                    return;
                }

                DialogueGroup currentGroup = Group;
                
                graphView.RemoveGroupedNode(this, Group);

                DialogueName = callback.newValue;
                
                graphView.AddGroupedNode(this, currentGroup);
            });

            dialogueNameTextField.AddClasses(
                "dialogue-node__text-field",
                "dialogue-node__filename-text-field",
                "dialogue-node__text-field__hidden"
            );

            titleContainer.Insert(0, dialogueNameTextField);

            /* INPUT CONTAINER */
            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input,
                Port.Capacity.Multi);
            
            inputContainer.Add(inputPort);

            /* EXTENSIONS CONTAINER */
            VisualElement customDataContainer = new VisualElement();
            
            customDataContainer.AddToClassList("dialogue-node__custom-data-container");

            Foldout textFoldout = DialogueElementUtility.CreateFoldout("Dialogue");

            if (!isObjectType)
            {
                TextField textTextField = DialogueElementUtility.CreateTextArea(Text, null, callback =>
                {
                    Text = callback.newValue;
                });
                
                textTextField.AddClasses(
                    "dialogue-node__text-field",
                    "dialogue-node__quote-text-field"
                );
                
                textFoldout.Add(textTextField);
            }
            else
            {
                ObjectField objectField = null;

                switch (DialogueType)
                {
                    case DialogueType.Event:
                        objectField = CreateEventObjectField();
                        break;
                    case DialogueType.Audio:
                        objectField = CreateAudioObjectField("Audio");
                        break;
                }
                
                objectField.AddClasses(
                    "dialogue-node__text-field",
                    "dialogue-node__quote-text-field"
                );
                
                textFoldout.Add(objectField);
            }

            customDataContainer.Add(textFoldout);
            
            extensionContainer.Add(customDataContainer);
        }

        #region ObjectFields

        private ObjectField CreateEventObjectField()
        {
            ObjectField objectField = DialogueElementUtility.CreateObjectField(DialogueType.ToString(), typeof(DialogueEventSO));
                
            objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(callback =>
            {
                DialogueEvent = (DialogueEventSO)callback.newValue;
            });

            if (DialogueEvent != null)
                objectField.value = DialogueEvent;

            return objectField;
        }
        
        private ObjectField CreateAudioObjectField(string label = null)
        {
            ObjectField objectField = DialogueElementUtility.CreateObjectField(label, typeof(AudioClip));
                
            objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(callback =>
            {
                DialogueAudio = (AudioClip)callback.newValue;
            });

            if (DialogueAudio != null)
                objectField.value = DialogueAudio;

            return objectField;
        }

        #endregion

        #region Overrided Methods

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());
            
            base.BuildContextualMenu(evt);
        }

        #endregion
        
        #region Utility Methods

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
            
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }
        
        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }
                
                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            Port inputPort = (Port)inputContainer.Children().First();

            return !inputPort.connected;
        }
        
        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }

        #endregion
    }
}
