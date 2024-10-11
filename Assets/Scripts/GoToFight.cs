using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetcodePlus;
using NetcodePlus.Demo;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class GoToFight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TheNetwork.tank = true;
            TankGame.name = "WhitePawn";
            GameMode mode = GameMode.Tank;
            GameModeData gmdata = GameModeData.Get(mode);
            DemoConnectData cdata = new DemoConnectData(mode);
            TheNetwork.Get().SetConnectionExtraData(cdata);
            TheNetwork.Get().LoadScene(gmdata.scene);
        }
    }

    async void changeGame(string scene) {
            TheNetwork.Get().Disconnect();
            BlackPanel.Get().Show();
            await Task.Yield(); //Wait a frame after the disconnect
            TheNetwork.Get().LoadScene(scene);
    }
}
