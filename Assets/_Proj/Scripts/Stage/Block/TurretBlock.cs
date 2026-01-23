using UnityEngine;

/// <summary>
/// 터렛 감지 결과를 신호로 전달하는 기믹 오브젝트.
/// ISignalSender를 구현하여 감지 상태 변화 시 연결된 ISignalReceiver(Door 등)에 신호를 전달.
/// 실제 감지 로직은 Turret 시스템에 위임.
/// </summary>

public class TurretBlock : Block, ISignalSender
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