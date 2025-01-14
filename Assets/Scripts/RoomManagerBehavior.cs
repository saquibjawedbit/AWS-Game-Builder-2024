using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManagerBehavior : NetworkBehaviour
{
    public static RoomManagerBehavior Instance { get; private set; }
    private readonly SyncDictionary<string, NetworkRoomInfo> rooms = new SyncDictionary<string, NetworkRoomInfo>();
    [SerializeField] private int maxPlayersPerRoom = 4;

    // Track connection state
    public bool IsConnected => NetworkClient.isConnected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Register connection/disconnection callbacks
        NetworkClient.OnConnectedEvent += OnConnectedToServer;
        NetworkClient.OnDisconnectedEvent += OnDisconnectedFromServer;
    }

    private void OnDestroy()
    {
        // Unregister callbacks to prevent memory leaks
        NetworkClient.OnConnectedEvent -= OnConnectedToServer;
        NetworkClient.OnDisconnectedEvent -= OnDisconnectedFromServer;
    }

    private void OnConnectedToServer()
    {
        Debug.Log("Connected to server!");
    }

    private void OnDisconnectedFromServer()
    {
        Debug.Log("Disconnected from server!");
    }

    public struct NetworkRoomInfo
    {
        public string roomName;
        public int currentPlayers;
        public int maxPlayers;
        public bool isOpen;
    }

    // Client-side method to request room creation
    public void RequestCreateRoom(string roomName)
    {
        if (!NetworkClient.isConnected)
        {
            Debug.Log("Connecting to server first...");
            NetworkClient.Connect("localhost"); // Or your server address
            // Store the room creation request to execute after connection
            StartCoroutine(WaitForConnectionAndCreateRoom(roomName));
            return;
        }

        CmdCreateRoom(roomName);
    }

    private System.Collections.IEnumerator WaitForConnectionAndCreateRoom(string roomName)
    {
        // Wait for connection
        while (!NetworkClient.isConnected)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Once connected, create the room
        CmdCreateRoom(roomName);
    }

    // Client-side method to request joining a room
    public void RequestJoinRoom(string roomName)
    {
        if (!NetworkClient.isConnected)
        {
            Debug.Log("Connecting to server first...");
            NetworkClient.Connect("localhost"); // Or your server address
            // Store the join request to execute after connection
            StartCoroutine(WaitForConnectionAndJoinRoom(roomName));
            return;
        }

        CmdJoinRoom(roomName);
    }

    private System.Collections.IEnumerator WaitForConnectionAndJoinRoom(string roomName)
    {
        // Wait for connection
        while (!NetworkClient.isConnected)
        {
            yield return new WaitForSeconds(0.1f);
        }
       
        // Once connected, join the room
        CmdJoinRoom(roomName);
    }

    [Command(requiresAuthority = false)]
    private void CmdCreateRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Room name cannot be empty");
            return;
        }

        if (rooms.ContainsKey(roomName))
        {
            Debug.LogError($"Room {roomName} already exists!");
            return;
        }

        NetworkRoomInfo newRoom = new NetworkRoomInfo
        {
            roomName = roomName,
            currentPlayers = 0,
            maxPlayers = maxPlayersPerRoom,
            isOpen = true
        };
        SceneManager.LoadScene(1);
        rooms[roomName] = newRoom;
        Debug.Log($"Room {roomName} created successfully!");
        // Automatically join after creating
        CmdJoinRoom(roomName);
    }

    [Command(requiresAuthority = false)]
    private void CmdJoinRoom(string roomName)
    {
        if (!rooms.ContainsKey(roomName))
        {
            Debug.LogError($"Room {roomName} does not exist!");
            return;
        }

        NetworkRoomInfo room = rooms[roomName];

        if (!room.isOpen || room.currentPlayers >= room.maxPlayers)
        {
            Debug.LogError($"Room {roomName} is full or closed!");
            return;
        }

        room.currentPlayers++;
        rooms[roomName] = room;
        Debug.Log($"Successfully joined room: {roomName}");

        // Notify the specific client that they've successfully joined
        TargetOnRoomJoined(connectionToClient, roomName);
    }

    [TargetRpc]
    private void TargetOnRoomJoined(NetworkConnection target, string roomName)
    {
        Debug.Log($"Successfully joined room: {roomName}");
    }
}
