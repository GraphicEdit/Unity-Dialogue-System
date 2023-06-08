using UnityEngine;

namespace Insignia.DialogueSystem.Data.Error
{
    public class DialogueErrorData
    {
        public Color Color { get; set; }

        public DialogueErrorData()
        {
            GenerateRandomColor();
        }
        
        private void GenerateRandomColor()
        {
            Color = new Color32(
                (byte) Random.Range(65, 255),
                (byte) Random.Range(50, 175),
                (byte) Random.Range(50, 175),
                255
            );
        }
    }
}
