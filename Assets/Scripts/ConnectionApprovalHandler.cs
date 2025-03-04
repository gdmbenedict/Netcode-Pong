using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionApprovalHandler : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 2;

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    //Method that handles approving a client connection
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Approval Check Called");

        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;

        if (NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers)
        {
            response.Approved = false;
            response.Reason = "Game is Full";
        }

        response.Pending = false;
    }
}
