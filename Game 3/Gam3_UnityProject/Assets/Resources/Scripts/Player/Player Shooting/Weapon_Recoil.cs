using UnityEngine;

public class Weapon_Recoil : MonoBehaviour
{
    [Header("Recoil_Transform")]
    // parent of the childs parent
    public Transform RecoilPositionTranform;
    // parent of the child
    public Transform RecoilRotationTranform;
    [Space(10)]
    [Header("Recoil_Settings")]
    // how smooth does the position change with recoil
    public float PositionDampTime;
    // how smooth does rotation transitionwith recoil
    public float RotationDampTime;
    [Space(10)]
    // multipliers at different rates
    public float Recoil1;
    public float Recoil2;
    public float Recoil3;
    public float Recoil4;
    [Space(10)]
    public Vector3 RecoilRotation;
    public Vector3 RecoilKickBack;

    public Vector3 RecoilRotation_Aim;
    public Vector3 RecoilKickBack_Aim;
    [Space(10)]
    // current recoil of the gun 
    Vector3 CurrentRecoil1;
    Vector3 CurrentRecoil2;
    Vector3 CurrentRecoil3;
    Vector3 CurrentRecoil4;
    [Space(10)]
    Vector3 RotationOutput;

    public bool aim;
    public Shooting_Mechanic playerscript;
    /// <summary>
    /// We set values for recoil in inspector
    /// 
    /// </summary>
    void FixedUpdate()
    {
        // set up what the recoil will be for each stage. starting at 0 multiplying the recoil 
        CurrentRecoil1 = Vector3.Lerp(CurrentRecoil1, Vector3.zero, Recoil1 * Time.deltaTime);
        CurrentRecoil2 = Vector3.Lerp(CurrentRecoil2, CurrentRecoil1, Recoil2 * Time.deltaTime);
        CurrentRecoil3 = Vector3.Lerp(CurrentRecoil3, Vector3.zero, Recoil3 * Time.deltaTime);
        CurrentRecoil4 = Vector3.Lerp(CurrentRecoil4, CurrentRecoil3, Recoil4 * Time.deltaTime);
        // where will the gun end up with its recoil
        //RecoilPositionTranform.localPosition = Vector3.Slerp(RecoilPositionTranform.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
        // what rotatiom will the gun have with recoil
        RotationOutput = Vector3.Slerp(RotationOutput, CurrentRecoil1, RotationDampTime * Time.fixedDeltaTime);
        // rotate gun
        RecoilRotationTranform.localRotation = Quaternion.Euler(RotationOutput);
    }
    /// <summary>
    /// This function is for when the player is aiming in which this logic is called in the shooting mechanic class. 
    /// When aiming firing recoil is more accurate. 
    /// </summary>
    public virtual void Fire()
    {
        aim = playerscript.imAiming;
        if (aim == true)
        {
            // when aiming pick a random range for each axis that gets set in aim kickback and rotation
            CurrentRecoil1 += new Vector3(RecoilRotation_Aim.x, Random.Range(-RecoilRotation_Aim.y, RecoilRotation_Aim.y), Random.Range(-RecoilRotation_Aim.z, RecoilRotation_Aim.z));
            CurrentRecoil3 += new Vector3(Random.Range(-RecoilKickBack_Aim.x, RecoilKickBack_Aim.x), Random.Range(-RecoilKickBack_Aim.y, RecoilKickBack_Aim.y), RecoilKickBack_Aim.z);
        }
        if (aim == false)
        {
            CurrentRecoil1 += new Vector3(RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
            CurrentRecoil3 += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);
        }
    }
}
