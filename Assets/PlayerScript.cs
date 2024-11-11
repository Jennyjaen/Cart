using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
public enum LInputGesture {
    Default,
    Right,
    Left,
}
public enum RInputGesture {
    Drift,
    Item,
    Drive,
    Neutral,
    Reverse,
    NoGesture
}

public class PlayerScript : MonoBehaviour {
    private Rigidbody rb;

    public enum InputMethod {
        KeyBoard,
        GamePad,
        HandStickThrottle,
        HandStickGesture,
        HandStickCombine
    }
    private LInputGesture LgestureState=LInputGesture.Default;
    private RInputGesture RgestureState= RInputGesture.Neutral;

    public InputMethod inputMethod;

    private float CurrentSpeed = 0;
    public float MaxSpeed = 40f;
    public float boostSpeed = 60f;
    private float RealSpeed; //not the applied speed
    [HideInInspector]
    public bool GLIDER_FLY;

    public Animator gliderAnim;

    [Header("Tires")]
    public Transform frontLeftTire;
    public Transform frontRightTire;
    public Transform backLeftTire;
    public Transform backRightTire;

    //drift and steering stuffz
    private float steerDirection;
    private float driftTime;

    bool driftLeft = false;
    bool driftRight = false;
    float outwardsDriftForce = 50000;

    public bool isSliding = false;
     
    private bool touchingGround;


    [Header("Particles Drift Sparks")]
    public Transform leftDrift;
    public Transform rightDrift;
    public Color drift1;
    public Color drift2;
    public Color drift3;

    [HideInInspector]
    public float BoostTime = 0;


    public Transform boostFire;
    public Transform boostExplosion;

    private bool drift_click = false;
    private bool drift_button = false;
    private EventScript status;

    private int gestureMovement = 0; //-1,0,1
    private bool gestureDrift = false;
    [HideInInspector]
    public bool gestureItem = false;

