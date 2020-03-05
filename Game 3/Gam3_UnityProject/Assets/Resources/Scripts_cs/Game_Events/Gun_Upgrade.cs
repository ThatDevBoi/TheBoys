using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Upgrade : MonoBehaviour
{
    private GameManager gameManager;
    private BoxCollider objectTrigger;
    private Vector3 triggerSize = new Vector3(3, 1, 3);
    [Header("PICK ONE UPGRADE PER PREFAB!")]
    public bool is_This_Explosive_Upgrade;
    [Space(10)]
    public bool is_This_FullAuto_Upgrade;
    [Space(10)]
    public bool is_This_BurstFire_Upgrade;
    [Header("Make Pressing C optional")]
    public bool need_To_Press_C = false;
    private bool nearObject = false;
    

    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        // Find the GameManager in the scene
        gameManager = GameManager.FindObjectOfType<GameManager>();
        // find the player
        player = GameObject.Find("PC").GetComponent<Transform>();
        // Box collider set up
        objectTrigger = gameObject.AddComponent<BoxCollider>();
        objectTrigger.isTrigger = true;
        objectTrigger.size = triggerSize;

        #region Check names
        // change names 
        if (is_This_Explosive_Upgrade)
            gameObject.name = gameManager.explosiveUpgradeName;
        else if (is_This_FullAuto_Upgrade)
            gameObject.name = gameManager.fullAutoUpgradeName;
        else if (is_This_BurstFire_Upgrade)
            gameObject.name = gameManager.burstFireUpgradeName;
        #endregion

        #region Stop game error check
        // bool check 
        if (is_This_Explosive_Upgrade && is_This_FullAuto_Upgrade && is_This_BurstFire_Upgrade)
        {
            Debug.LogError("Dont set all the bools as true we only want the player to obtain one upgrade an object");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else if (!is_This_Explosive_Upgrade && is_This_FullAuto_Upgrade && is_This_BurstFire_Upgrade)
        {
            Debug.LogError("Two bools are true" + is_This_FullAuto_Upgrade + is_This_BurstFire_Upgrade +
                    "Pick one not both");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else if (is_This_Explosive_Upgrade && !is_This_FullAuto_Upgrade && is_This_BurstFire_Upgrade)
        {
            Debug.LogError("Two Bools are true" + is_This_Explosive_Upgrade + is_This_BurstFire_Upgrade +
                "Pick one not both");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else if (is_This_Explosive_Upgrade && is_This_FullAuto_Upgrade && !is_This_BurstFire_Upgrade)
        {
            Debug.LogError("Two Bools are true" + is_This_Explosive_Upgrade + is_This_FullAuto_Upgrade +
                "Pick one not both");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        Monitor();
    }

    void Monitor()
    {
        if (Vector3.Distance(transform.position, player.position) <= 2)
        {
            if (is_This_Explosive_Upgrade)
            {
                if (!need_To_Press_C && nearObject)
                {
                    gameManager.explosiveAmmo = true;
                    Destroy(this.gameObject);
                }
                else
                {
                    Debug.Log("Press C: ");
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        gameManager.explosiveAmmo = true;
                        Destroy(this.gameObject);
                    }
                }
            }
            else if (is_This_FullAuto_Upgrade)
            {
                if (!need_To_Press_C && nearObject)
                {
                    gameManager.fullAuto = true;
                    Destroy(this.gameObject);
                }
                else
                {
                    Debug.Log("Press C: ");
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        gameManager.fullAuto = true;
                        Destroy(this.gameObject);
                    }
                }
            }
            else if (is_This_BurstFire_Upgrade)
            {
                if (!need_To_Press_C && nearObject)
                {
                    gameManager.burstFire = true;
                    Destroy(this.gameObject);
                }
                else
                {
                    Debug.Log("Press C: ");
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        gameManager.burstFire = true;
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PC")
            nearObject = true;
    }
}
