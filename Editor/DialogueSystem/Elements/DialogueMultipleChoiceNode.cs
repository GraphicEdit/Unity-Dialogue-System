using System;
using Insignia.DialogueSystem.Enumerations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Insignia.DialogueSystem.Elements
{
    using Data.Save;
    using Utilities;
    using Windows;
    
    public class DialogueMultipleChoiceNode : DialogueNode
    {
        public override void Initialize(string nodeName, DialogueGraphView graphView, Vector2 position)
        {
            base.Initialize(nodeName, graphView, position);

            DialogueType = DialogueType.MultipleChoice;

            DialogueChoiceSaveData choiceData = new DialogueChoiceSaveData()
            {
                Text = "New Choice"
            };
            
            Choices.Add(choiceData);
        }

        public override void Draw(bool isObjectType = false, Type objectType = null)
        {
            base.Draw();
            
            /* MAIN CONTAINER */
            Button addChoiceButton = DialogueElementUtility.CreateButton("Add Choice", () =>
            {
                DialogueChoiceSaveData choiceData = new DialogueChoiceSaveData()
                {
                    Text = "New Choice"
                };
            
                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);
                
                outputContainer.Add(choicePort);
            });
            
            addChoiceButton.AddToClassList("dialogue-node__button");
            
            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */
            foreach (DialogueChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);
                
                outputContainer.Add(choicePort);
            }
            
            RefreshExpandedState();
        }

        #region Elements Creation

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DialogueChoiceSaveData choiceData = (DialogueChoiceSaveData) userData;

            Button deleteChoiceButton = DialogueElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choiceData);
                
                graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("dialogue-node__button");

            TextField choiceTextField = DialogueElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses(
                "dialogue-node__text-field",
                "dialogue-node__choice-text-field",
                "dialogue-node__text-field__hidden"
            );

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            return choicePort;
        }

        #endregion
    }
}