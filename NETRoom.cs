using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NETRoom : MonoBehaviourPunCallbacks
{
    private NETController netController;
    private string playerID;

    private string roomName;
    private int playersInRoom;
    private int maxPlayersInRoom;

    private bool isConnected;
    private bool isGameRoom;

    public void Init(string _playerID)
    {
        netController = GetComponent<NETController>();
        playerID = _playerID;

        isConnected = false;
    }

    #region Room Interaction

    public void EnterRoom(bool _isGameRoom)
    {
        isGameRoom = _isGameRoom;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (_isGameRoom)
            {
                PhotonNetwork.JoinRandomRoom();
                Debug.Log("EnterGameRoom works");
            }
            else
            {
                PhotonNetwork.JoinRoom("General");
            }
        }
    }

    private void CreateGeneralRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.EmptyRoomTtl = 0;
        roomOptions.CleanupCacheOnLeave = true;
        roomOptions.PublishUserId = true;
        PhotonNetwork.CreateRoom("General", roomOptions);
    }

    private void CreateRandomRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayersInRoom;
        PhotonNetwork.CreateRoom(playerID, roomOptions, TypedLobby.Default);
    }

    public void CreateDelicatedRoom(string[] users)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.EmptyRoomTtl = 0;
        roomOptions.CleanupCacheOnLeave = true;
        roomOptions.PublishUserId = true;
        
        EnterRoomParams roomParams = new EnterRoomParams();
        roomParams.ExpectedUsers = users;   

        PhotonNetwork.CreateRoom("General", roomOptions);
    }

    public void EnterDelicatedRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        isConnected = true;
        roomName = PhotonNetwork.CurrentRoom.Name;

        if (isGameRoom)
        {
            playersInRoom = PhotonNetwork.PlayerList.Length;
            netController.UpdatePlayersInRoom();
            netController.CheckRoomMasterClient();
            netController.SetRoomConnectionStatus(CheckRoomConnection(), CheckMasterClient(), roomName);
        }
        else
        {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                Debug.Log($"Player ID: {player.UserId}, Nickname: {player.NickName}");
            }
        }

        netController.UpdateRoomConnection();
        netController.OnRoomEntered();
        Debug.Log($"You Joined {PhotonNetwork.CurrentRoom.Name} Room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        CreateGeneralRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRandomRoom();
    }

    public void LeaveRoom()
    {
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        isConnected = false;
        Debug.Log($"Yoy left room. Room {CheckRoomConnection()} Server {PhotonNetwork.IsConnectedAndReady}");
    }
    #endregion

    #region Settings

    public bool GetConnectionStatus() { return isConnected; }

    public void SetPlayerNickName(string nickName) { PhotonNetwork.NickName = nickName; }

    public void SetPlayersInRoom(int max) { maxPlayersInRoom = max; }

    public void CloseRoom() { PhotonNetwork.CurrentRoom.IsOpen = false; }

    public int GetPlayersInRoom() { return playersInRoom; }

    public bool CheckRoomConnection() { return PhotonNetwork.InRoom; }

    public bool CheckMasterClient() { return PhotonNetwork.IsMasterClient; }

    #endregion

    #region Players Interaction

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (isGameRoom)
        {
            playersInRoom = PhotonNetwork.PlayerList.Length;
            netController.ManagePlayersInRoom(true);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (isGameRoom)
        {
            playersInRoom = PhotonNetwork.PlayerList.Length;
            netController.ManagePlayersInRoom(false);
        }    
    }
    #endregion
}
