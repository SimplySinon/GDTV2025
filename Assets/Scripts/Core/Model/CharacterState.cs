using UnityEngine;

[System.Serializable]
public class CharacterState
{
    public State State;
    public CharacterState(State state)
    {
        State = state;
    }
}