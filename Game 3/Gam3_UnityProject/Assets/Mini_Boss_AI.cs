using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_Boss_AI : MonoBehaviour
{
    [LabelArray(new string[] { "Dipo", "Katie", "Nadeem"})]
    public bool[] whatBoss_;

    [Header("Variables Dipo")]
    public float speed;
    public int damage;
    public int currentHealth;
    private int maxHealth = 100;

    private Vector3 moveDirection;
    private Quaternion currentRotation;
    // Shooting
    public float ammo;
    public LayerMask whatToShoot;
    public float weaponRange;
    public float fireRate;
    public float fireRateTimer;
    [SerializeField]
    private Transform target;
    private RaycastHit hit;




    //[Header("Katie")]


    //[Header("Nadeem")]

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PC").GetComponent<Transform>();
        // Set the health
        currentHealth = maxHealth;
        //
        
        //

        //

    }

    // Update is called once per frame
    void Update()
    {
        // Fire Rate for gun
        if (fireRateTimer < fireRate)
            fireRateTimer += Time.deltaTime;

        if (whatBoss_[0] == true)
        {
            // Functions
            movement();
            Shoot();
        }
    }

    void movement()
    {
        if(Vector3.Distance(transform.position, target.position) >= 18)
        {
            speed = 2;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else if(Vector3.Distance(transform.position, target.position) <= 3)
        {
            speed = 0;
        }
    }

    void Shoot()
    {
        if (fireRateTimer < fireRate) return;
        fireRateTimer = 0;
        if (Physics.Raycast(transform.position, transform.TransformDirection(transform.forward), out hit, weaponRange, whatToShoot))
        {
            Player_Controller hitDamage = target.gameObject.GetComponent<Player_Controller>();
            hitDamage.HitDetection(gameObject.GetComponent<Transform>());
            hitDamage.ApplyDamage(damage);
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.blue);
        }
    }

    void Health()
    {

    }
}
