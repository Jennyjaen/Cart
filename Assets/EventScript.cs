using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScript : MonoBehaviour
{
    //���� ��Ȳ�� detect�ؼ� situation�� detail�� update
    //Hatpic Feedback.cs���� detail�� situation�� �������� array update
    [HideInInspector]
    public int situation;
    //  1: �浹 2: �帮��Ʈ/ ���� 3. ������ 4. ���� ����/ ����/ �ӵ� 5. �ٴ��� ���� 10. �ǵ�� ����.
    [HideInInspector]
    public int detail;

    [HideInInspector]
    public float speed;
    /*
     situation�� detail�� �켱������� idx�� �ű⵵�� ��. : index�� ���ؼ� �� ���� ��� ���� : idx�� �߰��� �ʿ��� ��� ���� �ٲٰų� float���� ���� �ٲ��ּ���!
    coroutine���� ���� �ð� �Ŀ� 10���� �ٲ���� �մϴ�.
     1. �浹
        1. ��ź�� �ָ��� 2. ������ ���ȰŸ�(bump, ���� ��)-> �����غ��� handstick �����ϸ� ��������� �� �� ����. 3. ��ֹ�/ ������(���) 4. �ٳ���
    2. �帮��Ʈ ����
        1. ���� ���
        
     4.
        1. �������� 2. ���� 3. �ӵ�
    5.
        1. ���ٴ�_1 2. ���ٴ�_2 3. �ܵ�
     */

    // Start is called before the first frame update
    void Start()
    {
        situation = 10;
        detail = 10;
        speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"situation : {situation} , detail: {detail}");
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("Collision Enter " + collision.collider.tag);
    }

    private void OnCollisionExit(Collision collision) {
        Debug.Log("Collision Exit " + collision.collider.tag);
    }

    private void OnCollisionStay(Collision collision) {
        //Debug.Log("Collision Stay " + collision.collider.tag);
        if(situation > 4) {
            if (collision.collider.CompareTag("Ground")) {
                situation = 10;}
            else {
                situation = 5;
                if (collision.collider.CompareTag("RockRoad_1")) { detail = 1; }
                else if (collision.collider.CompareTag("RockRoad_2")) { detail = 2; }
                else if (collision.collider.CompareTag("Grass")) { detail = 3; }
            }

        }   
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Trigger Enter" + other.tag);
        if (other.CompareTag("Bump")) { }
        else if (other.CompareTag("CircularBump")) { }
    }

    private void OnTriggerExit(Collider other) {
        // Debug.Log("Trigger Exit " + other.tag);
        if (other.CompareTag("Bump")) { }
        else if (other.CompareTag("CircularBump")) { }
    }

    private void OnTriggerStay(Collider other) {
        //Debug.Log("Trigger Stay " + other.tag);
    }


}
