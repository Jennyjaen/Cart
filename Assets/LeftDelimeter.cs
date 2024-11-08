/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using UnityEngine;
using System.Collections;
using System.Text;

/**
 * Sample for reading using polling by yourself, and writing too.
 */
public class LeftDelimiter : MonoBehaviour {
    public SerialControllerCustomDelimiter serialController;

    private byte[] sendArray;
    //private FirstPersonMovement person;
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

    private int accum_x; //���ݱ��� �� ������ �Ÿ�
    private int accum_y;
    [HideInInspector]
    public int save_x => accum_x; //���ݱ��� �� ������ �Ÿ�
    [HideInInspector]
    public int save_y => accum_y;

    void Start() {
        serialController = GameObject.Find("LSerial").GetComponent<SerialControllerCustomDelimiter>();

        /*person = GetComponentInParent<FirstPersonMovement>();
        if (person == null) {
            Debug.Log("Can not find person");
        }*/

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
        string output = "";  // ����� ���ڿ��� ������ ����

        for (int i = 0; i < array.Length; i++) {
            output += (int)(array[i] / 6) + " " + (int)(array[i] % 6) + " ";  // �迭�� ���� ��� ���ڿ��� �߰�

            // groupSize ���� ����� ������ �ٹٲ�
            if ((i + 1) % 6 == 0) {
                output += "\n";  // groupSize���� �ٹٲ� �߰�
            }
        }

        // ���� ���
        Debug.Log(output);
    }

    // Executed each frame
    void Update() {
        if (serialController == null) {
            Debug.Log("find serial controller");
            return;
        }

        int[] message = serialController.ReadSerialMessage();

        if (message == null) {
            //Debug.Log("no message");
            x = 0;
            y = 0;
            return;
        }
        /*
        if (person != null) {
            sendArray = person.larray;
            //printArray(sendArray);
        }*/
        if(sendArray != null) {
            serialController.SendSerialMessage(sendArray);
        }


        if (message.Length == 2) {
            /*
            y = (int)message[0];
            x = (int)message[1];

            if (y > 127) { y = 127 - y; }
            if (x > 127) { x = 127 - x; }*/
            y = message[0];
            x = message[1];
            accum_x += x;
            accum_y += y;

            /*
            if (person.inputMethod == FirstPersonMovement.InputMethod.HandStickGesture) {
                if (x != 0 || y != 0) {
                    if (y != 0) {
                        if (sum_y * y < 0) {
                            sum_y = y;
                        }
                        else {
                            sum_y += y;
                        }
                        if (Mathf.Abs(sum_y) > 10) { zerostream_y = 0; }
                        else { zerostream_y++; }
                    }
                    else {
                        sum_y = 0;
                        zerostream_y++;
                    }
                    if (x != 0) {
                        if (sum_x * x < 0) {
                            sum_x = x;
                        }
                        else {
                            sum_x += x;
                        }
                        if (Mathf.Abs(sum_x) > 10) { zerostream_x = 0; }
                        else { zerostream_x++; }
                    }
                    else {
                        sum_x = 0;
                        zerostream_x++;
                    }
                }
                else {
                    sum_x = 0;
                    sum_y = 0;
                    zerostream_x++;
                    zerostream_y++;
                }

                zerostream = Mathf.Min(zerostream_x, zerostream_y);
            }
            else if (person.inputMethod == FirstPersonMovement.InputMethod.HandStickThrottle) {
                if (Mathf.Abs(accum_x) < 100 && Mathf.Abs(accum_y) < 100) {
                    zerostream++;
                }
                else {
                    zerostream = 0;
                }
            }

            */
        }
        else {
            Debug.Log("message length: " + message.Length);
            x = 0;
            y = 0;
        }

    }
}
