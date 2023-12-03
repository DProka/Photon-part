using UnityEngine;

public class NETController : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] PlayerProfile playerProfile;
    [SerializeField] FriendsList friendData;

    private bool isConnectedToMaster;
    private bool isConnectedToChat;
    private bool isConnectedToRoom;
    private bool isRoomMasterServer;

    [Header("Server Part")]

    [SerializeField] string region;

    private ConnectToServer connectToServer;
    
    [Header("Room part")]

    private NETRoom roomScript;
    private NETTimer startGameTimer;
    [SerializeField] private int maxPlayers;
    [SerializeField] private float startTimer;
    private int playersInRoom;

    private bool isGameRoom;

    [Header("NET Interaction")]

    private NETInteraction netInteraction;

    [HideInInspector] public bool isConnectedToGame;

    private NETChatController chatController;

    private NETFriendsController friendsController;

    public void Init()
    {
        ResetAllTriggers();

        connectToServer = GetComponent<ConnectToServer>();
        connectToServer.Init(region, playerProfile.playerID, playerProfile.playerNickName);
        ConnectToMasterServer();

        roomScript = GetComponent<NETRoom>();
        roomScript.Init(playerProfile.playerID);
        roomScript.SetPlayerNickName(playerProfile.playerNickName);
        roomScript.SetPlayersInRoom(maxPlayers);

        startGameTimer = GetComponent<NETTimer>();
        startGameTimer.Init();
        startGameTimer.SetTimer(startTimer);

        netInteraction = GetComponent<NETInteraction>();
        netInteraction.Init();

        chatController = GetComponent<NETChatController>();
        chatController.Init();
        chatController.SetRegion(region);
        chatController.SetPlayersName(playerProfile.playerNickName);

        friendsController = GetComponent<NETFriendsController>();
        friendsController.Init(playerProfile.playerID, playerProfile.playerNickName, playerProfile.avatarNumber);
    }

    public void UpdateNet()
    {
        chatController.UpdateChat();
    }

    public void UpdateTimer()
    {
        startGameTimer.UpdateTimer();
    }

    private void ResetAllTriggers()
    {
        isConnectedToMaster = false;
        isConnectedToChat = false;
        isConnectedToRoom = false;
        isRoomMasterServer = false;
    }

    #region Server Connection

    public void UpdateMasterServerConnection()
    {
        isConnectedToMaster = connectToServer.CheckMasterServerConnection();
        friendsController.CheckFriendStatus(friendData.friendID);
        Debug.Log($"Connection To Master: {isConnectedToMaster}");
        ConnectToChat();
    }

    public void ConnectToMasterServer() { connectToServer.ConnectToMasterServer(); }

    public bool GetNetConnectStatus()
    {
        if (isConnectedToMaster && chatController.CheckChatConnection())// && roomScript.GetConnectionStatus())
            return isConnectedToMaster;
        else
            return false;
    }
    #endregion

    #region Room Connection

    public void EnterRoom(bool isGameRoom)
    {
        if (chatController.CheckChatConnection())
        {
            roomScript.EnterRoom(isGameRoom);

            if(isGameRoom)
                UpdatePlayersInRoom();
        }
    }

    public void OnRoomEntered()
    {
        //gameController.OnGameRoomEntered();
    }

    public void UpdateRoomConnection()
    {
        isConnectedToRoom = roomScript.GetConnectionStatus();
    }

    public void CheckRoomMasterClient() { isRoomMasterServer = roomScript.CheckMasterClient(); }
    
    public bool GetRoomMasterStatus()
    {
        CheckRoomMasterClient();
        return isRoomMasterServer;
    }

    public void SetRoomConnectionStatus(bool isConnected, bool isMaster, string roomName)
    {
        //gameController.SetRoomConnection(isConnected, isMaster, roomName);
    }

    public void UpdatePlayersInRoom() { } //gameController.SetPlayersInRoom(roomScript.GetPlayersInRoom()); }

    public void ManagePlayersInRoom(bool add) { } //gameController.ManagePlayers(add); }
    
    public void LeaveRoom() { roomScript.LeaveRoom(); }
    
    public void CloseRoom() { roomScript.CloseRoom(); }
    #endregion

    #region Chat

    public void ConnectToChat() { chatController.ConnectToGlobal(); }

    public void UpdateChatConnection() 
    { 
        isConnectedToChat = chatController.CheckChatConnection();
    }

    #endregion

    #region Timer

    public float GetTimer()
    {
        startTimer = startGameTimer.GetTimer();
        return startTimer;
    }

    public void ResetTimer() { startGameTimer.ResetTimer(); }

    public void StartTimer() { startGameTimer.StartTimer(); }
    #endregion

    #region Net Interaction

    public void SendListOfNumbersToClients(int[] list, int bingoCount) { netInteraction.SendListOfNumbersToClients(list, bingoCount); }

    public void GetListOfNumbersFromMaster(int[] list, int bingocount) { }// gameController.GetNumbersListFromMaster(list, bingocount); }
    
    public void SubstractBingoFromClients(int bingoCount) { netInteraction.SendBingoDataToAllClients(bingoCount); }

    public void SubstractBingoFromMaster() { netInteraction.UpdateBingoDataOnMasterClient(); }

    public void UpdateBingoCount(int count)
    {
        //gameController.UpdateBingoCount(count);
        Debug.Log("Bingos in game " + count);
    }
    #endregion
}
