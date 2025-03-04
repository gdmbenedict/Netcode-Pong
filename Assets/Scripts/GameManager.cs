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
    [SerializeField] private Vector3[] playerSpawnPoints;
    [SerializeField] private TextMeshPro[] playerScores;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((hostScore.Value >= winScore || clientScore.Value >= winScore) && gameActive.Value)
        {
            EndGame();
        }

        playerScores[1].text = clientScore.Value.ToString();
        playerScores[0].text = hostScore.Value.ToString();
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
            hostScore.Value++;
            playerScores[1].text = clientScore.Value.ToString();
        }
        else
        {
            clientScore.Value++;
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
    public void StartGame()
    {
        if (!IsOwner) return;

        gameActive.Value = true;

        clientScore.Value = 0;
        hostScore.Value = 0;

        hostController.ResetPaddle(playerSpawnPoints[0]);
        clientController.ResetPaddle(playerSpawnPoints[1]);

        SpawnBall();
    }

    //Method that connects a paddle controller to the game manager
    public void SetPaddleController(bool isHost, PaddleController paddle)
    {
        if (isHost)
        {
            hostController = paddle;
        }
        else
        {
            clientController = paddle;
        }
    }

    public void SetPaddleController(PaddleController paddle)
    {
        if (hostController == null)
        {
            hostController = paddle;
        }
        else
        {
            clientController = paddle;
        }
    }

    public Vector3 GetSpawnPoint(int index)
    {
        return playerSpawnPoints[index];
    }
}
