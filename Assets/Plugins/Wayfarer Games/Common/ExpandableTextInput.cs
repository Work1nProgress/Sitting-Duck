using System;
using TMPro;
using UnityEngine;

namespace WayfarerGames.Common
{
    public class ExpandableTextInput : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI layoutText;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TextMeshProUGUI autocompleteText;

        
        
        private void Update()
        {
            layoutText.text = input.text.Length > autocompleteText.text.Length ? input.text : autocompleteText.text;
        }
    }
}