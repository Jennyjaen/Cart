using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

/*
 //  1: 충돌 2: 드리프트/ 가속 3. 아이템 4. 모터 진동/ 공중/ 속도 5. 바닥의 질감 10. 피드백 없음.
 1. 충돌
    1. 폭탄에 휘말림  2. 장애물, 3. 등껍질 4. 바나나 5. 도로의 덜컹거림(bump, 착륙 등)-> 생각해보니 handstick 생각하면 나누어줘야 할 것 같음. 
    TODO: 장애물 효과가 나타나는 도중 다른 장애물에 부딪히지 못하도록 설정할 것.
 4.
    1. 모터진동 2. 공중
5.
    1. 돌바닥_1 2. 돌바닥_2 3. 잔디
 */

public class HapticScript : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public byte[] larray = new byte[108];
    [HideInInspector]
    public byte[] rarray= new byte[108];

    private PlayerScript player;
    private EventScript status;

    private GamePadState state;
    private PlayerIndex play;
    void Start()
    {
        player = transform.GetComponent<PlayerScript>();
        status = transform.GetComponent<EventScript>();
        switch (player.inputMethod) {
            case PlayerScript.InputMethod.GamePad:
                state = GamePad.GetState(PlayerIndex.One);
                play = PlayerIndex.One;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (player.inputMethod) {
            case PlayerScript.InputMethod.GamePad:
                if(status.situation == 1) {
                    if(status.detail == 1) {
                        StartCoroutine(GP_Bomb());
                    }
                    else if(status.detail <4) {
                        StartCoroutine(GP_Collide());
                    }
                    else if(status.detail == 4) {
                        StartCoroutine(GP_Banana());
                    }
                }
                else if(status.situation == 2) {
                    if(status.detail == 1) {
                        float vib = Mathf.Clamp(player.CurrentSpeed / 60f, 0, 1);
                        GamePad.SetVibration(play, 0, 1 * vib);
                    }
                }
                else if(status.situation == 5) {
                    if(status.detail == 1) { }
                    if(status.detail == 2) { }
                    if(status.detail == 3) { }
                }
                if(status.situation == 10) {
                    status.detail = 10;
                    GamePad.SetVibration(play, 0, 0);
                }
                break;
        }
    }

    IEnumerator GP_Banana() {
        GamePad.SetVibration(play, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.25f);
        GamePad.SetVibration(play, 0.1f, 0.1f);
        yield return new WaitForSeconds(0.5f);
        GamePad.SetVibration(play, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.15f);

        status.situation = 10;
        status.detail = 10;
    }

    IEnumerator GP_Bomb() {
        GamePad.SetVibration(play, 1, 1);
        yield return new WaitForSeconds(1f);
        status.situation = 10;
        status.detail = 10;
    }

    IEnumerator GP_Collide() {
        GamePad.SetVibration(play, 1f, 1f);
        yield return new WaitForSeconds(1f);
        status.situation = 10;
        status.situation = 10;
    }
    IEnumerator HS_Banana() {
        float clock = 1f /56f;
        int x;
        int y;
        for(int i = 0; i< 56; i++) {
            if (i <= 5) {
                x = 12 + i; y = 0;
            }
            else if (i <= 16) {
                x = 17; y = i - 5;
            }
            else if (i <= 33) {
                x = 17 - (i - 16); y = 11;
            }
            else if (i <= 44) {
                x = 0; y = 11 - (i - 33);
            }
            else {
                x = i - 44; y = 0;
            }

            for(int p = 0; p< 18; p++) {
                for(int q = 0; q< 6; q++) {
                    larray[p * 6 + q] = (byte)(6 * InSquare(x, y, q* 2, p) + InSquare(x, y, q * 2 +1, p));
                    rarray[107 - (p * 6 + q)] = (byte)(InSquare(x, y, q * 2 + 12, p) + 6 * InSquare(x, y, q * 2 + 13, p));
                }
            }
            yield return new WaitForSeconds(clock);
        }
        
    }

    int InSquare(int x, int y, int p, int q) {
        if(p>= x && p <= 5 + x && q >= y && q<= y+5) {
            return 4;
        }

        return 0;
    }
}
