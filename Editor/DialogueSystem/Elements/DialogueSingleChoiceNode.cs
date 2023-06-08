using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Insignia.DialogueSystem.Elements
{
    using Data.Save;
    using Utilities;
    using Windows;
    using Enumerations;
    
    public class DialogueSingleChoiceNode : DialogueNode
    {
        public override void Initialize(string nodeName, DialogueGraphView graphView, Vector2 position)
        {
            base.Initialize(nodeName, graphView, position);

            DialogueType = DialogueType.SingleChoice;

            DialogueChoiceSaveData choiceData = new DialogueChoiceSaveData()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choiceData);
        }

        public override void Draw(bool isObjectType = false, Type objectType = null)
        {
            base.Draw();

            /* OUTPUT CONTAINER */
            foreach (DialogueChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }
            
            RefreshExpandedState();
        }
    }
}
