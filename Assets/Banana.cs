using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    public float throwForceUp;
    public float throwForceForward;
    public float rotationForce;

    public float lifetime;

    private EventScript status;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        banana_thrown(10);
        status = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EventScript>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!rb)
            rb = GetComponent<Rigidbody>();
    }

    public void banana_thrown(float extraForward)
    {
        rb.AddForce(transform.up * throwForceUp, ForceMode.Impulse);
        rb.AddForce(transform.forward * (throwForceForward + extraForward), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Opponent")
        {
            StartCoroutine(Hit());
        }
    }

    IEnumerator Hit()
    {
        gameObject.SetActive(false);
        if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < 250)
        {
            //TODO: Player Explosion Logic

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().bananaSplit();

            if (status.situation > 1)
            {
                status.situation = 1;
                status.detail = 1;
            }
        }
        yield return new WaitForSeconds(2);

        status.situation = 10;
        status.detail = 10;

    }

}
