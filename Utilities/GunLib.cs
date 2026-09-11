using GorillaLocomotion;
using System;
using UnityEngine;

namespace GoAwayMonkeys.Utilities;

public class GunLib
{
    private static bool hand = false;

    private static bool hand1 = false;

    private static RaycastHit raycastHit;

    public static GameObject pointer = null;

    public static void MakeGun(Color color, Vector3 pointersize, float linesize, PrimitiveType pointershape, Transform arm, bool liner, Action action, Action action1)
    {
        if (arm == GTPlayer.Instance.RightHand.controllerTransform)
        {
            hand = ControllerInputPoller.instance.rightControllerGripFloat > 0.6f;
            hand1 = ControllerInputPoller.instance.rightControllerTriggerButton;
        }
        else if (arm == GTPlayer.Instance.LeftHand.controllerTransform)
        {
            hand = ControllerInputPoller.instance.rightControllerGripFloat > 0.6f;
            hand1 = ControllerInputPoller.instance.rightControllerTriggerButton;
        }
        if (hand)
        {
            Physics.Raycast(arm.position, -arm.up, out raycastHit, float.MaxValue, NoInvisLayerMask());
            if (pointer == null) { pointer = GameObject.CreatePrimitive(pointershape); }
            pointer.name = "GunlibPointer";
            pointer.transform.localScale = pointersize;
            pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            pointer.transform.position = raycastHit.point;
            pointer.GetComponent<Renderer>().material.color = color;
            pointer.layer = LayerMask.NameToLayer("FirstPersonOnly");
            if (liner)
            {
                GameObject g = new GameObject("GunlibLine")
                {
                    layer = LayerMask.NameToLayer("FirstPersonOnly")
                };
                LineRenderer line = g.AddComponent<LineRenderer>();
                line.material.shader = Shader.Find("GUI/Text Shader");
                line.startWidth = linesize;
                line.endWidth = linesize;
                line.startColor = color;
                line.endColor = color;
                line.positionCount = 2;
                line.useWorldSpace = true;
                line.SetPosition(0, arm.position);
                line.SetPosition(1, pointer.transform.position);
                UnityEngine.Object.Destroy(g, Time.deltaTime);
            }
            UnityEngine.Object.Destroy(pointer.GetComponent<Collider>());
            UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
            if (hand1)
            {
                action.Invoke();
            }
            else
            {
                action1.Invoke();
            }
        }
        else
        {
            if (pointer != null)
            {
                UnityEngine.Object.Destroy(pointer, Time.deltaTime);
            }
        }
    }

    public static VRRig? GunHitRig()
    {
        foreach (VRRig rig in VRRigCache.ActiveRigs)
        {
            if (Vector3.Distance(pointer.transform.position, rig.transform.position) < 0.4f && !rig.isLocal)
            {
                return rig;
            }
        }
        return null;
    }

    private static int? noInvisLayerMask;
    public static int NoInvisLayerMask()
    {
        noInvisLayerMask ??= ~(
            1 << LayerMask.NameToLayer("TransparentFX") |
            1 << LayerMask.NameToLayer("Ignore Raycast") |
            1 << LayerMask.NameToLayer("Zone") |
            1 << LayerMask.NameToLayer("Gorilla Trigger") |
            1 << LayerMask.NameToLayer("Gorilla Boundary") |
            1 << LayerMask.NameToLayer("GorillaCosmetics") |
            1 << LayerMask.NameToLayer("GorillaParticle"));

        return noInvisLayerMask ?? GTPlayer.Instance.locomotionEnabledLayers;
    }
}
