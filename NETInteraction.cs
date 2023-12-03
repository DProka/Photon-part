using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NETInteraction : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private NETController netController;
    private int nextBallNumber = 0;
    private int bingoCount = 0;

    private int[] numbers;
    private int bingos;

    public void Init()
    {
        netController = GetComponent<NETController>();
    }

    public void SendListOfNumbersToClients(int[] numbers, int bingoCount)
    {
        object[] data = new object[76];

        for (int i = 0; i < numbers.Length; i++)
        {
            data[i] = numbers[i];
        }

        data[75] = bingoCount;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(1, data, raiseEventOptions, SendOptions.SendReliable);

        Debug.Log("New List Sended");
    }

    public void SubstractOneBingo()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object[] data = new object[] { 1 };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(2, data, raiseEventOptions, SendOptions.SendReliable);

            Debug.Log("Bingo Sended From Master");
        }
        else
        {
            object[] data = new object[] { 1 };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
            PhotonNetwork.RaiseEvent(9, data, raiseEventOptions, SendOptions.SendReliable);

            Debug.Log("Bingo Sended To Master");
        }
    }

    public void SendBingoDataToAllClients(int bingoCount)
    {
        object[] data = new object[] { bingoCount };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(2, data, raiseEventOptions, SendOptions.SendReliable);

        Debug.Log("Bingo Info Sended");
    }

    public void UpdateBingoDataOnMasterClient()
    {
        object[] data = new object[] { 1 };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(9, data, raiseEventOptions, SendOptions.SendReliable);
        
        Debug.Log("Array sended");
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            numbers = new int[75];

            object[] data = (object[])photonEvent.CustomData;

            for (int i = 0; i < 75; i++)
            {
                numbers[i] = (int)data[i];
            }

            bingos = (int)data[75];

            if(!PhotonNetwork.IsMasterClient)
                netController.GetListOfNumbersFromMaster(numbers, bingos);
            
            Debug.Log("Received List from master-client");
        }

        if (photonEvent.Code == 2)
        {
            object[] data = (object[])photonEvent.CustomData;
            int receivedData = (int)data[0];
            bingoCount = receivedData;

            netController.UpdateBingoCount(bingoCount);
            Debug.Log("Received BingoCount From MasterClient: " + nextBallNumber);
        }
    }
}
