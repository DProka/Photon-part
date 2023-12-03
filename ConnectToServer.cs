
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    private NETController netController;
    private string region;
    private string userName;
    private AuthenticationValues authentication;

    public void Init(string _region, string _userID, string _userName)
    {
        netController = GetComponent<NETController>();

        authentication = new AuthenticationValues(_userID);
        region = _region;
        userName = _userName;
    }

    public void ConnectToMasterServer()
    {
        PhotonNetwork.AuthValues = authentication;
        PhotonNetwork.NickName = userName;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion(region);
    }

    public override void OnConnectedToMaster() 
    {
        netController.UpdateMasterServerConnection();
        Debug.Log("You are connected to master");
    }
    
    public override void OnDisconnected(DisconnectCause cause) { ConnectToMasterServer(); }

    public bool CheckMasterServerConnection() { return PhotonNetwork.IsConnectedAndReady; }

    public void DisconnectFromMaster()
    {
        if (CheckMasterServerConnection())
        {
            PhotonNetwork.Disconnect();
        }
    }
}
