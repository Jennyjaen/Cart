using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bomb : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    public float throwForceUp;
    public float throwForceForward;

    public float lifetime;

    [HideInInspector]
    public float bounce_count = 1;
    public float bounceForce;
    public GameObject explosion;
    //public Transform explosionPos;
    //public Transform smokePos;
    //public GameObject smoke;

    public SkinnedMeshRenderer[] renderers;
    public Material[] regMat;
    public Material glowMat;
    //public GameObject[] spark;

    private bool exploded = false;
    private bool landed = false;

    //private Transform closest_Player;
    //public Transform[] players;

    public float moveSpeed;

    bool countDownColor = false;
    private EventScript status;

    private float explosionRadius = 15f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //renderers[1].material = regMat[1];
        renderers[0].material = regMat[0];
        bomb_thrown(10);
        status = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EventScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!rb)
            rb = GetComponent<Rigidbody>();
        //rb.AddRelativeForce(Vector3.down * 10000 * Time.deltaTime, ForceMode.Acceleration);

        /*if (landed)
        {
            //Move();
            //groundNormalRotation();
            if (!countDownColor)
            {
                StartCoroutine(countdownColor());
                countDownColor = true;
            }
        }*/

        /*if (exploded)
        {
            GetComponent<AudioSource>().Stop();
        }*/

        lifetime -= Time.deltaTime;
        if (lifetime < 0 & !countDownColor) {
            StartCoroutine(countdownColor());
            StartCoroutine(Explode());
            countDownColor = true;
        }
    }
    /*
    public void Move()
    {
        if (!GetComponent<AudioSource>().isPlaying && !exploded && Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < 50)
        {
            GetComponent<AudioSource>().Play();
        }
        GetComponent<Animator>().SetBool("Moving", true);
        float shortDistance = 999999;
        for (int i = 0; i < players.Length; i++)
        {
            float distance = Vector3.Distance(players[i].position, transform.position);
            if (shortDistance > distance)
            {
                shortDistance = distance;
                closest_Player = players[i];
            }
        }


        //fix this
        //angle calc
        Vector3 myangle = closest_Player.position - transform.position;
        Vector3 angle = Vector3.Cross(transform.forward, myangle);
        float dir = Vector3.Dot(angle, transform.up);


        float none = 0;

        // maybe get dir, and make float y lerp to that dir value, and then rotate y axis (space.self) according to that y value or something

        //float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 2.5f * Time.deltaTime);
        y = Mathf.SmoothDamp(y, dir, ref none, 2.5f * Time.deltaTime);


        transform.Rotate(0, y / 2, 0, Space.Self);

        rb.velocity = transform.TransformDirection(0, rb.velocity.y, moveSpeed * Time.deltaTime); //goes in direction thingy is facing in as its positive z value

    }*/

    public void bomb_thrown(float extraForward)
    {
        rb.AddForce(transform.up * throwForceUp , ForceMode.Impulse);
        rb.AddForce(transform.forward * (throwForceForward + extraForward) , ForceMode.Impulse);
    }


    void groundNormalRotation()
    {
        //ground normal rotation
        Ray ground = new Ray(transform.position, transform.InverseTransformDirection(Vector3.down));
        RaycastHit hit;
        if (Physics.Raycast(ground, out hit, 5))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 9f * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Dirt")
        {
            //groundNormalRotation();

            /*if (bounce_count < 1)
            {
                rb.AddRelativeForce(transform.InverseTransformDirection(transform.up) * bounceForce / (bounce_count * 1.5f) * Time.deltaTime, ForceMode.Impulse);
                yield return new WaitForSeconds(0.01f);
                bounce_count++;
            }
            if (bounce_count == 1)
            {
                StartCoroutine(Explode());
                landed = true;
            }
            */
            //landed = true;
            //StartCoroutine(Explode());
        }

        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Opponent")
        {
            StartCoroutine(explodeImmediately());
        }
    }


    IEnumerator Explode()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("This script is attached to: " + gameObject.name);
        if (!exploded)
        {
            //GameObject clone = Instantiate(explosion, explosionPos.position, explosion.transform.rotation);
            GameObject clone = Instantiate(explosion, transform.position, explosion.transform.rotation);

            //Instantiate(smoke, smokePos.position, smokePos.rotation);
            /*for (int i = 0; i < spark.Length; i++)
            {
                spark[i].SetActive(false);
            }*/
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }
            Debug.Log("GAAAAAAAAAAAAAAAAAAAAAAH");
            Debug.Log(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position));
            if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < explosionRadius)
            {
                //TODO: Player Explosion Logic
                float factor = (explosionRadius - Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position)) / explosionRadius;
                Debug.Log(GameObject.FindGameObjectWithTag("Player").name);
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().itemHit(30*factor);
                if (status.situation > 1)
                {
                    status.situation = 1;
                    status.detail = 1;
                }
            }
            exploded = true;
            
        }
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);

        //explode end
        status.situation = 10;
        status.detail = 10;
        Debug.Log("change value");

    }

    IEnumerator explodeImmediately()
    {
        if (!exploded)
        {
            //Debug.Log("This script is attached to: " + gameObject.name);
            //GameObject clone = Instantiate(explosion, explosionPos.position, explosion.transform.rotation);
            GameObject clone = Instantiate(explosion, transform.position, explosion.transform.rotation);
            //Instantiate(smoke, smokePos.position, smokePos.rotation);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }
            /*for (int i = 0; i < spark.Length; i++)
            {
                spark[i].SetActive(false);
            }*/
            Debug.Log("GAAAAAAAAAAAAAAAAAAAAAAH");
            Debug.Log(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position));
            if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < explosionRadius)
            {
                //TODO: Player Explosion Logic
                //float factor = (250-Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position))/250;
                Debug.Log(GameObject.FindGameObjectWithTag("Player").name);
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().itemHit(30);
                if (status.situation > 1)
                {
                    status.situation = 1;
                    status.detail = 1;
                }
            }
            exploded = true;
            //gameObject.SetActive(false);
            
        }
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);

        //explode end
        status.situation = 10;
        status.detail = 10;
        Debug.Log("change value2");
    }

    IEnumerator countdownColor()
    {
        while (!exploded)
        {
            renderers[0].material = glowMat;
            yield return new WaitForSeconds(0.2f);
            renderers[0].material = regMat[0];
            yield return new WaitForSeconds(0.2f);
        }
    }
}