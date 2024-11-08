using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;


public class HapticScript : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public byte[] larray;
    [HideInInspector]
    public byte[] rarray;

    private PlayerScript player;
    private EventScript status;
    void Start()
    {
        player = transform.GetComponent<PlayerScript>();
        status = transform.GetComponent<EventScript>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (player.inputMethod) {
            case PlayerScript.InputMethod.GamePad:
                break;
        }
    }
}
