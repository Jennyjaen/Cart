using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScript : MonoBehaviour
{
    //현재 상황을 detect해서 situation과 detail을 update
    //Hatpic Feedback.cs에서 detail과 situation을 바탕으로 array update
    [HideInInspector]
    public int situation;
    // 0; 피드백 x 1: 충돌 2: 드리프트 3. 바닥의 질감 4. 모터 진동 5. 아이템
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
