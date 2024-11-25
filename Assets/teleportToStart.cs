using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportToStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>() != null) {
            var prevPos = other.gameObject.transform.position;
            other.gameObject.transform.position = new Vector3(prevPos.x, prevPos.y, 650);
        }
    }
}