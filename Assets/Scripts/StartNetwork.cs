using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartNetwork : MonoBehaviour
{
    //Class contains methods to allow access to static instance of network manager through buttons
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
