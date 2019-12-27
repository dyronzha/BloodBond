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

        public bool GetMove() {
            float x = playerInput.GetAxis("MoveHorizontal");
            float z = playerInput.GetAxis("MoveVertical");
            if ((x * x + z * z) > 0.1f) return true;
            else return false;
        }

        public bool GetDodgeInput() {
            if (playerInput.GetButtonDown("Dodge")) return true;
            else return false;
        }
        public bool GetDashInput()
        {
            if (playerInput.GetAxis("Dash") > 0.3f) return true;
            else return false;
        }
        public bool GetNormalComboATK() {
            return playerInput.GetButtonDown("NormalComboATK");
        }
    }
}


