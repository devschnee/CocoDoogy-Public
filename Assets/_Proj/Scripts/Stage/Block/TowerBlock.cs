using UnityEngine;

public class TowerBlock : Block, ISignalSender
{
    public ISignalReceiver Receiver { get; set; }

    public void ConnectReceiver(ISignalReceiver receiver)
    {
        Receiver = receiver;
    }

    public void SendSignal()
    {
        Receiver.ReceiveSignal();
    }
}
