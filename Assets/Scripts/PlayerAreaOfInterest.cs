using Fusion;
using System.Diagnostics;
using UnityEngine;

public class PlayerAreaOfInterest : NetworkBehaviour
{
    private float range = 2f;

    public override void FixedUpdateNetwork()
    {
        //if (Runner.IsServer)
        {
            var controller = Object.InputAuthority;
            if (controller.IsValid)
            {
                Runner.AddPlayerAreaOfInterest(controller, transform.position, range);
            }
        }
    }
}
