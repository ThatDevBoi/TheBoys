using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Used for weapon sway so the weapon has a delayed follow time so it sways 
/// This needed to be seperate from the core gun logic as it interferes with the Aim Function
/// So a parent needs to have this script so the child sways (Child should be gun)
/// </summary>
public class Weapon_Sway : MonoBehaviour
{
    [Header("Weapon Sway")]
    // PUBLIC
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothTime = 6f;

    // PRIVATE
    Vector3 def;
    Vector3 euler;
    // Start is called before the first frame update
    void Start()
    {
        // Weapon Sway
        def = transform.localPosition;
        euler = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        WeaponSway();
    }

    float _smooth;
    public void WeaponSway()
    {
        _smooth = smoothTime;

        float MovementX = -Input.GetAxis("Mouse X") * amount;
        float MovementY = -Input.GetAxis("Mouse Y") * amount;

        MovementX = Mathf.Clamp(MovementX, -maxAmount, maxAmount);
        MovementY = Mathf.Clamp(MovementY, -maxAmount, maxAmount);

        Vector3 final = new Vector3(def.x + MovementX, def.y + MovementY, def.z);
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, final, Time.deltaTime * _smooth);
    }
}
