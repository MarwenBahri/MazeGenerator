//
//Filename: maxCamera.cs
//
// original: http://www.unifycommunity.com/wiki/index.php?title=MouseOrbitZoom
//
// --01-18-2010 - create temporary target, if none supplied at start

using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    public static bool startfollowing = false;
    int fieldofview = 36;
    public Text followtext;
    public static bool unfollow = false;
    public void followbutton()
    {

        if (!startfollowing)
        {
            
            //transform.rotation = Quaternion.Euler(newrotation);
            GetComponent<Camera>().fieldOfView = fieldofview;
            gpos = oldpos;
            startfollowing = true;
            followtext.text = "Unfollow";
        }else
        {
            startfollowing = false;
            followtext.text = "Follow";
            unfollow = true;
            
            transform.rotation = Quaternion.Euler(oldrotation);
        }
        
        
    }
    
    public Transform player;
    public Vector3 offset ;
    public Vector3 camrotation;
    public Vector3 newrotation;
    public Vector3 oldpos = new Vector3(0.2f, 14.44917f, - 0.533864f);
    public Vector3 oldrotation = new Vector3(86.01401f, -0.482f, -0.558f);
    public static Vector3 gpos;
    void LateUpdate()
    {
        if(!startfollowing)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                GetComponent<Camera>().fieldOfView--;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                GetComponent<Camera>().fieldOfView++;
            }
            // If middle mouse and left alt are selected? ORBIT
            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt))
            {
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                ////////OrbitAngle

                //Clamp the vertical axis for the orbit
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                // set camera rotation 
                desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
                currentRotation = transform.rotation;

                rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
                transform.rotation = rotation;
            }
            // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
            else if (Input.GetMouseButton(2))
            {
                //grab the rotation of the camera so we can move in a psuedo local XY space
                //target.rotation = transform.rotation;
                target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
                target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
            }

            ////////Orbit Position

            // affect the desired Zoom distance if we roll the scrollwheel
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
            //clamp the zoom min/max
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            // For smoothing of the zoom, lerp distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            // calculate position based on the new currentDistance 
            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
            transform.position = position;
        }
        
    }
    void FixedUpdate()
    {

        if (startfollowing)
        {
            Vector3 nextpost = player.transform.position + offset;
            Vector3 smoothed = Vector3.Lerp(gpos, nextpost, 0.125f);
            transform.position = smoothed;
            gpos = transform.position;
            transform.LookAt(player);
            
        }
        if (unfollow)
        {
            Vector3 smoothed = Vector3.Lerp(transform.position, oldpos, 0.125f);
            transform.position = smoothed;
            unfollow = !(transform.position == oldpos);
        }   
    }
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}