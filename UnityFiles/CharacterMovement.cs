using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;
public class CharacterMovement : MonoBehaviour
{

    public Camera cam;
    public NavMeshAgent agent;
    public ThirdPersonCharacter charact;
    // Start is called before the first frame update
    void Start()
    {
        agent.updateRotation = false;
    }
    public static bool won = false;
    public GameObject GoButton;
    public static Vector3 winPosition;
    // Update is called once per frame
    public static bool followed = false;
    bool justonce = false;
    bool changepos = false;
    public Text followtext;
    void Update()
    {
        if (Input.GetMouseButtonDown(1))//$!won
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
            GetComponent<Animator>().SetBool("Win", false);
            if (!GoButton.activeSelf) GoButton.SetActive(true);
            justonce = false;
        }
        if (agent.remainingDistance > agent.stoppingDistance || won)
        {
            charact.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            charact.Move(Vector3.zero, false, false);
            if(Vector3.Distance(winPosition,transform.position)<0.8f)
            {
                GetComponent<Animator>().SetBool("Win", true);
                if(!CameraMovement.startfollowing && !justonce)
                {
                    Vector3 newrotation = cam.GetComponent<CameraMovement>().newrotation;
                    
                    cam.transform.rotation = Quaternion.Euler(newrotation);
                    //cam.transform.LookAt(transform);
                    CameraMovement.gpos = cam.GetComponent<CameraMovement>().oldpos;
                    CameraMovement.startfollowing = true;
                    followtext.text = "Unfollow";
                    cam.GetComponent<Camera>().fieldOfView = 36;
                    won = true;
                    Manager.followed = true;
                }
                justonce = true;
                won = true;
            }
        }
    }
    public void goonow()
    {
        agent.SetDestination(winPosition);
        charact.Move(agent.desiredVelocity, false, false);
        justonce = false;
    }
    void FixedUpdate()
    {
        if(changepos)
        {
            Vector3 nextpost = transform.position + cam.GetComponent<CameraMovement>().offset;
            Vector3 oldpos = cam.GetComponent<CameraMovement>().oldpos;
            Vector3 smoothed = Vector3.Lerp(oldpos, nextpost, 0.125f);
            cam.transform.position = smoothed;
            oldpos = cam.transform.position;   
           //need to upload
        }
    }
}
