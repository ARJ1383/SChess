using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;

namespace NetcodePlus.Demo
{
    public class TankGame : SMonoBehaviour
    {
        private SNetworkActions actions;

        private bool ended = false;
        public static string name;
        public static string name2;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            if (!TheNetwork.Get().IsActive())
            {
                //Start in test mode, when running directly from Unity Scene

                if(name != null) Authenticator.Get().LoginTest(name);
                else Authenticator.Get().LoginTest(name); //May not work with more advanced auth system, works in Test mode
                DemoConnectData cdata = new DemoConnectData(GameMode.Tank);
                if (name != null) cdata.character = name;
                TheNetwork.Get().SetConnectionExtraData(cdata);
                TheNetwork.Get().StartHost(NetworkData.Get().game_port);
            }
            // if(!TheNetwork.Get().IsHost) {
            //     string host = TheNetwork.Get().Address;
            //     DemoConnectData cdata = new DemoConnectData(GameMode.Tank);
            //     if (name2 != null) cdata.character = name2;
            //     TheNetwork.Get().SetConnectionExtraData(cdata);
            //     TheNetwork.Get().StartClient(host, 7777);
            // }
            //Shuffle PlayerSpawn ids
            if (TheNetwork.Get().IsServer)
            {
                List<PlayerSpawn> spawns2 = PlayerSpawn.GetAll();
                List<PlayerSpawn> spawns = new List<PlayerSpawn>();
                spawns.Add(spawns2[0]);
                spawns.Add(spawns2[1]);
                for (int i = 0; i < spawns.Count; i++)
                    spawns[i].player_id = i;
            }
            Cursor.lockState = CursorLockMode.Locked;
            BlackPanel.Get().Show(true);
        }

        protected override void OnReady()
        {
            actions = new SNetworkActions(1); //Use NetworkID 1 for the manager
            actions.RegisterRefresh("data", DoRefresh);
            BlackPanel.Get().Hide();
        }

        protected override void OnClientReady(ulong client_id)
        {
            foreach (Tower tower in Tower.GetAll())
                tower.Refresh();
            
            //Refresh data to other players
            RefreshData();
        }

        void Update()
        {
            GameData gdata = GameData.Get();
            if (gdata == null)
                return;

            if (!TheNetwork.Get().IsReady())
                return;

            //Check for endgame
            if (!ended)
            {
                PlayerData player = gdata.GetPlayer(TheNetwork.Get().PlayerID);
                if (player != null)
                {
                    Tank tank = Tank.Get(player.player_id);
                    if (tank != null && tank.HP <= 0)
                    {
                        ended = true;
                        GameOverPanel.Get().Show("You lost!", "Your tank was destroyed");
                    }

                    int count_all = 0;
                    int count_alive = 0;

                    foreach (PlayerData aplayer in gdata.players)
                    {
                        Tank atank = Tank.Get(aplayer.player_id);
                        if (atank != null && aplayer.IsAssigned())
                        {
                            count_all += 1;
                            if (atank.HP > 0)
                                count_alive += 1;
                        }
                    }

                    if (tank != null && tank.HP > 0 && count_all >= 2 && count_alive == 1)
                    {
                        ended = true;
                        GameOverPanel.Get().Show("You won!", "You destroyed everyone else");
                    }
                }
            }
        }

        public void RefreshData()
        {
            byte[] bytes = NetworkTool.Serialize(GameData.Get());
            actions?.Refresh("data", bytes); //DoRefresh();
        }

        private void DoRefresh(byte[] sdata)
        {
            GameData gdata = NetworkTool.Deserialize<GameData>(sdata);
            GameData.Override(gdata);
        }

    }
}
