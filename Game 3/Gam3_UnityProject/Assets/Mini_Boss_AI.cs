using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_Boss_AI : MonoBehaviour
{
    [LabelArray(new string[] { "Dipo"})]
    public bool[] whatBoss_;
    #region Engineer Mini boss
    // Current state of Mini Boss Engineer
    public engineerStates states = engineerStates.DORMANT;
    // Types of states Mini Boss Can Have
    public enum engineerStates {DORMANT, CHASING, FAZING};
    #endregion

    [Header("Variables Dipo")]
    public int damage;
    public int currentHealth;
    private int maxHealth = 100;

    private Vector3 startPosition;

    // Shooting
    public LayerMask whatToShoot;
    public float weaponRange;
    public float fireRate;
    public float fireRateTimer;
    public Transform target;
    private RaycastHit hit;
    public GameObject shootingPoint;

    // Chasing
    public float speed;
    public bool inRange = false;
    public float chasingDistance = 12;

    // Fazing 
    public Transform[] FazePositions;   // list of possible faze positions
    public float MaxTimerFaze = 3;  // Max amount of time to wait until Faze decide stops
    public int ifFaze = 0;     // int that decides if we faze
    public float nextFazeMove;
    public float nextTimeToFaze = 30f;

    private int currentFazeArray;   // the array value that got chose to faze to
    private Transform currentPosition;  // the current position the object has fazed to
    private float decideTimer = 0;


    //[Header("Nadeem")]

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PC").GetComponent<Transform>();
        shootingPoint = gameObject.transform.GetChild(0).gameObject;
        // Set the health
        currentHealth = maxHealth;
        //
        states = engineerStates.DORMANT;
        //
        startPosition = transform.position;
        //
        currentHealth = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (whatBoss_[0] == true)
        {
            // Functions
            EnumMonitor();
            movement();
            Shoot();
            if (fireRateTimer < fireRate)
                fireRateTimer += Time.deltaTime;
        }

        // Death
        if (currentHealth <= 0)
            Destroy(gameObject);
        // Make sure there is a balanced combat 
        if (inRange)
            states = engineerStates.CHASING;
    }

    void movement()
    {
        // Chasing the player
        if (states == engineerStates.CHASING)
        {
            speed = 2;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else if(states == engineerStates.FAZING && states != engineerStates.DORMANT)    // but if the AI wants to faze in and out of positions
        {
            nextFazeMove -= Time.deltaTime;
            if(nextFazeMove <= 0)
            {
                int i = Random.Range(0, 10);
                Debug.Log(i);
                if(i <= 8)
                {
                    // reset timer
                    nextFazeMove = nextTimeToFaze;
                    currentFazeArray = Random.Range(0, 3);
                    currentPosition = FazePositions[currentFazeArray];
                    transform.position = new Vector3(currentPosition.position.x, currentPosition.position.y + 3, currentPosition.position.z);
                }
                else
                {
                    ifFaze = 0;
                    i = 0;
                    nextFazeMove = nextTimeToFaze;
                    transform.position = startPosition;
                    states = engineerStates.DORMANT;
                }

            }
        }
    }

    void Shoot()
    {
        if (fireRateTimer < fireRate) return;
        fireRateTimer = 0;  // reset next time to shoot
        if (Physics.Raycast(shootingPoint.transform.position, (target.position - shootingPoint.transform.position), out hit, weaponRange, whatToShoot))
        {
            // are we shooting the player?
            if(hit.transform.gameObject.layer == 10)
            {
                Debug.Log("Hit player" + hit.transform.gameObject.name);
                Player_Controller hitDamage = target.gameObject.GetComponent<Player_Controller>();
                hitDamage.HitDetection(gameObject.GetComponent<Transform>());
                hitDamage.ApplyDamage(damage);
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            }
            Debug.DrawRay(transform.position, (target.position - shootingPoint.transform.position) * hit.distance, Color.blue);
        }
    }

    /// <summary>
    /// Subtract value from current health
    /// </summary>
    public void HealthMonitor(int damage)
    {
        currentHealth -= damage;
    }
    /// <summary>
    /// Controls what is going on 
    /// </summary>
    

    public void EnumMonitor()
    {
        // When we are dormant 
        if(states == engineerStates.DORMANT && transform.position == startPosition)
        {
            speed = 0;  // we dont move
        }
        
        if(states == engineerStates.DORMANT && inRange)
        {
            states = engineerStates.CHASING;
        }
        else if(states == engineerStates.CHASING && states != engineerStates.FAZING && !inRange)
        {
            // increase value
            decideTimer += Time.deltaTime;
            if (decideTimer >= MaxTimerFaze && ifFaze < 35)
            {
                ifFaze = Random.Range(10, 50);
                decideTimer = 0;
            }

            if (ifFaze > 35 && !inRange)
                states = engineerStates.FAZING;
        }

        // Chasing state change
        if(Vector3.Distance(transform.position, target.position) <= chasingDistance)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }
    }
}