    private float bananaTimer=0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        status = transform.GetComponent<EventScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move();
        tireSteer();
        steer();
        groundNormalRotation();
        drift();
        boosts();
    }

    private void move()
    {
        RealSpeed = transform.InverseTransformDirection(rb.velocity).z; //real velocity before setting the value. This can be useful if say you want to have hair moving on the player, but don't want it to move if you are accelerating into a wall, since checking velocity after it has been applied will always be the applied value, and not real

        switch (inputMethod) {
            case InputMethod.KeyBoard:
                if (Input.GetKey(KeyCode.W)) {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, Time.deltaTime * 0.5f); //speed
                }
                else if (Input.GetKey(KeyCode.S)) {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, -MaxSpeed / 1.75f, 1f * Time.deltaTime);
                }
                else {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0, Time.deltaTime * 1.5f); //speed
                }
                break;
            case InputMethod.GamePad:
                PlayerIndex playerIndex = PlayerIndex.One;
                GamePadState state = GamePad.GetState(playerIndex); 

                if (state.ThumbSticks.Left.Y > 0.1f)
                {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, Time.deltaTime * 0.3f * state.ThumbSticks.Left.Y);
                }
                else if (state.ThumbSticks.Left.Y < -0.1f)
                {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, -MaxSpeed / 1.75f, Time.deltaTime * 0.8f * Mathf.Abs(state.ThumbSticks.Left.Y));
                }
                else
                {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0, Time.deltaTime * 1.5f);
                }
                break;
            case InputMethod.HandStickGesture:
                if (gestureMovement>0) {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, Time.deltaTime * 0.5f); //speed
                }
                else if (gestureMovement < 0) {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, -MaxSpeed / 1.75f, 1f * Time.deltaTime);
                }
                else {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0, Time.deltaTime * 1.5f);
                }
                break;

        }
        

        if (!GLIDER_FLY)
        {
            Vector3 vel = transform.forward * CurrentSpeed;
            vel.y = rb.velocity.y; //gravity
            rb.velocity = vel;
        }
        else
        {
            Vector3 vel = transform.forward * CurrentSpeed;
            vel.y = rb.velocity.y * 0.6f; //gravity with gliding
            rb.velocity = vel;
        }
        

    }
    private void steer()
    {
        steerDirection = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        Vector3 steerDirVect; //this is used for the final rotation of the kart for steering

        float steerAmount;

        if (driftLeft && !driftRight)
        {
            steerDirection = Input.GetAxis("Horizontal") < 0 ? -1.5f : -0.5f;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, -20f, 0), 8f * Time.deltaTime);
            
            
            if(isSliding && touchingGround)
               rb.AddForce(transform.right * outwardsDriftForce * Time.deltaTime, ForceMode.Acceleration);
        }
        else if (driftRight && !driftLeft)
        {
            steerDirection = Input.GetAxis("Horizontal") > 0 ? 1.5f : 0.5f;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 20f, 0), 8f * Time.deltaTime);

            if(isSliding && touchingGround)
                rb.AddForce(transform.right * -outwardsDriftForce * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0f, 0), 8f * Time.deltaTime);
        }

        //since handling is supposed to be stronger when car is moving slower, we adjust steerAmount depending on the real speed of the kart, and then rotate the kart on its y axis with steerAmount
        steerAmount = RealSpeed > 30 ? RealSpeed / 4 * steerDirection : steerAmount = RealSpeed / 1.5f * steerDirection;

        bool leftmove = false;
        bool rightmove = false;
        bool upmove = false;
        bool downmove = false;

        switch (inputMethod) {
            case InputMethod.KeyBoard:
                leftmove = Input.GetKey(KeyCode.LeftArrow);
                rightmove = Input.GetKey(KeyCode.RightArrow);
                upmove = Input.GetKey(KeyCode.UpArrow);
                downmove =Input.GetKey(KeyCode.DownArrow) ;
                break;
            case InputMethod.GamePad:
                PlayerIndex playerIndex = PlayerIndex.One; 
                GamePadState state = GamePad.GetState(playerIndex);
                leftmove = state.ThumbSticks.Left.X < -0.1f;
                rightmove = state.ThumbSticks.Left.X > 0.1f;
                upmove = state.ThumbSticks.Left.Y > 0.1f;
                downmove = state.ThumbSticks.Left.Y < -0.1f;
                break;
        }

        //glider movements

        if (leftmove && GLIDER_FLY)  //left
        {
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 40), 2 * Time.deltaTime);          
        } // left 
        else if (rightmove && GLIDER_FLY) //right
        {
          
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -40), 2 * Time.deltaTime);
        } //right
        else //nothing
        {
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0), 2 * Time.deltaTime);
        } //nothing

        if (upmove && GLIDER_FLY) 
        {
            
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(25, transform.eulerAngles.y, transform.eulerAngles.z), 2 * Time.deltaTime);
            
            rb.AddForce(Vector3.down * 8000 * Time.deltaTime, ForceMode.Acceleration);
        } //moving down
        else if (downmove && GLIDER_FLY)  
        {
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(-25, transform.eulerAngles.y, transform.eulerAngles.z), 2 * Time.deltaTime);
            rb.AddForce(Vector3.up * 4000 * Time.deltaTime, ForceMode.Acceleration);

        } //rotating up - only use this if you have special triggers around the track which disable this functionality at some point, or the player will be able to just fly around the track the whole time
        else
        {
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z), 2 * Time.deltaTime);
        }

        if (bananaTimer > 0)
        {
            steerAmount = 240;
            bananaTimer -= Time.deltaTime;
        }

        steerDirVect = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + steerAmount, transform.eulerAngles.z);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, steerDirVect , 3 * Time.deltaTime); 

    }
    private void groundNormalRotation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.75f))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
        }
    }
    private void drift()
    {

        switch (inputMethod) {
            case InputMethod.KeyBoard:
                drift_button = Input.GetKey(KeyCode.V);
                drift_click = Input.GetKeyDown(KeyCode.V);
                break;
            case InputMethod.GamePad:
                GamePadState state = GamePad.GetState(PlayerIndex.One);
                bool button_state = (state.Buttons.RightShoulder == ButtonState.Pressed);
                if(button_state && !drift_button) { drift_click = true; }
                else { drift_click = false; }
                drift_button = button_state;
                break;
            case InputMethod.HandStickGesture:
            case InputMethod.HandStickCombine:
                drift_click = gestureDrift && !drift_button;
                drift_button = gestureDrift;
                break;
        }
        if (drift_click && touchingGround)
        {
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hop");
            if(steerDirection > 0)
            {
                driftRight = true;
                driftLeft = false;
            }
            else if(steerDirection < 0)
            {
                driftRight = false;
                driftLeft = true;
            }
        }


        if (drift_button && touchingGround && CurrentSpeed > 10 && Input.GetAxis("Horizontal") != 0)
        {
            driftTime += Time.deltaTime;

            //particle effects (sparks)
            if (driftTime >= 1.5 && driftTime < 4)
            {

                for (int i = 0; i < leftDrift.childCount; i++)
                {
                    ParticleSystem DriftPS = rightDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //right wheel particles
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;

                    ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //left wheel particles
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

                    PSMAIN.startColor = drift1;
                    PSMAIN2.startColor = drift1;

                    if (!DriftPS.isPlaying && !DriftPS2.isPlaying)
                    {
                        DriftPS.Play();
                        DriftPS2.Play();
                    }

                }
            }
            if (driftTime >= 4 && driftTime < 7)
            {
                //drift color particles
                for (int i = 0; i < leftDrift.childCount; i++)
                {
                    ParticleSystem DriftPS = rightDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;
                    ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;
                    PSMAIN.startColor = drift2;
                    PSMAIN2.startColor = drift2;


                }

            }
            if (driftTime >= 7)
            {
                for (int i = 0; i < leftDrift.childCount; i++)
                {

                    ParticleSystem DriftPS = rightDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;
                    ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;
                    PSMAIN.startColor = drift3;
                    PSMAIN2.startColor = drift3;

                }
            }
        }

        if (!drift_button || RealSpeed < 10)
        {
            driftLeft = false;
            driftRight = false;
            isSliding = false; /////////



            //give a boost
            if (driftTime > 1.5 && driftTime < 4)
            {
                BoostTime = 0.75f;
            }
            if (driftTime >= 4 && driftTime < 7)
            {
                BoostTime = 1.5f;
                
            }
            if (driftTime >= 7)
            {
                BoostTime = 2.5f;
               
            }

            //reset everything
            driftTime = 0;
            //stop particles
            for (int i = 0; i < 5; i++)
            {
                ParticleSystem DriftPS = rightDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //right wheel particles
                ParticleSystem.MainModule PSMAIN = DriftPS.main;

                ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //left wheel particles
                ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

                DriftPS.Stop();
                DriftPS2.Stop();

            }
        }



    }
    private void boosts()
    {
        BoostTime -= Time.deltaTime;
        if(BoostTime > 0)
        {
            for(int i = 0; i < boostFire.childCount; i++)
            {
                if (! boostFire.GetChild(i).GetComponent<ParticleSystem>().isPlaying)
                {
                    boostFire.GetChild(i).GetComponent<ParticleSystem>().Play();
                }

            }
            MaxSpeed = boostSpeed;

            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, 1 * Time.deltaTime);
        }
        else
        {
            for (int i = 0; i < boostFire.childCount; i++)
            {
                boostFire.GetChild(i).GetComponent<ParticleSystem>().Stop();
            }
            MaxSpeed = boostSpeed - 20;
        }
    }

    private void tireSteer() //타이어 회전
    {
        switch (inputMethod) {
            case InputMethod.KeyBoard:
                if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 155, 0), 5 * Time.deltaTime);
                        frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 155, 0), 5 * Time.deltaTime);
                    }
                    else if (Input.GetKey(KeyCode.RightArrow))
                    {
                        frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 205, 0), 5 * Time.deltaTime);
                        frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 205, 0), 5 * Time.deltaTime);
                    }
                    else
                    {
                        frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 180, 0), 5 * Time.deltaTime);
                        frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 180, 0), 5 * Time.deltaTime);
                    }
                break;
            case InputMethod.GamePad:
                PlayerIndex playerIndex = PlayerIndex.One; // 첫 번째 컨트롤러
                GamePadState state = GamePad.GetState(playerIndex); // 현재 게임패드 상태
                float changeSpeed = Mathf.Abs(state.ThumbSticks.Left.X * 5);

                if (state.ThumbSticks.Left.X < -0.1f) // Thumbstick X축이 왼쪽으로 이동
                {
                    frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 155, 0), changeSpeed * Time.deltaTime);
                    frontRightTire.localEulerAngles = Vector3.Lerp(frontRightTire.localEulerAngles, new Vector3(0, 155, 0), changeSpeed * Time.deltaTime);
                }
                else if (state.ThumbSticks.Left.X > 0.1f) // Thumbstick X축이 오른쪽으로 이동
                {
                    frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 205, 0), changeSpeed * Time.deltaTime);
                    frontRightTire.localEulerAngles = Vector3.Lerp(frontRightTire.localEulerAngles, new Vector3(0, 205, 0), changeSpeed * Time.deltaTime);
                }
                else // Thumbstick이 중립에 있을 때
                {
                    frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 180, 0), changeSpeed * Time.deltaTime);
                    frontRightTire.localEulerAngles = Vector3.Lerp(frontRightTire.localEulerAngles, new Vector3(0, 180, 0), changeSpeed * Time.deltaTime);
                }
                break;
        }
        

        //tire spinning

        if (CurrentSpeed > 30)
        {
            frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
            frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
            backLeftTire.Rotate(90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
            backRightTire.Rotate(90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
        }
        else
        {
            frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
            frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
            backLeftTire.Rotate(90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
            backRightTire.Rotate(90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
        }



    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "GliderPanel")
        {
            GLIDER_FLY = true;
            if (status.situation > 4) {
                status.situation = 4;
                if (status.detail > 2) {
                    status.detail = 2;
                }
            }
            gliderAnim.SetBool("GliderOpen", true);
            gliderAnim.SetBool("GliderClose", false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground" || collision.gameObject.tag == "OffRoad")
        {
            if (GLIDER_FLY) {
                //날고 있다가 바닥에 부딪히는 경우
                StartCoroutine(Landing());
            }
            GLIDER_FLY = false;
            gliderAnim.SetBool("GliderOpen", false);
            gliderAnim.SetBool("GliderClose", true);
        }
    }

    IEnumerator Landing() {
        if (status.situation > 1) {
            status.situation = 1;
            if (status.detail > 2) {
                status.detail = 2;
            }
        }
        yield return new WaitForSeconds(3 * (rb.velocity.magnitude / 5));

        status.situation = 10;
        status.detail = 10;
    }

    public void getRGesture(RInputGesture gesture) {
        if(gesture!=RInputGesture.NoGesture)Debug.Log(gesture);
        switch (gesture) {
            case RInputGesture.Drive:
                gestureMovement = 1;
                break;
            case RInputGesture.Neutral:
                gestureMovement = 0;
                break;
            case RInputGesture.Reverse:
                gestureMovement = -1;
                break;
            case RInputGesture.Drift:
                gestureDrift = !gestureDrift;
                break;
            case RInputGesture.Item:
                gestureItem = true;
                break;
            default:
                gestureItem = false;
                break;
        }
    }

    public void bananaSplit() {
        bananaTimer = 0.5f;
    }
}
