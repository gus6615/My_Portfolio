using System.Collections.Generic;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public class LobbyState : StateBase
    {
        public override void OnEnter()
        {
            Debug.Log("LobbyState OnEnter");
        }

        public override void OnUpdate(float dt)
        {

        }
        
        public override void OnExit()
        {
            Debug.Log("LobbyState OnExit");
        }
    }
}
