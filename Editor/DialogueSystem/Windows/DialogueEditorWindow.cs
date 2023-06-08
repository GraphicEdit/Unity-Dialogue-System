using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Insignia.DialogueSystem.Windows
{
    using Utilities;
    
    public class DialogueEditorWindow : EditorWindow
    {
        private DialogueGraphView graphView;
        
        private readonly string defaultFileName = "NewDialogue";
        
        private static TextField fileNameTextField;
        private Button saveButton;
        private Button miniMapButton;
        
        [MenuItem("Window/Insignia Interactive/Dialogue Graph")]
        public static void Open()
        {
            GetWindow<DialogueEditorWindow>("Dialogue Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();

            AddStyles();
        }

        #region Elements Addition

        private void AddGraphView()
        {
            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            
            rootVisualElement.Add(graphView);
        }
        
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = DialogueElementUtility.CreateTextField(defaultFileName, "File Name:",
            callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = DialogueElementUtility.CreateButton("Save", () => Save());
            
            Button loadButton = DialogueElementUtility.CreateButton("Load", () => Load());
            Button clearButton = DialogueElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = DialogueElementUtility.CreateButton("Reset", () => ResetGraph());
            miniMapButton = DialogueElementUtility.CreateButton("Minimap", () => ToggleMiniMap());
            
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(miniMapButton);

            toolbar.AddStyleSheets(
                "DialogueSystem/DialogueToolbarStyles.uss"
            );
            
            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets(
                "DialogueSystem/DialogueVariables.uss"
            );
        }

        #endregion

        #region Toolbar Actions

        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog(
                    "Invalid file name.",
                    "Please ensure the file name you've typed in is valid.",
                    "OK"
                );
                
                return;
            }
            
            DialogueIOUtility.Initialize(graphView, fileNameTextField.value);
            DialogueIOUtility.Save();
        }

        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DialogueSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            
            Clear();
            
            DialogueIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            DialogueIOUtility.Load();
        }

        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();
            
            UpdateFileName(defaultFileName);
        }

        private void ToggleMiniMap()
        {
            graphView.ToggleMiniMap();
            
            miniMapButton.ToggleInClassList("dialogue-toolbar__button__selected");
        }

        #endregion

        #region Utility Methods

        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }
        
        public void EnableSaving()
        {
            saveButton.SetEnabled(true);   
        }
        
        public void DisableSaving()
        {
            saveButton.SetEnabled(false);   
        }

        #endregion
    }
}