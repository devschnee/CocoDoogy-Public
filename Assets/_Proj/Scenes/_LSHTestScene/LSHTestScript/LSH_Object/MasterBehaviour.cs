using System.Collections;
using UnityEngine;

public class MasterBehaviour : BaseLobbyCharacterBehaviour
{
    //private int currentDecoIndex;
    private Transform targetDeco;

    protected override void Awake()
    {
        base.Awake();
        agent.avoidancePriority = 98;
        //currentDecoIndex = 0;
        targetDeco = null;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private IEnumerator Move()
    {
        isMoving = true;
        while (isMoving)
        {
            if (targetDeco != null)
            {

            }
            else
            {
                charAgent.MoveToRandomTransPoint(transform);
            }

            yield return waitU;
        }
    }


    // 인터페이스 영역
    public override void OnCocoAnimalEmotion()
    {
        
    }

    public override void OnCocoMasterEmotion()
    {
        
    }

    public override void OnLobbyEndDrag(Vector3 position)
    {
        base.OnLobbyEndDrag(position);
        StartCoroutine(Move());
    }
    public override void OnLobbyInteract()
    {
        base.OnLobbyInteract();
        charAnim.ClickMaster();
    }
    public override void InNormal()
    {
        base.InNormal();
        StartCoroutine(Move());
    }
    public override void StartScene()
    {
        throw new System.NotImplementedException();
    }
    protected override void HandleIdle()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleMove()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleStuck()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleInteraction()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleAnimation()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleCocoOther()
    {
        throw new System.NotImplementedException();
    }

    protected override void ChangeState(LobbyCharacterState newState)
    {
        throw new System.NotImplementedException();
    }

    // public override void ExitScene()
    // {

    // }
}
