using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightDelimeter : MonoBehaviour
{
    public SerialControllerCustomDelimiter serialController;

    private byte[] sendArray;

    // Initialization
    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;

    [HideInInspector]
    public int stream;

    [HideInInspector]
    public int zerostream;
    [HideInInspector]
    public int zerostream_x;
    [HideInInspector]
    public int zerostream_y;

    [HideInInspector]
    public int sum_x;
    [HideInInspector]
    public int sum_y;

    private int accum_x; //지금까지 총 누적된 거리
    private int accum_y;
    [HideInInspector]
    public int save_x => accum_x; //지금까지 총 누적된 거리
    [HideInInspector]
    public int save_y => accum_y;

    private int XdirectionDistance = 0;
    private int YdirectionDistance = 0;

    private int prevX = 0; 
    private int prevY = 0;

    private RInputGesture currentGesture=RInputGesture.Neutral;

    private float shakeThreshold = 80f;
    private int shakeCountThreshold = 2;
    private int shakeCount=0;
    private float noShakeTimer = 0;
    private float noShakeTimerLimit = 0.5f;
    private bool driveStickMode = false;

    void Start() {
        serialController = GameObject.Find("RSerial").GetComponent<SerialControllerCustomDelimiter>();

        x = 0;
        y = 0;
        sum_x = 0;
        sum_y = 0;
        zerostream = 0;
        zerostream_x = 0;
        zerostream_y = 0;

        accum_x = 0;
        accum_y = 0;
    }


    void printArray(byte[] array) {
        string output = "";  // 출력할 문자열을 저장할 변수

        for (int i = 0; i < array.Length; i++) {
            output += (int)(array[i] / 6) + " " + (int)(array[i] % 6) + " ";  // 배열의 값을 출력 문자열에 추가

            // groupSize 개씩 출력할 때마다 줄바꿈
            if ((i + 1) % 6 == 0) {
                output += "\n";  // groupSize마다 줄바꿈 추가
            }
        }

        // 최종 출력
        Debug.Log(output);
    }

    // Executed each frame
    void Update() {
        bool hasShaken = false;
        if (serialController == null) {
            Debug.Log("find serial controller");
            return;
        }

        int[] message = serialController.ReadSerialMessage();

        if (message == null) {
            //Debug.Log("no message");
            x = 0;
            y = 0;
            noShakeTimer += Time.deltaTime;
            return;
        }

        /*
        if (person != null) {
            sendArray = person.larray;
            //printArray(sendArray);
        }*/

        if (sendArray != null) {
            serialController.SendSerialMessage(sendArray);
        }


        if (message.Length == 2) {
            /*
            y = (int)message[0];
            x = (int)message[1];

            if (y > 127) { y = 127 - y; }
            if (x > 127) { x = 127 - x; }*/
            //Debug.Log(x);
            //Debug.Log(x * message[1]);
            y = message[0];
            x = message[1];
            accum_x += x;
            accum_y += y;
            if (x * prevX < 0 ) {
                if ( Mathf.Abs(XdirectionDistance) < shakeThreshold) {
                    shakeCount += 1;
                    noShakeTimer = 0;
                    hasShaken = true;
                    //Debug.Log($"MiniShake {shakeCount} {XdirectionDistance}");
                }
                XdirectionDistance = 0;
                //Debug.Log("XChange!");
            }
            if (y * prevY < 0 ) {
                if (Mathf.Abs(YdirectionDistance) < shakeThreshold) {
                    shakeCount += 1;
                    noShakeTimer = 0;
                    hasShaken = true;
                    //Debug.Log($"MiniShake {shakeCount} {XdirectionDistance}");
                }
                YdirectionDistance = 0;
                //Debug.Log("YChange!");
            }
            prevX = x;
            prevY = y;

            //Debug.Log($"Right Serial - x: {x}, y: {y} ");
            //Debug.Log($"Right Serial - x: {accum_x}, y: {accum_y} ");
        }
        else {
            //Debug.Log("message length: " + message.Length);
            x = 0;
            y = 0;
        }
        if (!hasShaken) {
            noShakeTimer += Time.deltaTime;
            //Debug.Log("ZZ"+noShakeTimer);
        }
        if (noShakeTimer > noShakeTimerLimit) {
            shakeCount = 0;
            if (currentGesture == RInputGesture.Drift) {
                //Debug.Log("SHAKE RESET");
                currentGesture = RInputGesture.NoGesture;
            }
        }
        XdirectionDistance += x;
        YdirectionDistance += y;
        //Debug.Log($"X: {XdirectionTimer}s, {XdirectionDistance} units, Y:  {YdirectionTimer}s, {YdirectionDistance} units.");
        //Debug.Log("CHECK");
        checkGesture();
    }

    void checkGesture() {
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        if (XdirectionDistance > 500) {
            if (!driveStickMode) {
                Debug.Log("NEUTRAL!");
                driveStickMode = true;
            }
            currentGesture = RInputGesture.Neutral;
        }
        if (XdirectionDistance < -100 || (accum_x<50&& accum_x>-50)) {
            if (driveStickMode) {
                driveStickMode = false;
                currentGesture = RInputGesture.NoGesture;
            }
        }
        if (driveStickMode) {
            if (YdirectionDistance > 400) {
                Debug.Log("Drive");
                currentGesture = RInputGesture.Drive;
                XdirectionDistance = 0;
                YdirectionDistance = 0;
                driveStickMode = false;
            }
            if (YdirectionDistance < -400) {
                Debug.Log("Reverse");
                currentGesture = RInputGesture.Reverse;
                XdirectionDistance = 0;
                YdirectionDistance = 0;
                driveStickMode = false;
            }
        }


        if (YdirectionDistance > 600 ) {
            if (currentGesture != RInputGesture.Item) {
                Debug.Log("ITEM!");
                currentGesture = RInputGesture.Item;
                YdirectionDistance = 0;
            }
        }

        if ( shakeCount > shakeCountThreshold) {
            if (currentGesture != RInputGesture.Drift) {
                shakeCount = 0;
                Debug.Log("Drift!");
                currentGesture = RInputGesture.Drift;
            }
        }

        player.getRGesture(currentGesture);
        currentGesture = RInputGesture.NoGesture;
    }
}
