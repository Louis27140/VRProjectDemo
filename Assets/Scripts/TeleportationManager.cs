using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    [SerializeField] 
    private InputActionAsset input;

    [SerializeField]
    private TeleportationProvider provider;

    private InputAction thumbStick;

    [SerializeField]
    private XRRayInteractor teleportRay;

    private bool isActive;


    // Start is called before the first frame update
    void Start()
    {
        teleportRay.enabled = false;

        var activate = input.FindActionMap("XRI RightHand Locomotion").FindAction("Teleport Mode Activate");
        activate.Enable();
        activate.performed += OnTeleportActivate;


        var cancel = input.FindActionMap("XRI RightHand Locomotion").FindAction("Teleport Mode Cancel");
        cancel.Enable();
        cancel.performed += OnTeleportCancel;

        thumbStick = input.FindActionMap("XRI RightHand Locomotion").FindAction("move");
        thumbStick.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
            return;

        if (thumbStick.triggered)
            return;

        if (!teleportRay.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            teleportRay.enabled = false;
            isActive = false;
            return;

        }

        TeleportRequest tpRequest = new TeleportRequest()
        {
            destinationPosition = hit.point
        };

        provider.QueueTeleportRequest(tpRequest);
        teleportRay.enabled = false;
        isActive = false;
    }

    private void OnTeleportActivate(InputAction.CallbackContext ctx)
    {
        teleportRay.enabled = true;
        isActive = true;
    }

    private void OnTeleportCancel(InputAction.CallbackContext ctx)
    {
        teleportRay.enabled = false;
        isActive = false;
    }
}
