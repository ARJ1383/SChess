using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetcodePlus.Demo
{

    public class SimpleGame : SMonoBehaviour
    {
        protected void Start()
        {
            if (!TheNetwork.Get().IsActive())
            {
                //Start in test mode, when running directly from Unity Scene
                Authenticator.Get().LoginTest("Player"); //May not work with more advanced auth system, works in Test mode
                DemoConnectData cdata = new DemoConnectData(GameMode.Simple);
                TheNetwork.Get().SetConnectionExtraData(cdata);
                TheNetwork.Get().StartHost(NetworkData.Get().game_port);
            }
            if (!TheNetwork.Get().IsHost) {
                Camera.main.transform.position = new Vector3(0,6,-6);
                Camera.main.transform.rotation = Quaternion.Euler(45,0,0);
            }
        }

    }
}
