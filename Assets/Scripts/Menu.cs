using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlienMaker
{
    public class Menu : Singleton<Menu> {

        public Button[] buttons;
        private int currentSelected = 0;
        private int numButtons;
        private List<Settings.GameSetup> setups;

        public void ResetSeletion()
        {
            currentSelected = 0;
            UpdateButtons();
        }

        public void InitializeButtons()
        {
            // Hide all buttons
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(false);
            }

            setups = Settings.Instance.setups;
            for (int i = 0; i < setups.Count; ++i)
            {
                if (i < buttons.Length)
                {
                    Text buttonText = buttons[i].gameObject.GetComponentInChildren<Text>();
                    buttonText.text = setups[i].name;
                    int index = i;
                    //buttons[i].onClick.AddListener(delegate { Manager.Instance.startGame(setups[index].id); });
                    buttons[i].gameObject.SetActive(true);
                }
            }
        }

        public void Start()
        {
            currentSelected = 0;
            buttons[currentSelected].gameObject.GetComponent<Image>().color = new Color(256.0f / 256.0f, 256.0f / 256.0f, 256.0f / 256.0f);
        }

        public void Update()
        {
            if (setups.Count == 0)
            {
                return;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                currentSelected = (currentSelected + 1) % setups.Count;
                UpdateButtons();
            }

            if (Input.GetKeyUp(KeyCode.Return))
            {
                Manager.Instance.startGame(setups[currentSelected].id);
            }
        }

        private void UpdateButtons()
        {
            for (int i = 0; i < buttons.Length; ++i)
            {
                if (i == currentSelected)
                {
                    buttons[i].gameObject.GetComponent<Image>().color = new Color(256.0f / 256.0f, 256.0f / 256.0f, 256.0f / 256.0f);
                } else
                {
                    buttons[i].gameObject.GetComponent<Image>().color = new Color(100 / 256.0f, 100 / 256.0f, 100 / 256.0f);
                }
            }
        }
    }
}
