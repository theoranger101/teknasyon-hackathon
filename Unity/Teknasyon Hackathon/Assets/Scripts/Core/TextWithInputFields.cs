using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Core
{
    public class TextWithInputFields : MonoBehaviour
    {
        public TextMeshProUGUI displayText;
        public TMP_InputField inputField;
        private string originalText = "This is a sample _ text with multiple _ blank spaces.";
        private List<string> textParts;
        private int currentPartIndex = 0;

        void Start()
        {
            // Split the original text by '_'
            textParts = new List<string>(originalText.Split('_'));
            // Display the initial text and set up the first input field
            UpdateTextAndInputField();
        }

        private void Update()
        {
            UpdateTextAndInputField();
        }

        void UpdateTextAndInputField()
        {
            if (currentPartIndex < textParts.Count)
            {
                // Display the text up to the next underscore
                displayText.text = string.Join("", textParts.GetRange(0, currentPartIndex + 1));
                // Enable and position the input field after the displayed text
                inputField.text = "";
                inputField.gameObject.SetActive(true);
                /*
                inputField.transform.position =
                    new Vector3(displayText.transform.position.x + displayText.preferredWidth,
                        displayText.transform.position.y, displayText.transform.position.z);
                        */
                
                Vector3 bottomLeft = displayText.textInfo.characterInfo[displayText.textInfo.characterInfo.Length - 1].bottomLeft;
                Vector3 worldBottomLeft = displayText.transform.TransformPoint(bottomLeft);
                
                Debug.Log(bottomLeft);
                Debug.Log(worldBottomLeft);
                
                Vector3 buttonSpacePos = inputField.transform.parent.InverseTransformPoint(worldBottomLeft);
                inputField.transform.localPosition = new Vector3(-worldBottomLeft.x, worldBottomLeft.y - 200, 0f);
                // worldBottomLeft; // new Vector3(50, buttonSpacePos.y - 40, 0);
                
                // inputField.transform.position = displayText.textInfo.lineInfo[0].lastVisibleCharacterIndex
                inputField.ActivateInputField();
            }
            else
            {
                // Hide the input field when all blanks are filled
                inputField.gameObject.SetActive(false);
            }
        }

        public void OnInputSubmit()
        {
            if (currentPartIndex < textParts.Count - 1)
            {
                // Insert the input into the text parts and move to the next part
                textParts.Insert(currentPartIndex + 1, inputField.text);
                currentPartIndex += 2;
                // Update the text and input field
                UpdateTextAndInputField();
            }
        }
    }
}