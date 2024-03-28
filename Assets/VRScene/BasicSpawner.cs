using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.Windows.Speech;
using System.Linq;

public class BasicSpawner : FusionMonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private KeywordRecognizer KeywordRecognizer;
    private Dictionary<string, Action> Actions = new Dictionary<string, Action>();

    private Controls Inputs;
    private void OnEnable()
    {
       //Actions.Add("up", MoveForward);
       //Actions.Add("down", MoveBackward);
       //Actions.Add("right", MoveRight);
       //Actions.Add("left", MoveLeft);
       //Actions.Add("jump", Jump);
       //Actions.Add("speak coordinates", SpeakCoordinates);
       //Actions.Add("speak landmarks", SpeakLandmarks);
       //Actions.Add("speak ambiant sounds", SpeakAmbientSounds);
       //Actions.Add("toggle ambiant sounds", ToggleAmbientSounds);
       //KeywordRecognizer = new KeywordRecognizer(Actions.Keys.ToArray());
       //KeywordRecognizer.OnPhraseRecognized += Recognized;
       //KeywordRecognizer.Start();
        Inputs = new Controls();
        Inputs.PlayerInputs.Enable();
    }
     private void Recognized(PhraseRecognizedEventArgs Word)
    {

        Debug.Log(Word.text);
        Actions[Word.text].Invoke();
    }
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
  
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.LogWarning("called");
        Vector3 spawnPosition = transform.position;
        NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        //Camera.SetupCurrent(networkPlayerObject.GetComponent<Camera>());
        // Keep track of the player avatars for easy access
        Debug.LogWarning(networkPlayerObject.transform.name);
        _spawnedCharacters.Add(player, networkPlayerObject);
        foreach (PlayerRef Player in _spawnedCharacters.Keys)
        {
            Debug.LogWarning("checked if player Camera");
            if (runner.LocalPlayer != Player)
            {
                Debug.LogWarning("disabled Camera");
                networkPlayerObject.GetComponentInChildren<Camera>().enabled = false;
            }
        }
      
      // if (player.IsMasterClient)
      // {
      //     Debug.LogWarning("called when server");
      //     // Create a unique position for the player
      //    
      //     foreach(PlayerRef Player in _spawnedCharacters.Keys)
      //     {
      //         if (Player != player)
      //         {
      //             Debug.LogWarning("disabled Camera");
      //             networkPlayerObject.GetComponentInChildren<Camera>().enabled = false;
      //         }
      //     }
      // }
      // if (!player.IsMasterClient)
      // {
      //     Debug.LogWarning("called player isnt master client Camera");
      //     foreach (PlayerRef Player in _spawnedCharacters.Keys)
      //     {
      //         if (Player != player)
      //         {
      //             Debug.Log("disabled Camera");
      //             networkPlayerObject.GetComponentInChildren<Camera>().enabled = false;
      //         }
      //     }
      // }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    
    public void OnInput(NetworkRunner runner, NetworkInput input)
    { 
        
       
        var data = new NetworkInputData();
        var PlayerActions = Inputs.PlayerInputs;

        data.direction = PlayerActions.Move.ReadValue<Vector2>();

        //Debug.LogWarning(data.direction);
       //if (Input.GetKey(KeyCode.W))
       //    data.direction += Vector3.forward;
       // 
       // if (Input.GetKey(KeyCode.S))
       //     data.direction += Vector3.back;
       // 
       // if (Input.GetKey(KeyCode.A))
       //     data.direction += Vector3.left;
       // 
       // if (Input.GetKey(KeyCode.D))
       //     data.direction += Vector3.right;
        
        input.Set(data);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

}