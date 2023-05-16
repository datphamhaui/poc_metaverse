using Fusion;
using UnityEngine;

public enum EInputButtons
{
    Jump = 0,
    Run = 1
}
public struct NetworkInputData : INetworkInput
{
    
    public const byte MOUSEBUTTON1 = 0x01;
    public const byte MOUSEBUTTON2 = 0x02;

    public NetworkButtons Buttons; 
    public Vector3 direction;
    public int speed;
    public bool Jump { get { return Buttons.IsSet(EInputButtons.Jump); } set { Buttons.Set((int)EInputButtons.Jump, value); } }

}

