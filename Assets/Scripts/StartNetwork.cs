using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class StartNetwork : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ipDisplay;
    //[SerializeField] private TMP_InputField ipInput;
    [SerializeField] private string ip;
    private UnityTransport transport;

    private void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

    public void StartHost()
    {
        transport.SetConnectionData("0.0.0.0", transport.ConnectionData.Port);
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        string ip = this.ip;
        SetIPAddress(ip);
        NetworkManager.Singleton.StartClient();
    }

    private void SetIPAddress(string ip)
    {
        transport.ConnectionData.Address = ip;
        transport.ConnectionData.Port = 7777;
    }
}
