using UnityEngine;

public class PlayerTransparentCollider : MonoBehaviour, IMoveStrategy
{

    public LayerMask transparentMask;
    public (Vector3, Vector3) Execute(Vector3 moveDir, Rigidbody rb, PlayerMovement player)
    {
        Vector3 finalDir = moveDir;

        //레이캐스트 방식
        Ray ray = new(rb.position + Vector3.up * .5f, moveDir);
        if (Physics.Raycast(ray, .5f, transparentMask))
        {
            finalDir = Vector3.ClampMagnitude(finalDir, .2f);
        }

        return (finalDir, Vector3.zero);
    }


    void Reset()
    {
        transparentMask = LayerMask.GetMask("TransparentWall");
    }

}