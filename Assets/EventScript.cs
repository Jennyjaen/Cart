using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScript : MonoBehaviour
{
    //���� ��Ȳ�� detect�ؼ� situation�� detail�� update
    //Hatpic Feedback.cs���� detail�� situation�� �������� array update
    [HideInInspector]
    public int situation;
    // 0; �ǵ�� x 1: �浹 2: �帮��Ʈ 3. �ٴ��� ���� 4. ���� ���� 5. ������
    [HideInInspector]
    public int detail;
    // Start is called before the first frame update
    void Start()
    {
        situation = 0;
        detail = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("Collision Enter " + collision.collider.tag);
    }

    private void OnCollisionExit(Collision collision) {
        Debug.Log("Collision Exit " + collision.collider.tag);
    }

    private void OnCollisionStay(Collision collision) {
        //Debug.Log("Collision Stay " + collision.collider.tag);
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Trigger Enter" + other.tag);
    }

    private void OnTriggerExit(Collider other) {
       // Debug.Log("Trigger Exit " + other.tag);
    }

    private void OnTriggerStay(Collider other) {
        //Debug.Log("Trigger Stay " + other.tag);
    }


}
