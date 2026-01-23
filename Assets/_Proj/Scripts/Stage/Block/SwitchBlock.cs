using UnityEngine;

public class SwitchBlock : Block, ISignalSender
{
    public ISignalReceiver Receiver { get; set; }

    public void ConnectReceiver(ISignalReceiver receiver)
    {
        Receiver = receiver;
    }

    public void SendSignal()
    {
        AudioEvents.Raise(SFXKey.InGameObject, 2, pooled: true);
        Receiver.ReceiveSignal();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Block>(out Block block))
        {
            SendSignal();
         }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Block>(out Block block))
        {
            SendSignal();
        }

    }
}