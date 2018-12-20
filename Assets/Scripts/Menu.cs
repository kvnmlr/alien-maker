using System.Collections.Generic;
using UnityEngine.UI;

namespace AlienMaker
{
    public class Menu : Singleton<Menu> {

        public Button[] buttons;

        public void initializeButtons()
        {
            // Hide all buttons
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(false);
            }

            List<Settings.GameSetup> setups = Settings.Instance.setups;
            for (int i = 0; i < setups.Count; ++i)
            {
                if (i < buttons.Length)
                {
                    Text buttonText = buttons[i].gameObject.GetComponentInChildren<Text>();
                    buttonText.text = setups[i].name;
                    int index = i;
                    buttons[i].onClick.AddListener(delegate { Manager.Instance.startGame(setups[index].id); });
                    buttons[i].gameObject.SetActive(true);
                }
            }

        }
    }
}
