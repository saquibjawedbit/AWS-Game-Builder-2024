using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;
using Mirror;
using Photon.Pun;

public class MirrorPhotonVoiceIntegration : NetworkBehaviour
{
    [SerializeField] private VoiceConnection voiceConnection;
    [SerializeField] private bool autoConnect = true;

    private bool hasInitialized = false;
    private string currentRoomName;
    private const string DEFAULT_ROOM_PREFIX = "VoiceRoom_";

    // SyncVar to ensure all clients join the same voice room
    [SyncVar(hook = nameof(OnVoiceRoomNameChanged))]
    private string syncedRoomName;

    private void Awake()
    {
        if (voiceConnection == null)
        {
            voiceConnection = FindObjectOfType<VoiceConnection>();
            if (voiceConnection == null)
            {
                Debug.LogError("No VoiceConnection found in the scene. Please add one or assign it in the inspector.");
            }
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Generate a unique room name on the server
        syncedRoomName = DEFAULT_ROOM_PREFIX + System.Guid.NewGuid().ToString("N").Substring(0, 8);
        Debug.Log($"Server created voice room name: {syncedRoomName}");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Mirror client started. Preparing to connect to Photon Voice...");

        if (!hasInitialized && autoConnect)
        {
            InitializeVoiceConnection();
        }
    }

    private void OnVoiceRoomNameChanged(string oldRoomName, string newRoomName)
    {
        currentRoomName = newRoomName;
        if (hasInitialized && voiceConnection.Client.IsConnectedAndReady)
        {
            JoinVoiceRoom();
        }
    }

    private void InitializeVoiceConnection()
    {
        if (voiceConnection != null && !voiceConnection.Client.IsConnected)
        {
            hasInitialized = true;
            voiceConnection.ConnectUsingSettings();
            voiceConnection.Client.StateChanged += VoiceClientStateChanged;
        }
    }

    private void VoiceClientStateChanged(ClientState fromState, ClientState toState)
    {
        Debug.Log($"Voice client state changed from {fromState} to {toState}");

        if (toState == ClientState.ConnectedToMasterServer && !string.IsNullOrEmpty(currentRoomName))
        {
            JoinVoiceRoom();
        }
    }

    private void JoinVoiceRoom()
    {
        if (string.IsNullOrEmpty(currentRoomName))
        {
            Debug.LogWarning("Cannot join voice room: room name not yet synchronized");
            return;
        }

        RoomOptions roomOptions = new RoomOptions
        {
            IsVisible = false,
            MaxPlayers = 0,
            PublishUserId = true
        };

        Debug.Log($"Attempting to join voice room: {currentRoomName}");
        voiceConnection.Client.OpJoinOrCreateRoom(new EnterRoomParams
        {
            RoomName = currentRoomName,
            RoomOptions = roomOptions
        });
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        DisconnectVoice();
    }

    private void OnDisable()
    {
        DisconnectVoice();
    }

    private void DisconnectVoice()
    {
        if (voiceConnection != null && voiceConnection.Client != null)
        {
            if (voiceConnection.Client.IsConnected)
            {
                Debug.Log("Disconnecting from Photon Voice...");
                voiceConnection.Client.StateChanged -= VoiceClientStateChanged;
                voiceConnection.Client.Disconnect();
            }
            hasInitialized = false;
            currentRoomName = null;
        }
    }

    public void ToggleVoiceConnection(bool connect)
    {
        if (connect)
        {
            InitializeVoiceConnection();
        }
        else
        {
            DisconnectVoice();
        }
    }
}