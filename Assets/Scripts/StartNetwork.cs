using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class StartNetwork : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ipDisplay;
    [SerializeField] private TMP_InputField ipInput;
    private UnityTransport transport;

    private void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

    public void StartHost()
    {
        //retreive system ip and display it for sharing
        string systemIP = IPManager.GetIP(ADDRESSFAM.IPv4);
        ipDisplay.text = systemIP;

        SetIPAddress("0.0.0.0");
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        string ip = ipInput.text;
        SetIPAddress(ip);
        NetworkManager.Singleton.StartClient();
    }

    private void SetIPAddress(string ip)
    {
        transport.SetConnectionData(ip, transport.ConnectionData.Port);
    }
}
