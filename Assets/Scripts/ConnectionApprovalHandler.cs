using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionApprovalHandler : NetworkBehaviour
{
    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private float startDelay = 1;
    private int playerNum = 0;

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

        Debug.Log(NetworkManager.Singleton.IsServer);

        if (NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers)
        {
            response.Approved = false;
            response.Reason = "Game is Full";
        }
        else
        {
            /*
            if (IsServer)
            {
                response.Position = GameManager.instance.GetSpawnPoint(0);
            }
            else
            {
                response.Position = GameManager.instance.GetSpawnPoint(0);
                GameManager.instance.StartGame();
            }
            */

            //Change this to isServer check
            response.Position = GameManager.instance.GetSpawnPoint(playerNum % 2);
            playerNum++;

            if (0 == playerNum % 2)
            {
                StartCoroutine(StartCall());
            }
            
        }  

        response.Pending = false;
    }

    private IEnumerator StartCall()
    {
        yield return new WaitForSeconds(startDelay);
        GameManager.instance.StartGame();
    }
}
