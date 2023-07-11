using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Rigidbody))]
public class BallControl : MonoBehaviour
{
    public float throwForce = 100f;
    public float throwForceDirection_X = 0.17f;
    public float throwForceDirection_Y = 0.67f;
    public Vector3 ballCameraOffset = new Vector3(0f, -0.4f, 1f);

    private Vector3 startingPosition;
    private Vector3 direction;
    private float startTime;
    private float endTime;
    private float duration;
    private bool directionChosen = false;
    private bool throwStarted = false;

    [SerializeField]
    private GameObject arCam;

    [SerializeField]
    public ARSessionOrigin sessionOrigin;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        arCam = sessionOrigin.transform.Find("AR Camera").gameObject;
        transform.SetParent(arCam.transform);
        ResetBall();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startingPosition = Input.mousePosition;
            startTime = Time.time;
            throwStarted = true;
            directionChosen = false;
        }
        // Touch the screen to throw the ball
        else if (Input.GetMouseButtonUp(0))
        {
            endTime = Time.time;
            duration = endTime - startTime;
            direction = Input.mousePosition - startingPosition;
            directionChosen = true;
        }
        // Choose the direction to throw the ball
        if (directionChosen)
        {
            rb.mass = 1;
            rb.useGravity = true;
            rb.AddForce((arCam.transform.forward * throwForce / duration) +
                        Vector3.Scale(arCam.transform.up, new Vector3(direction.y * throwForceDirection_Y, direction.y * throwForceDirection_Y, direction.y * throwForceDirection_Y)) +
                        Vector3.Scale(arCam.transform.right, new Vector3(direction.x * throwForceDirection_X, direction.x * throwForceDirection_X, direction.x * throwForceDirection_X)));

            startTime = 0.0f;
            duration = 0.0f;
            startingPosition = Vector3.zero;
            direction = Vector3.zero;

            throwStarted = false;
            directionChosen = false;
        }

        // After 5 seconds, reset the ball's position
        if (Time.time - endTime >= 5 && Time.time - endTime <= 6)
        {
            ResetBall();
        }
    }

    public void ResetBall()
    {
        rb.mass = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        endTime = 0.0f;

        Vector3 ballpos = arCam.transform.position +
                          Vector3.Scale(arCam.transform.forward, new Vector3(ballCameraOffset.z, ballCameraOffset.z, ballCameraOffset.z)) +
                          Vector3.Scale(arCam.transform.up, new Vector3(ballCameraOffset.y, ballCameraOffset.y, ballCameraOffset.y));
        transform.position = ballpos;
    }
}