using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Insignia.DialogueSystem.Windows
{
    using Data.Error;
    using Data.Save;
    using Elements;
    using Utilities;
    using Enumerations;

    public class DialogueGraphView : GraphView
    {
        private DialogueEditorWindow editorWindow;
        private DialogueSearchWindow searchWindow;

        private MiniMap miniMap;

        private SerializableDictionary<string, DialogueNodeErrorData> ungroupedNodes;
        private SerializableDictionary<string, DialogueGroupErrorData> groups;
        private SerializableDictionary<Group, SerializableDictionary<string, DialogueNodeErrorData>> groupedNodes;

        private int nameErrorsAmount;
        public int NameErrorsAmount
        {
            get
            {
                return nameErrorsAmount;
            }

            set
            {
                nameErrorsAmount = value;

                if (nameErrorsAmount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (nameErrorsAmount == 1)
                {
                    editorWindow.DisableSaving();
                }
            }
        }

        public DialogueGraphView(DialogueEditorWindow _editorWindow)
        {
            editorWindow = _editorWindow;

            ungroupedNodes = new SerializableDictionary<string, DialogueNodeErrorData>();
            groups = new SerializableDictionary<string, DialogueGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DialogueNodeErrorData>>();
            
            AddManipulators();
            AddSearchWindow();
            AddMiniMap();
            AddGridBackground();

            OnElementsDeleted();
            OnGroupElemenetsAdded();
            OnGroupElemenetsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyles();
        }

        #region Overrided Methods

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port) return;
                if(startPort.node == port.node) return;
                if(startPort.direction == port.direction) return;
                
                compatiblePorts.Add(port);
                
            });
            
            return compatiblePorts;
        }

        #endregion

        #region Manipulators
        
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DialogueType.MultipleChoice));
            
            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))));
            
            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("DialogueName", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );
            
            return contextualMenuManipulator;
        }
        
        #endregion

        #region Elements Creation

        public DialogueGroup CreateGroup(string title, Vector2 position)
        {
            DialogueGroup group = new DialogueGroup(title, position);

            AddGroup(group);
            
            AddElement(group);

            foreach (GraphElement selectedElement in selection)
            {
                if (!(selectedElement is DialogueNode))
                {
                    continue;
                }

                DialogueNode node = (DialogueNode)selectedElement;
                
                group.AddElement(node);
            }

            return group;
        }

        public DialogueNode CreateNode(string nodeName, DialogueType dialogueType, Vector2 position, bool shouldDraw = true, Type objectType = null)
        {
            Type nodeType = Type.GetType($"Insignia.DialogueSystem.Elements.Dialogue{dialogueType}Node");
            
            DialogueNode node = (DialogueNode)Activator.CreateInstance(nodeType);
            
            node.Initialize(nodeName, this, position);

            if (shouldDraw)
            {
                switch (dialogueType)
                {
                    case DialogueType.SingleChoice:
                    case DialogueType.MultipleChoice:
                        node.Draw();
                        break;
                    case DialogueType.Event:
                    case DialogueType.Audio:
                        node.Draw(true, objectType);
                        break;
                }
            }

            AddUngroupedNode(node);

            return node;
        }

        #endregion

        #region Callbacks

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(DialogueGroup);
                Type edgeType = typeof(Edge);

                List<DialogueGroup> groupsToDelete = new List<DialogueGroup>();
                List<Edge> edgesToDelete = new List<Edge>();
                List<DialogueNode> nodesToDelete = new List<DialogueNode>();
                
                foreach (GraphElement element in selection)
                {
                    if (element is DialogueNode node)
                    {
                        nodesToDelete.Add(node);
                        
                        continue;
                    }

                    if (element.GetType() == edgeType)
                    {
                        Edge edge = (Edge)element;
                        
                        edgesToDelete.Add(edge);
                        
                        continue;
                    }

                    if (element.GetType() != groupType)
                    {
                        continue;
                    }

                    DialogueGroup group = (DialogueGroup) element;

                    groupsToDelete.Add(group);
                }

                foreach (DialogueGroup group in groupsToDelete)
                {
                    List<DialogueNode> groupNodes = new List<DialogueNode>();

                    foreach (GraphElement groupElement in group.containedElements)
                    {
                        if (!(groupElement is DialogueNode))
                        {
                            continue;
                        }

                        DialogueNode groupNode = (DialogueNode)groupElement;
                        
                        groupNodes.Add(groupNode);
                    }
                    
                    group.RemoveElements(groupNodes);
                    
                    RemoveGroup(group);
                    
                    RemoveElement(group);
                }
                
                DeleteElements(edgesToDelete);

                foreach (DialogueNode node in nodesToDelete)
                {
                    if (node.Group != null)
                    {
                        node.Group.RemoveElement(node);
                    }
                    
                    RemoveUngroupedNode(node);
                    
                    node.DisconnectAllPorts();
                    
                    RemoveElement(node);
                }
            };
        }

        private void OnGroupElemenetsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DialogueNode))
                    {
                        continue;
                    }

                    DialogueGroup nodeGroup = (DialogueGroup) group;
                    DialogueNode node = (DialogueNode) element;
                    
                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, nodeGroup);
                }
            };
        }

        private void OnGroupElemenetsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DialogueNode))
                    {
                        continue;
                    }

                    DialogueNode node = (DialogueNode) element;
                    
                    RemoveGroupedNode(node, group);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DialogueGroup dialogueGroup = (DialogueGroup)group;

                dialogueGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();
                
                if (string.IsNullOrEmpty(dialogueGroup.title))
                {
                    if (!string.IsNullOrEmpty(dialogueGroup.OldTitle))
                    {
                        ++NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dialogueGroup.OldTitle))
                    {
                        --NameErrorsAmount;
                    }
                }
                
                RemoveGroup(dialogueGroup);

                dialogueGroup.OldTitle = dialogueGroup.title;
                
                AddGroup(dialogueGroup);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        DialogueNode nextNode = (DialogueNode) edge.input.node;

                        DialogueChoiceSaveData choiceData = (DialogueChoiceSaveData)edge.output.userData;

                        choiceData.NodeID = nextNode.ID;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge)element;

                        DialogueChoiceSaveData choiceData = (DialogueChoiceSaveData)edge.output.userData;

                        choiceData.NodeID = "";
                    }
                }
                
                return changes;
            };
        }

        #endregion
        
        #region Repeated Elements

        public void AddUngroupedNode(DialogueNode node)
        {
            string nodeName = node.DialogueName.ToLower();

            if (!ungroupedNodes.ContainsKey(nodeName))
            {
                DialogueNodeErrorData nodeErrorData = new DialogueNodeErrorData();
                
                nodeErrorData.Nodes.Add(node);
                
                ungroupedNodes.Add(nodeName, nodeErrorData);
                
                return;
            }

            List<DialogueNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;
            
            ungroupedNodesList.Add(node);

            Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;
            
            node.SetErrorStyle(errorColor);

            if (ungroupedNodes[nodeName].Nodes.Count == 2)
            {
                ++NameErrorsAmount;
                
                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveUngroupedNode(DialogueNode node)
        {
            string nodeName = node.DialogueName.ToLower();
            
            List<DialogueNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Remove(node);
            
            node.ResetStyle();

            if (ungroupedNodesList.Count == 1)
            {
                --NameErrorsAmount;
                
                ungroupedNodesList[0].ResetStyle();
                
                return;
            }

            if (ungroupedNodesList.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
            }
        }
        
        private void AddGroup(DialogueGroup group)
        {
            string groupName = group.title.ToLower();

            if (!groups.ContainsKey(groupName))
            {
                DialogueGroupErrorData groupErrorData = new DialogueGroupErrorData();
                
                groupErrorData.Groups.Add(group);
                
                groups.Add(groupName, groupErrorData);
                
                return;
            }

            List<DialogueGroup> groupsList = groups[groupName].Groups;

            groupsList.Add(group);

            Color errorColor = groups[groupName].ErrorData.Color;
            
            group.SetErrorStyle(errorColor);

            if (groupsList.Count == 2)
            {
                ++NameErrorsAmount;
                
                groupsList[0].SetErrorStyle(errorColor);
            }
        }
        
        private void RemoveGroup(DialogueGroup group)
        {
            string oldGroupName = group.OldTitle.ToLower();

            List<DialogueGroup> groupsList = groups[oldGroupName].Groups;

            groupsList.Remove(group);
            
            group.ResetStyle();

            if (groupsList.Count == 1)
            {
                --NameErrorsAmount;
                
                groupsList[0].ResetStyle();
                
                return;
            }

            if (groupsList.Count == 0)
            {
                groups.Remove(oldGroupName);
            }
        }
        
        public void AddGroupedNode(DialogueNode node, DialogueGroup group)
        {
            string nodeName = node.DialogueName.ToLower();

            node.Group = group;

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, DialogueNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                DialogueNodeErrorData nodeErrorData = new DialogueNodeErrorData();
                
                nodeErrorData.Nodes.Add(node);
                
                groupedNodes[group].Add(nodeName, nodeErrorData);
                
                return;
            }

            List<DialogueNode> groupedNodeList = groupedNodes[group][nodeName].Nodes;
            
            groupedNodeList.Add(node);

            Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;
            
            node.SetErrorStyle(errorColor);

            if (groupedNodeList.Count == 2)
            {
                ++NameErrorsAmount;
                
                groupedNodeList[0].SetErrorStyle(errorColor);
            }
        }
        
        public void RemoveGroupedNode(DialogueNode node, Group group)
        {
            string nodeName = node.DialogueName.ToLower();

            node.Group = null;

            List<DialogueNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);
            
            node.ResetStyle();

            if (groupedNodesList.Count == 1)
            {
                --NameErrorsAmount;
                
                groupedNodesList[0].ResetStyle();
                
                return;
            }

            if (groupedNodesList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);

                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }
        }

        #endregion

        #region Elements Addition

        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DialogueSearchWindow>();
                
                searchWindow.Initialize(this);
            }

            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private void AddMiniMap()
        {
            miniMap = new MiniMap()
            {
                anchored = true
            };
            
            miniMap.SetPosition(new Rect(15, 50, 200, 100));

            Add(miniMap);

            miniMap.visible = false;
        }
        
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                "DialogueSystem/DialogueViewStyles.uss", 
                "DialogueSystem/DialogueNodeStyles.uss"
            );
        }

        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));
            StyleColor textColor = new StyleColor(new Color32(170, 170, 0, 255));

            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderLeftColor = borderColor;
            miniMap.style.borderRightColor = borderColor;

            miniMap.style.color = textColor;
        }

        #endregion

        #region Utilities

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));
            
            groups.Clear();
            groupedNodes.Clear();
            ungroupedNodes.Clear();

            NameErrorsAmount = 0;
        }

        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }

        #endregion
    }
}
