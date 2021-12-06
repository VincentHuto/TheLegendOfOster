using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player.UI
{
    public class LevelText : MonoBehaviour
    {
        public Text txt;

        private void Start()
        {
            txt = GetComponentInChildren<Text>();
        }
        public void SetText(string text)
        {
            txt.text = text;
        }

    }
}