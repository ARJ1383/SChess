using UnityEngine;
using UnityEngine.UI;

namespace NetcodePlus.Demo
{
    public enum Language
    {
        English,
        Persian
    }
    public class LanguageManager : MonoBehaviour
    {
        private Language _language;
        
        
        public Text CreatorsOfGameText;
        public Text CreateGameText;
        public Text JoinGameText;
        public Text HostGameText;
        public Text NameLabelText;
        public Text StartButtonText;
        public Text JoinGameMenuText;
        public Text NameLabelJoinMenuText;
        public Text HostLabelText;
        public Text JoinButtonText;
        
        public void ChangeLanguageToEnglish()
        {
            SwitchLanguage(Language.English);
        }

        public void ChangeLanguageToPersian()
        {
            SwitchLanguage(Language.Persian);
        }

        private void SwitchLanguage(Language language)
        {
            _language = language;
            if (_language == Language.English)
            {
                CreatorsOfGameText.text = "Made by Virtual Visionaries";
                CreateGameText.text = "CREATE";
                JoinGameText.text = "JOIN";
                HostGameText.text = "HOST GAME";
                NameLabelText.text = "Username";
                StartButtonText.text = "START";
                JoinGameMenuText.text = "JOIN GAME";
                NameLabelJoinMenuText.text = "Username";
                HostLabelText.text = "Host";
                JoinButtonText.text = "JOIN";
            }
            else
            {
                CreatorsOfGameText.text = "Virtual Visionaries  ﻢﯿﺗ ﻂﺳﻮﺗ ﻩﺪﺷ ﻪﺘﺧﺎﺳ ";
                CreateGameText.text = "ىﺯﺎﺑ ﻦﺘﺧﺎﺳ ";
                JoinGameText.text = "ىﺯﺎﺑ ﻪﺑ ﻦﺘﺳﻮﯿﭘ ";
                HostGameText.text = "ىﺯﺎﺑ ﻥﺎﺑﺰﯿﻣ ";
                NameLabelText.text = "ىﺮﺑﺭﺎﻛ  ﻡﺎﻧ ";
                StartButtonText.text = "ىﺯﺎﺑ ﺯﺎﻏﺁ ";
                JoinGameMenuText.text = "ىﺯﺎﺑ ﻪﺑ ﻦﺘﺳﻮﯿﭘ ";
                NameLabelJoinMenuText.text = "ىﺮﺑﺭﺎﻛ  ﻡﺎﻧ ";
                HostLabelText.text = "ﻥﺎﺑﺰﯿﻣ ";
                JoinButtonText.text = "ﻦﺘﺳﻮﯿﭘ ";
            }
        }
    }
}