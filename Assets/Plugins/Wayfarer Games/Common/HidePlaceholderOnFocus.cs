using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WayfarerGames.Common
{
    public class HidePlaceholderOnFocus : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI placeholder;
        public void OnFocus()
        {
            placeholder.text = "";
        }
    }
}