using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScript : MonoBehaviour
{
    //현재 상황을 detect해서 situation과 detail을 update
    //Hatpic Feedback.cs에서 detail과 situation을 바탕으로 array update
    [HideInInspector]
    public int situation;
    //  1: 충돌 2: 드리프트/ 가속 3. 아이템 4. 모터 진동/ 공중/ 속도 5. 바닥의 질감 10. 피드백 없음.
    [HideInInspector]
    public int detail;

    [HideInInspector]
    public float speed;
    /*
     situation과 detail은 우선순위대로 idx를 매기도록 함. : index를 비교해서 더 작은 경우 적용 : idx가 추가로 필요한 경우 값을 바꾸거나 float으로 만들어서 바꿔주세요!
    coroutine으로 일정 시간 후에 10으로 바꿔줘야 합니다.
     1. 충돌
        1. 폭탄에 휘말림 2. 도로의 덜컹거림(bump, 착륙 등)-> 생각해보니 handstick 생각하면 나누어줘야 할 것 같음. 3. 장애물/ 아이템(등껍질) 4. 바나나
    2. 드리프트 가속
        1. 버섯 사용
        
     4.
        1. 모터진동 2. 공중 3. 속도
    5.
        1. 돌바닥_1 2. 돌바닥_2 3. 잔디
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
