using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabRemote : XRGrabInteractable
{
    public float velocityTreshold = 2;
    public float JumpAngleInDegree = 60;

    private XRRayInteractor ray;
    private Vector3 previousPos;
    private Rigidbody interactableRigidbody;

    private bool canJump = true;

    protected override void Awake()
    {
        base.Awake();
        interactableRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isSelected && firstInteractorSelecting is XRRayInteractor && canJump)
        {
            Vector3 velocity = (ray.transform.position - previousPos) / Time.deltaTime;
            previousPos = ray.transform.position;

            if (velocity.magnitude > velocityTreshold)
            {
                Drop();
                interactableRigidbody.velocity = computeVelocity();
                canJump = false;
            }

        }
    }

    public Vector3 computeVelocity()
    {
        Vector3 diff = ray.transform.position - transform.position;
        Vector3 diffXZ = new Vector3(diff.x, 0, diff.z);
        float diffXZLength = diffXZ.magnitude;
        float diffYLength = diff.y;

        float angleInRadian = JumpAngleInDegree * Mathf.Deg2Rad;

        float impulseSpeed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(diffXZLength, 2)
            / (2 * Mathf.Cos(angleInRadian) * Mathf.Cos(angleInRadian) * (diffXZ.magnitude * Mathf.Tan(angleInRadian) - diffYLength)));

        Vector3 impulseVector = diffXZ.normalized * Mathf.Cos(angleInRadian) * impulseSpeed + Vector3.up * Mathf.Sin(angleInRadian) * impulseSpeed;

        return impulseVector;


    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor)
        {
            trackPosition = false;
            trackRotation = false;
            throwOnDetach = false;

            ray = (XRRayInteractor)args.interactorObject;
            previousPos = ray.transform.position;
            canJump = true;
        } else
        {
            trackPosition = true;
            trackRotation = true;
            throwOnDetach = true;
        }

        base.OnSelectEntered(args);
    }


}
