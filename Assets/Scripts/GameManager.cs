using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
    [Header("Player Variables")]
    [SerializeField] private List<Position> playerSpawnPoints;

    [Header("Ball Variables")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Vector3 ballSpawnPoint;
    private GameObject ballInstance;
    private NetworkObject ballNetworkObject;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Method that spawns a ball to be used for the game.
    private void SpawnBall()
    {
        //instantiate then spawn using the NetworkObject to sync across games
        ballInstance = Instantiate(ballPrefab,ballSpawnPoint, Quaternion.identity);
        ballNetworkObject = ballInstance.GetComponent<NetworkObject>();
        ballNetworkObject.Spawn(true);
    }
}
