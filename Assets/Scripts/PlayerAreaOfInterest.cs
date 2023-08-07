using Fusion;
using  UnityEngine;

public class PlayerAreaOfInterest : NetworkBehaviour, ISimulationEnter, ISimulationExit
{
    private float radius = 2f;

    public override void FixedUpdateNetwork()
    {
        var target = Object.InputAuthority;

        if (target.IsValid)
        {
            Runner.AddPlayerAreaOfInterest(target, transform.position, radius);
        }
    }

    public void SimulationEnter()
    {
        Debug.Log("SimulationEnter");
    }

    public void SimulationExit()
    {
        Debug.Log("SimulationExit");
    }
}
