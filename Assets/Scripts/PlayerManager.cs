using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerManager : MonoBehaviour
{
    public PlayerInput player1;
    public PlayerInput player2;

    public static float nextSwapAllowed;

    public static bool isSwapping = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //player1.user.UnpairDevices();
        //player2.user.UnpairDevices();

        InputUser.PerformPairingWithDevice(Keyboard.current, user: player1.user);
        InputUser.PerformPairingWithDevice(Keyboard.current, user: player2.user);

        player1.user.ActivateControlScheme("Keyboard_P1");
        player2.user.ActivateControlScheme("Keyboard_P2");
    }
}
