using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using NetcodePlus;
using UnityEngine.SceneManagement;

namespace NetcodePlus.Demo
{

    public class Menu : MonoBehaviour
    {
        [Header("Panels")]
        public UIPanel main_panel;
        public UIPanel create_panel;
        public UIPanel load_panel;
        public UIPanel join_panel;

        [Header("Create")]
        public Text create_ip;
        public InputField create_user;

        [Header("Join")]
        public InputField join_user;
        public InputField join_host;

        public ushort port;

        public static string username = null;
        public static string last_menu = "Menu";

        private static Menu instance;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            main_panel.Show();
            port = NetworkData.Get().game_port;
            create_ip.text = "LAN IP: " + NetworkTool.GetLocalIp();
            last_menu = SceneManager.GetActiveScene().name;

            LoadUser();
        }

        private void SaveUser(string user)
        {
            Menu.username = user;
        }

        private void LoadUser()
        {
            string name = Menu.username;
            if (name == null && DemoData.Get() != null)
                name = DemoData.Get().GetRandomName();
            if (name == null)
                name = "player";

            create_user.text = name;
            join_user.text = name;
        }

        public void OnClickGoToCreate()
        {
            main_panel.Hide();
            create_panel.Show();
        }

        public void OnClickGoToLoad()
        {
            main_panel.Hide();
            load_panel.Show();
        }

        public void OnClickGoToJoin()
        {
            main_panel.Hide();
            join_panel.Show();
        }

        public void OnClickGoToMain()
        {
            main_panel.Show();
            create_panel.Hide();
            load_panel.Hide();
            join_panel.Hide();
        }

        public void OnClickCreate()
        {
            CreateGame(create_user.text);
        }

        public void OnClickJoin()
        {
            JoinGame(join_user.text, join_host.text);
        }

        public void CreateGame(string user)
        {
            ChessLogic.GetInstance().ClearBoard();
            GameModeData mdata = GameModeData.Get(GameMode.Simple);
            if (SceneNav.DoSceneExist(mdata.scene))
            {
                DemoConnectData cdata = new DemoConnectData(GameMode.Simple);
                TheNetwork.Get().SetConnectionExtraData(cdata);
                GameManager.gameModeData = mdata;
                SaveUser(user);
                CreateTask(user, mdata.scene);
            }
        }

        public void JoinGame(string user, string host)
        {
            ChessLogic.GetInstance().ClearBoard();
            DemoConnectData cdata = new DemoConnectData();
            cdata.character = "BlackKnight";
            TheNetwork.Get().SetConnectionExtraData(cdata);
            SaveUser(user);
            JoinTask(user, host);
        }

        private async void CreateTask(string user, string scene)
        {
            TheNetwork.Get().Disconnect();
            BlackPanel.Get().Show();
            await Task.Yield(); //Wait a frame after the disconnect
            Authenticator.Get().LoginTest(user);
            TheNetwork.Get().StartHost(port);
            TheNetwork.Get().LoadScene(scene);
        }

        private async void JoinTask(string user, string host)
        {
            TheNetwork.Get().Disconnect();
            ConnectingPanel.Get().Show();
            await Task.Yield(); //Wait a frame after the disconnect
            Authenticator.Get().LoginTest(user);
            TheNetwork.Get().StartClient(host, port);
        }

        public static void GoToSimpleMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        public static void GoToLobbyMenu()
        {
            SceneManager.LoadScene("MenuLobby");
        }

        public static void GoToLastMenu()
        {
            SceneManager.LoadScene(last_menu);
        }

        public static Menu Get()
        {
            return instance;
        }
    }
}
