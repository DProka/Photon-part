
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public class NETChatController : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;
    private NETController netController;
    [SerializeField] string userName;
    [SerializeField] string globalChat;

    private string[] colors;
    private string playerColor;

    [Header("Links")]
    
    [SerializeField] UIChat uiChat;
    [SerializeField] TMP_InputField textMessage;

    private bool isConnected;

    public void Init()
    {
        chatClient = new ChatClient(this);
        netController = GetComponent<NETController>();
        isConnected = false;
        SetColors();
    }

    public void UpdateChat() { chatClient.Service(); }
    
    public void SetRegion(string region) { chatClient.ChatRegion = region; }
    
    public bool CheckChatConnection() { return isConnected; }

    public void DisconnectFromGlobal() { chatClient.Disconnect(); }
    
    public void SendMessage() { chatClient.PublishMessage(globalChat, $"<color={playerColor}>{userName}</color>: {textMessage.text}"); }
    
    public void SetPlayersName(string name) { userName = name; }
    
    public void DebugReturn(DebugLevel level, string message) { Debug.Log($"{level}, {message}"); }
    
    public void OnChatStateChange(ChatState state) { Debug.Log("ChatState: " + state); }
    
    private void SetColors()
    {
        colors = new string[11];

        colors[0] = "#0000ffff";
        colors[1] = "#a52a2aff";
        colors[2] = "#00ffffff";
        colors[3] = "#ff00ffff";
        colors[4] = "#008000ff";
        colors[5] = "#add8e6ff";
        colors[6] = "#ff0000ff";
        colors[7] = "#008080ff";
        colors[8] = "#a52a2aff";
        colors[9] = "#ffff00ff";
        colors[10] = "#800080ff";

        int rand = Random.Range(0, colors.Length);
        playerColor = colors[rand];
    }

    public void ConnectToGlobal()
    {
        if (!chatClient.CanChat)
        {
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userName));

            Debug.Log($"Connected To {globalChat}: {chatClient.CanChat}");
        }
    }

    public void OnConnected()
    {
        chatClient.Subscribe(globalChat, 0);
        //Debug.Log("You Connected To MasterChat");
    }

    public void OnDisconnected()
    {
        chatClient.Unsubscribe(new string[] { globalChat });
        isConnected = false;
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            uiChat.SetChatText($"\n {messages[i]}");
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) { throw new System.NotImplementedException(); }
    
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { throw new System.NotImplementedException(); }
    
    public void OnSubscribed(string[] channels, bool[] results)
    {
        isConnected = true;

        for (int i = 0; i < channels.Length; i++)
        {
            uiChat.SetChatText($"You Are Connected To: {channels[i]}");
        }

        netController.UpdateChatConnection();
    }

    public void OnUnsubscribed(string[] channels)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            Debug.Log($"You Are Disconnected from: {channels[i]}");
        }
    }

    public void OnUserSubscribed(string channel, string user) { Debug.Log($"{user} Subscribed To {channel}"); }
    
    public void OnUserUnsubscribed(string channel, string user) { Debug.Log($"{user} Unsubscribed From {channel}"); }
}
