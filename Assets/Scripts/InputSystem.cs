using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class InputSystem
    {
        Rewired.Player playerInput;

        public InputSystem()
        {
            playerInput = Rewired.ReInput.players.GetPlayer(0);
        }

        public float GetHMoveAxis()
        {
            return playerInput.GetAxis("MoveHorizontal");
        }
        public float GetVMoveAxis()
        {
            return playerInput.GetAxis("MoveVertical");
        }

        public bool GetDodgeInput() {
            if (playerInput.GetAxis("Dodge") > 0.75f) return true;
            else return false;
        }

        public bool GetNormalComboATK() {
            return playerInput.GetButtonDown("NormalComboATK");
        }
    }
}


