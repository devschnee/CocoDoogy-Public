using UnityEngine;
public interface IRider
{
    void OnStartRiding();
    void OnStopRiding();
    public GameObject gameObject { get; }
}
