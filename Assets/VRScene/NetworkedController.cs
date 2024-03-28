using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkedController : NetworkBehaviour
{
    public NetworkedVRCharacterController CharacterController; 
    private NetworkCharacterController _cc;
    
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
       
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            CharacterController.move = data.direction;
            //transform.position += data.direction;
           // _cc.Move(5 * data.direction * Runner.DeltaTime);
            
        }
    }

}