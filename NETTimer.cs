using Photon.Pun;
using UnityEngine;
using TMPro;

public class NETTimer : MonoBehaviourPunCallbacks, IPunObservable
{
    private float timeToStart;
    private float timer;

    private bool timerActive;

    public void Init()
    {
        timerActive = false;
    }

    public void UpdateTimer()
    {
        if (timer > 0 && timerActive)
            timer -= Time.fixedDeltaTime;
        else if (timer <= 0)
            timerActive = false;
    }

    public void ResetTimer()
    {
        timer = timeToStart;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (timerActive)
        {
            if (stream.IsWriting && PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(timer);
            }
            else if (stream.IsReading)
            {
                timer = (float)stream.ReceiveNext();
            }
        }
    }

    public void SetTimer(float time) { timeToStart = time; }

    public void StartTimer()
    {
        ResetTimer();
        timerActive = true;
    }

    public float GetTimer()
    {
        return timer;
    }
}
