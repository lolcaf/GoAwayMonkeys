using System;
using UnityEngine;

namespace GoAwayMonkeys.MonoBehaviors;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class PressableButton : MonoBehaviour
{
    public Action onPress;

    public Renderer visual;

    private float cooldownTime;

    private Color originalColor;

    private void Start()
    {
        gameObject.layer = 18;
        originalColor = visual.material.color;
    }

    private void OnButtonPress()
    {
        onPress?.Invoke();
        VRRig.LocalRig.PlayHandTapLocal(67, false, 1f);
        if (Plugin.Instance.focusMode)
        {
            visual.material.color = Color.red;
        }
        else
        {
            visual.material.color = originalColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < cooldownTime) return;
        if (other.gameObject == GorillaTagger.Instance.rightHandTriggerCollider || other.gameObject == GorillaTagger.Instance.leftHandTriggerCollider)
        {
            cooldownTime = Time.time + 1f;
            OnButtonPress();
        }
    }
}
