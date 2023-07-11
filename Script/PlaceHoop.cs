
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceHoop : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_BasketPrefab;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_BasketPrefab; }
        set { m_BasketPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnHoop { get; private set; }

    [SerializeField]
    [Tooltip("Instantiates this prefab in frond of  the AR Camera")]
    GameObject m_BallPrefab;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject BallPrefab
    {
        get { return m_BallPrefab; }
        set { m_BallPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnBall { get; private set; }


    public static event Action onPlacedObject;

    //Raycasting is used in the provided code to detect and interact with the ARenvironment. 
    //plsing object ,collition
    private bool isPlaced = false;
    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if(isPlaced)
        return;
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = s_Hits[0].pose;
                    //spawn the hoop 
                    spawnHoop = Instantiate(m_BasketPrefab, hitPose.position,Quaternion.AngleAxis(180,Vector3.up));
                    spawnHoop.transform.parent = transform.parent;
                    isPlaced =true;

                    spawnBall = Instantiate(m_BallPrefab);
                    spawnBall.transform.parent = m_RaycastManager.transform.Find("AR Camera").gameObject.transform;  


                    if (onPlacedObject != null)
                    {
                        onPlacedObject();
                    }
                }
            }
        }
    }
}
