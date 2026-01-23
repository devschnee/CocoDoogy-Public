using UnityEngine;

/// <summary>
/// 신호를 전달하는 타워형 기믹 오브젝트.
/// ISignalSender를 구현하여 연결된 ISignalReceiver(Door 등)에 신호를 전달하는 역할을 담당
/// 실제 감지 로직이나 판정은 외부 시스템에 위임
/// </summary>

public class TowerBlock : Block, ISignalSender
{
    public ISignalReceiver Receiver { get; set; }

    public void ConnectReceiver(ISignalReceiver receiver)
    {
        Receiver = receiver;
    }

    public void SendSignal()
    {
        AudioEvents.Raise(SFXKey.InGameObject, 6, pooled: true, pos: transform.position);
        Receiver.ReceiveSignal();
    }
}
