using UnityEngine;

public abstract class LobbyCharacterBaseState
{
    protected BaseLobbyCharacterBehaviour _baseChar;

    protected LobbyCharacterBaseState(BaseLobbyCharacterBehaviour baseChar)
    {
        _baseChar = baseChar;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
