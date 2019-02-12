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

        public void initializeButtons()
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
            buttons[currentSelected].gameObject.GetComponent<Image>().color = new Color(30/256.0f, 80 / 256.0f, 180 / 256.0f);
        }

        public void Update()
        {
            if (setups.Count == 0)
            {
                return;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                buttons[currentSelected].gameObject.GetComponent<Image>().color = new Color(20 / 256.0f, 20 / 256.0f, 20 / 256.0f);
                currentSelected = (currentSelected + 1) % setups.Count;
                buttons[currentSelected].gameObject.GetComponent<Image>().color = new Color(30 / 256.0f, 80 / 256.0f, 180 / 256.0f);
            }

            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (!Manager.Instance.currentSetupFinished)
                {
                    Manager.Instance.startGame(setups[currentSelected].id);
                }
            }
        }

    }
}
