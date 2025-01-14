using UnityEngine;
using Mirror;
using Photon.Voice.Unity;
using Photon.Realtime;

// Main voice chat controller
public class PlayerVoiceController : NetworkBehaviour
{
    [SerializeField] private UnityVoiceClient voiceConnection;
    [SerializeField] private Recorder recorder;
    [SerializeField] private Speaker speakerPrefab;
    [SerializeField] private string roomName = "DefaultVoiceRoom";
    [SerializeField] private bool autoConnect = true;
    [SerializeField] private bool debugEchoMode = false;

    private bool isConnecting = false;

    private void Awake()
    {
        InitializeComponents();
    }

    private void OnConnectedToMaster()
    {
        print("Hello World");
        if (autoConnect)
        {
            ConnectToVoice();
        }
    }

    private void InitializeComponents()
    {
        // Setup UnityVoiceClient
        if (voiceConnection == null)
        {
            voiceConnection = GetComponent<UnityVoiceClient>();
            if (voiceConnection == null)
            {
                voiceConnection = gameObject.AddComponent<UnityVoiceClient>();
            }
        }

        // Setup Recorder
        if (recorder == null)
        {
            recorder = FindObjectOfType<Recorder>();
            if (recorder == null)
            {
                GameObject recorderObj = new GameObject("VoiceRecorder");
                recorderObj.transform.SetParent(transform);
                recorder = recorderObj.AddComponent<Recorder>();
            }
        }

        // Configure components
        voiceConnection.PrimaryRecorder = recorder;
        voiceConnection.SpeakerPrefab = speakerPrefab.gameObject;
        recorder.DebugEchoMode = debugEchoMode;
    }

    public void ConnectToVoice()
    {
        if (isConnecting) return;

        if (!voiceConnection.Client.IsConnected)
        {
            Debug.Log("Connecting to Photon Voice...");
            isConnecting = true;
            voiceConnection.Client.StateChanged += OnVoiceClientStateChanged;
            voiceConnection.ConnectUsingSettings(); // Ensure Photon settings are configured correctly
        }
        else if (voiceConnection.Client.IsConnectedAndReady)
        {
            Debug.Log("Already connected to Photon Voice. Joining room...");
            JoinRoom();
        }
    }


    private void OnVoiceClientStateChanged(ClientState fromState, ClientState toState)
    {
        Debug.Log($"Voice client state changed from {fromState} to {toState}");

        switch (toState)
        {
            case ClientState.ConnectedToMasterServer:
                Debug.Log("Connected to Master Server. Now joining the lobby...");
                voiceConnection.Client.OpJoinLobby(TypedLobby.Default); // Explicitly join the lobby
                break;

            case ClientState.JoinedLobby:
                Debug.Log("Joined the lobby. Ready to join or create a room...");
                JoinRoom();
                break;

            case ClientState.Disconnected:
                isConnecting = false;
                Debug.Log("Disconnected from Photon Voice.");
                break;

            default:
                break;
        }
    }


    private void JoinRoom()
    {
        if (!voiceConnection.Client.IsConnectedAndReady)
        {
            Debug.LogWarning("Cannot join room - client not ready for operations.");
            return;
        }

        Debug.Log($"Attempting to join voice room: {roomName}");
        voiceConnection.Client.OpJoinOrCreateRoom(new EnterRoomParams
        {
            RoomName = roomName,
            RoomOptions = new RoomOptions
            {
                IsVisible = false,
                MaxPlayers = 0,
                PublishUserId = true
            }
        });
    }

        // Public control methods
        public void SetMute(bool isMuted)
        {
            if (recorder != null)
            {
                recorder.TransmitEnabled = !isMuted;
                Debug.Log($"Microphone {(isMuted ? "muted" : "unmuted")}");
            }
        }

    public void SetRoomName(string newRoomName)
    {
        roomName = newRoomName;
        if (voiceConnection.Client.IsConnectedAndReady)
        {
            JoinRoom();
        }
    }

    public void Disconnect()
    {
        if (voiceConnection.Client.IsConnected)
        {
            voiceConnection.Client.StateChanged -= OnVoiceClientStateChanged;
            voiceConnection.Client.Disconnect();
            isConnecting = false;
            Debug.Log("Disconnected from Photon Voice");
        }
    }

    private void OnDestroy()
    {
        if (voiceConnection != null && voiceConnection.Client != null)
        {
            voiceConnection.Client.StateChanged -= OnVoiceClientStateChanged;
        }
    }

    private void OnDisable()
    {
        Disconnect();
    }
}