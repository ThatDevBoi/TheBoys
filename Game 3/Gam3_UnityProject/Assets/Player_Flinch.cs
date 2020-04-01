using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Flinch : MonoBehaviour
{
    public float power = 0.7f;
    public float duration = 1f;
    [SerializeField]
    private Transform CameraTransform;
    public Transform Player;
    public float SlowDownAmount = 1f;
    public bool ShouldShake = false;
    Vector3 StartPosition;
    float InitialDuration;
    void Start()
    {
        InitialDuration = duration;
    }
    void Update()
    {
        CameraTransform = Player.transform;
        StartPosition = CameraTransform.localPosition;

        if (ShouldShake)
        {
            if (duration > 0)
            {
                CameraTransform.localPosition = StartPosition + Random.insideUnitSphere * power / 2;
                duration -= Time.deltaTime * SlowDownAmount;
            }
            else
            {
                ShouldShake = false;
                duration = InitialDuration;
                CameraTransform.localPosition = StartPosition;
            }
        }
    }
}
