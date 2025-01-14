using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private TextMeshProUGUI connectionStatusText;

    [Header("Network Settings")]
    [SerializeField] private string serverAddress = "localhost";
    [SerializeField] private bool autoConnect = true;

    private void Start()
    {
        if (autoConnect)
        {
            ConnectToServer();
        }

        // Setup connection status updates
        NetworkClient.OnConnectedEvent += OnConnected;
        NetworkClient.OnDisconnectedEvent += OnDisconnected;

        UpdateConnectionStatus();
    }

    private void OnDestroy()
    {
        // Cleanup event listeners
        NetworkClient.OnConnectedEvent -= OnConnected;
        NetworkClient.OnDisconnectedEvent -= OnDisconnected;
    }

    public void ConnectToServer()
    {
        if (!NetworkClient.isConnected)
        {
            Debug.Log($"Connecting to server at {serverAddress}...");
            NetworkClient.Connect(serverAddress);
            UpdateConnectionStatus();
        }
    }

    public void CreateRoom()
    {
        string roomName = roomNameInput != null ? roomNameInput.text : "Room" + Random.Range(0, 1000);

        if (RoomManagerBehavior.Instance != null)
        {
            RoomManagerBehavior.Instance.RequestCreateRoom(roomName);
            Debug.Log($"Requesting to create room: {roomName}");
        }
        else
        {
            Debug.LogError("RoomManagerBehavior instance not found!");
            ShowError("Room manager not found!");
        }
    }

    public void JoinRoom()
    {
        string roomName = roomNameInput != null ? roomNameInput.text : "";

        if (string.IsNullOrEmpty(roomName))
        {
            ShowError("Please enter a room name!");
            return;
        }

        if (RoomManagerBehavior.Instance != null)
        {
            RoomManagerBehavior.Instance.RequestJoinRoom(roomName);
            Debug.Log($"Requesting to join room: {roomName}");
        }
        else
        {
            Debug.LogError("RoomManagerBehaviour instance not found!");
            ShowError("Room manager not found!");
        }
    }

    private void OnConnected()
    {
        Debug.Log("Connected to server successfully!");
        UpdateConnectionStatus();
        EnableRoomControls(true);
    }

    private void OnDisconnected()
    {
        Debug.Log("Disconnected from server!");
        UpdateConnectionStatus();
        EnableRoomControls(false);
    }

    private void UpdateConnectionStatus()
    {
        if (connectionStatusText != null)
        {
            connectionStatusText.text = NetworkClient.isConnected ?
                "Connected to server" :
                "Not connected";

            connectionStatusText.color = NetworkClient.isConnected ?
                Color.green :
                Color.red;
        }
    }

    private void EnableRoomControls(bool enabled)
    {
        if (createRoomButton != null) createRoomButton.interactable = enabled;
        if (joinRoomButton != null) joinRoomButton.interactable = enabled;
        if (roomNameInput != null) roomNameInput.interactable = enabled;
    }

    private void ShowError(string message)
    {
        Debug.LogError(message);
        // You can implement UI feedback here, like a popup or error text
    }
}
