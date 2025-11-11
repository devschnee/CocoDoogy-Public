public interface IRider
{
    public UnityEngine.Transform transform { get; }
    void OnStartRiding();
    void OnStopRiding();
}
