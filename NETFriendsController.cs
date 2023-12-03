
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class NETFriendsController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] UIFriendsCenter friendsCenter;

    private string playerID;
    private string playerName;
    private int playerAvaNumber;

    public void Init(string id, string name, int avaNum)
    {
        playerID = id;
        playerName = name;
        playerAvaNumber = avaNum;
    }

    #region Friends Status

    public void CheckFriendStatus(string[] friendsArray)
    {
        if(friendsArray.Length > 0)
            PhotonNetwork.FindFriends(friendsArray);
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        bool[] onlineStatus = new bool[friendList.Count];

        for (int i = 0; i < friendList.Count; i++)
        {
            onlineStatus[i] = friendList[i].IsOnline;
        }

        UpdateFriendsStatus(onlineStatus);
    }

    private void UpdateFriendsStatus(bool[] friendStatus)
    {
        friendsCenter.UpdateListStatus(friendStatus);
    }

    #endregion

    #region Friends Interaction

    public void FindPlayerByID(string userID)
    {
        Player targetPlayer = GetPlayerByUserID(userID);
        string status = "";

        if (targetPlayer != null)
        {
            SendListRequest(targetPlayer);
            status = "Request sended";
            Debug.Log("Request sended");
        }
        else
        {
            status = "ID is not correct, or player is offline";
            Debug.Log("ID is not correct, or player is offline");
        }

        SetStatus(status);
    }

    public void SetStatus(string status)
    {
        friendsCenter.SetStatus(status);
    }

    private void SendListRequest(Player targetPlayer)
    {
        int[] targetRoomnumber = new int[] { targetPlayer.ActorNumber };        

        object[] data = new object[3];

        data[0] = playerID;
        data[1] = playerName;
        data[2] = playerAvaNumber;
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        raiseEventOptions.TargetActors = targetRoomnumber;

        PhotonNetwork.RaiseEvent(11, data, raiseEventOptions, SendOptions.SendReliable);
        
        Debug.Log("Friand request sended");
    }

    public void AcceptListRequest(string userID)
    {
        int[] targetRoomnumber = new int[] { GetPlayerByUserID(userID).ActorNumber };

        object[] data = new object[3];

        data[0] = playerID;
        data[1] = playerName;
        data[2] = playerAvaNumber;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        raiseEventOptions.TargetActors = targetRoomnumber;

        PhotonNetwork.RaiseEvent(12, data, raiseEventOptions, SendOptions.SendReliable);

        Debug.Log("Friand request sended");
    }

    public void SendRoomInvite(string userID)
    {
        int[] targetRoomnumber = new int[] { GetPlayerByUserID(userID).ActorNumber };
        object[] data = new object[1];

        data[0] = playerID;
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        raiseEventOptions.TargetActors = targetRoomnumber;

        PhotonNetwork.RaiseEvent(19, data, raiseEventOptions, SendOptions.SendReliable);

        Debug.Log("Friand request sended");
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 11)
        {
            GetListInvite((object[])photonEvent.CustomData);

            Debug.Log("Friend request received");
        }

        if (photonEvent.Code == 12)
        {
            GetNewFriend((object[])photonEvent.CustomData);

            Debug.Log("Friend accepted request");
        }
        
        if (photonEvent.Code == 19)
        {
            GetRoomInvite((object[])photonEvent.CustomData);

            Debug.Log("Room Invite Geted");
        }
    }

    private void GetNewFriend(object[] data)
    {
        string id = (string)data[0];
        string name = (string)data[1];
        int avaNum = (int)data[2];

        friendsCenter.AddFriendToList(id, name, avaNum);
    }

    private void GetListInvite(object[] data)
    {
        string id = (string)data[0];
        string name = (string)data[1];
        int avaNum = (int)data[2];

        friendsCenter.GetNewRequest(id, name, avaNum);
    }

    private void GetRoomInvite(object[] data)
    {
        string id = (string)data[0];

        //friendsCenter.GetNewRequest(id, name, avaNum);
    }

    private Player GetPlayerByUserID(string userID)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"Player ID: {player.UserId}. Player Nick: {player.NickName}");
            
            if (player.UserId == userID)
            {
                return player;
            }
        }
        return null;
    }
    #endregion
}
