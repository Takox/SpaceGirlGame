﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public GameObject handObject;
    public float detectionRadius;

    private bool hasObject = false;
    private bool inPlace = false;
    private Collider2D[] arounds;
    private GameObject grabbedObject;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, detectionRadius);
    }

    private void Update()
    {
        CheckAround();
        if(Input.GetMouseButtonDown(1) && !hasObject)
            Grab();
        if (Input.GetMouseButtonUp(1) && hasObject && inPlace)
            stickObject();
        else if (Input.GetMouseButtonUp(1) && hasObject)
            Drop();
    }

    private void CheckAround()
    {
        arounds = 
            Physics2D.OverlapCircleAll(transform.position, detectionRadius);
    }

    private void Grab()
    {
        if(arounds != null)
        {
            for(int i = 0; i < arounds.Length && !hasObject; i++)
            {
                Debug.Log(i);
                if (!hasObject && arounds[i].gameObject.tag == "Caja")
                {
                    Debug.Log("picking obj");
                    //pillar objeto
                    grabbedObject = arounds[i].gameObject;
                    takeOutPhysics(grabbedObject);
                    grabbedObject.transform.position = handObject.transform.position;
                    grabbedObject.transform.parent = handObject.transform;
                    hasObject = true;
                }
            }
        }
    }

    private void Drop()
    {
        if (hasObject)
        {
            grabbedObject.transform.parent = null;
            returnPhysics(grabbedObject);
            grabbedObject = null;
            hasObject = false;
        }
    }

    private void takeOutPhysics(GameObject pickup)
    {
        pickup.GetComponent<Rigidbody2D>().simulated = false;
        pickup.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void returnPhysics(GameObject pickup)
    {
        pickup.GetComponent<Rigidbody2D>().simulated = true;
        pickup.GetComponent<BoxCollider2D>().enabled = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Zone" && hasObject)
        {
            inPlace = true;
        }
    }

    private void stickObject()
    {
        takeOutPhysics(grabbedObject);
        Quaternion originalRotation =  grabbedObject.GetComponent<ObjectController>().getOriginalRotation();
        Transform originalTransform = grabbedObject.GetComponent<ObjectController>().getOriginalTransform();
        grabbedObject.transform.rotation = Quaternion.Slerp(grabbedObject.transform.rotation, originalRotation, Time.deltaTime * 180f);
        grabbedObject.transform.position = originalTransform.position;
        grabbedObject.transform.parent = null;

    }
}
