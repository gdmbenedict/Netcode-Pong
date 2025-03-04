using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
    [Header("Game Variables")]
    private NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false);
    [SerializeField] private int winScore = 10;
    
    [Header("Player Variables")]
    [SerializeField] private List<Vector3> playerSpawnPoints;
    [SerializeField] private List<TextMeshPro> playerScores;
    private NetworkVariable<int> hostScore = new NetworkVariable<int>(0);
    private NetworkVariable<int> clientScore = new NetworkVariable<int>(0);
    private PaddleController hostController;
    private PaddleController clientController;

    [Header("Ball Variables")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Vector3 ballSpawnPoint;
    private GameObject ballInstance;
    private NetworkObject ballNetworkObject;

    private static GameManager _instance;

    //function that checks if Instance exists and spawns one if it does not
    public static GameManager instance
    {
        get
        {
            _instance = FindObjectOfType<GameManager>();  // Try to find an existing AudioManager in the scene

            //check if Instance is null
            if (_instance == null)
            {
                // If no Instance exists, instantiate it
                _instance = Instantiate(Resources.Load("GameManager") as GameObject).GetComponent<GameManager>();
                _instance.name = "GameManager";
            }
            return _instance;
        }
    }

    // Awake is called before the first frame update and before start
    void Awake()
    {
        //check if this is the active Instance
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //remove copy
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HostConnected;
        NetworkManager.Singleton.OnConnectionEvent += ClientConnected;
    }

    // Update is called once per frame
    void Update()
    {
        if ((hostScore.Value >= winScore || clientScore.Value >= winScore) && gameActive.Value)
        {
            EndGame();
        }
    }

    //Method that spawns a ball to be used for the game.
    private void SpawnBall()
    {
        //instantiate then spawn using the NetworkObject to sync across games
        ballInstance = Instantiate(ballPrefab,ballSpawnPoint, Quaternion.identity);
        ballNetworkObject = ballInstance.GetComponent<NetworkObject>();
        ballNetworkObject.Spawn(true);
    }

    //Method that handles one of the players scoring against the other
    public void Score(bool hostGoal)
    {
        if (hostGoal)
        {
            clientScore.Value++;
            playerScores[1].text = clientScore.Value.ToString();
        }
        else
        {
            hostScore.Value++;
            playerScores[0].text = clientScore.Value.ToString();
        }

        if (hostScore.Value >= winScore || clientScore.Value >= winScore)
        {
            EndGame();
        }
        else
        {
            SpawnBall();
        }
    }

    //Method that ends the game
    private void EndGame()
    {
        gameActive.Value = false;
    }

    //Method that starts the game.
    private void StartGame()
    {
        gameActive.Value = true;

        clientScore.Value = 0;
        hostScore.Value = 0;

        playerScores[1].text = clientScore.Value.ToString();
        playerScores[0].text = clientScore.Value.ToString();

        hostController.ResetPaddle(playerSpawnPoints[0]);
        clientController.ResetPaddle(playerSpawnPoints[1]);

        SpawnBall();
    }

    private void HostConnected()
    {
        Debug.Log("Host Event Called");
    }

    private void ClientConnected(NetworkManager manager, ConnectionEventData data)
    {
        Debug.Log("Client Event Called");
    }
}
