using BepInEx;
using GoAwayMonkeys.Classes;
using GoAwayMonkeys.MonoBehaviors;
using GoAwayMonkeys.Patches;
using GoAwayMonkeys.Utilities;
using GorillaLocomotion;
using Liv.Lck.DependencyInjection;
using Liv.Lck.GorillaTag;
using System;
using TMPro;
using UnityEngine;

namespace GoAwayMonkeys;

[BepInPlugin(Constants.Guid, Constants.Name, Constants.Version)]
public class Plugin : BaseUnityPlugin
{
    internal static GorillaLog Log = new();

    public static Plugin Instance;

    private GameObject livCamera;

    public bool focusMode;

    private void Start()
    {
        Instance = this;
        HarmonyPatches.Patch();
        //GorillaTagger.OnPlayerSpawned(() => MethodUtilities.Attempt(OnPlayerSpawned));
    }

    private void Update()
    {
        if (focusMode)
        {
            GunLib.MakeGun(Color.red, new Vector3(0.15f, 0.15f, 0.15f), 0.025f, PrimitiveType.Sphere, GTPlayer.Instance.RightHand.controllerTransform, true, delegate
            {
                VRRig? hitRig = GunLib.GunHitRig();

                if (hitRig == null)
                    return;

                FocusOnRig(hitRig);
            }, delegate { });
        }
    }

    private void ToggleFocusMode()
    {
        focusMode = !focusMode;
        if (!focusMode)
        {
            UnhideAllRigs();
        }
    }

    private void HideVRRig(VRRig rig, bool hidden)
    {
        rig.transform.Find("gorilla_new").gameObject.SetActive(!hidden);
        rig.transform.Find("rig").gameObject.SetActive(!hidden);
    }

    private void FocusOnRig(VRRig focusRig)
    {
        foreach (VRRig rig in VRRigCache.AllRigs)
        {
            if (rig == focusRig || rig.isLocal)
            {
                HideVRRig(rig, false);
                continue;
            }

            HideVRRig(rig, true);
        }
    }

    private void UnhideAllRigs()
    {
        foreach (VRRig rig in VRRigCache.AllRigs)
        {
            HideVRRig(rig, false);
        }
    }

    public void SetupLCKCamera()
    {
        try
        {
            livCamera = GameObjectUtils.GetObject("LCKTablet(Clone)");

            GameObject focusButton = Instantiate(livCamera.transform.Find("UI").Find("Bar UI").Find("UI").Find("Record").gameObject, livCamera.transform.Find("UI").Find("Bar UI").Find("UI"), false);
            focusButton.name = "Focus";
            focusButton.transform.localRotation = Quaternion.identity;
            focusButton.transform.localPosition += new Vector3(0.5f, 0f, 0f);

            Destroy(focusButton.transform.Find("Triggers").gameObject);

            GameObject focusButtonCollider = Instantiate(focusButton.transform.Find("Visuals").Find("Body").gameObject, focusButton.transform, false);
            focusButtonCollider.name = "FocusButtonCollider";
            focusButtonCollider.GetComponent<Renderer>().enabled = false;
            focusButtonCollider.AddComponent<BoxCollider>().isTrigger = true;

            PressableButton focusPressable = focusButtonCollider.AddComponent<PressableButton>();
            focusPressable.onPress += ToggleFocusMode;
            focusPressable.visual = focusButton.transform.Find("Visuals").Find("Body").GetComponent<Renderer>();

            Destroy(focusButton.GetComponent<GtRecordButton>());
            Destroy(focusButton.GetComponent<LckDependencyResolver>());

            GameObject buttonText = focusButton.transform.Find("Visuals").Find("Value").gameObject;
            if (buttonText.activeSelf)
            {
                buttonText.GetComponent<TextMeshPro>().text = "FOCUS";
            }
            else
            {
                DestroyImmediate(buttonText);
                focusButton.transform.Find("Visuals").Find("Value").GetComponent<TextMeshPro>().text = "FOCUS";
            }
        }
        catch (Exception e)
        {
            Log.WriteException(e);
        }
    }
}
