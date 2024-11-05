using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS script is for shells moving around the track shot by opponents or player

public class GreenShell : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private Rigidbody rb;

    public Vector3 myVelocity;
    [HideInInspector]
    public float lifetime = 0;


    bool grounded = true;
    bool antiGravityGrounded = false;

    public LayerMask mask;


    [HideInInspector]
    public bool AntiGravity = false;

    [HideInInspector]
    public float velocityMagOriginal;

    public bool needsExtraDownForceAntigravity = false;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

    }

    public void InitialShoot(float speed, Vector3 direction) {
        myVelocity = direction*speed;
        velocityMagOriginal = speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        groundNormalRotation();


    }

    private void Move()
    {
        //myVelocity = myVelocity.normalized;
        //myVelocity *= velocityMagOriginal * Time.deltaTime;

        //if (!AntiGravity)
        //    myVelocity.y = rb.velocity.y;


        rb.velocity = myVelocity;



        lifetime += Time.deltaTime;
        if (lifetime > 20) { destroyShell(); }

    }
    void groundNormalRotation()
    {
        //ground normal rotation
        Ray ground = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ground, out hit, 10, mask))
        {
            transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 13f * Time.deltaTime);


        }

    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ground" && collision.gameObject.tag != "Dirt" && collision.gameObject.tag != "JumpPanel" && collision.gameObject.tag != "ShellPlatforms")
        {


            if (collision.gameObject.tag != "GliderPanel")
            {
                //Debug.Log("GAH");
                Vector3 oldvel = myVelocity;

                rb.velocity = Vector3.zero;
                var prevY = myVelocity.y;
                myVelocity = Vector3.Reflect(myVelocity, collision.contacts[0].normal);
                myVelocity.y = prevY;


                if (lifetime > 20)
                {
                    destroyShell();
                }
            }

            if (collision.gameObject.tag == "Player")
            {
                /*if (lifetime > 0.05f)
                {
                    if (!collision.gameObject.GetComponent<ItemManager>().StarPowerUp)
                    {
                        StartCoroutine(collision.gameObject.GetComponent<Player>().hitByShell()); //the player has the function that does all this work

                        if (rm.FrontCam.activeSelf)
                            GameObject.Find("Main Camera").GetComponent<Animator>().SetTrigger("ShellHit");
                    }
                    destroyShell();
                }*/
                if (lifetime > 0.05f)
                {
                    destroyShell();

                }
            }


        }


    }


    public void destroyShell()
    {
        int x = transform.GetChild(0).childCount; //particle systems
        for (int i = 0; i < x; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>().Play();
        }
        transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
        sphereCollider.enabled = false;
        rb.isKinematic = true;



        Destroy(gameObject, 3);

    }
}