using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetcodePlus;
using NetcodePlus.Demo;
using System.Threading.Tasks;

public class GoToFight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) {
            GameMode mode = GameMode.Tank;
            GameModeData gmdata = GameModeData.Get(mode);
            DemoConnectData cdata = new DemoConnectData(mode);
            cdata.character = "BlackKing";
            TheNetwork.Get().SetConnectionExtraData(cdata);
            TankGame.name = "BlackKing";
            TheNetwork.Get().LoadScene(gmdata.scene);

            //hangeGame(gmdata.scene);
        }
    }

    async void changeGame(string scene) {
            BlackPanel.Get().Show();
            await Task.Yield(); //Wait a frame after the disconnect
            TheNetwork.Get().LoadScene(scene);
    }
}
