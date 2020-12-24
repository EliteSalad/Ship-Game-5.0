using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileCannon : MonoBehaviour {

    public Healthbar reloadBar;
    public GameObject reloadPrefab = null;
    public float fireRate = 2.0f;
    public float fireRateCounter = 2.0f;
    public float movementSpeed = 1.0f ;
    public float fireVelocity = 10;
    public bool reloading = false;
    [SerializeField] private bool cannonActive = true;
	[SerializeField] private GameObject projectile;
    [SerializeField] private Vector2 minMaxAngle;
    [SerializeField] private Transform cannonTurnBase;
    [SerializeField] private Transform cannonBarrel;
    [SerializeField] private ParticleSystem cannonSmoke;
    [SerializeField] private bool arcFire = false;

    [SerializeField] private Transform targetObject;

    float distanceToTarget;

    void Awake()
    {
        fireRateCounter = fireRate;
        reloadBar.maximumHealth = fireRate;
        //reloadBar.healthbarDisplay
    }
    // Update is called once per frame
    void Update ()
    {

        DrawBebugLines();


        if (reloading){
            fireRateCounter -= Time.deltaTime;
            //reloadBar.health += Time.deltaTime;
            reloadBar.GainHealth(Time.deltaTime);
            //reloadBar.UpdateHealth();
            if (fireRateCounter <= 0)
                reloading = false;
        }

        if (cannonActive){
        TurnCannonBase();
        TurnBarrel();
        }

    }

    public void DrawBebugLines()
    {
        Debug.DrawRay(cannonBarrel.position, cannonBarrel.forward * fireVelocity, Color.white);
        Vector3 xComponenet = cannonBarrel.forward * fireVelocity;
        Vector3 yComponenet = cannonBarrel.forward * fireVelocity;
        xComponenet.y = 0;
        yComponenet.x = 0;
        yComponenet.z = 0;

        Debug.DrawRay(cannonBarrel.position, xComponenet, Color.red);
        Debug.DrawRay(cannonBarrel.position, yComponenet, Color.green);
    }

    public void TurnCannonBase()
    {
        Vector3 baseTurnDir = targetObject.position - cannonTurnBase.position;
        baseTurnDir.y = 0;
        cannonTurnBase.forward = Vector3.Lerp(cannonTurnBase.forward, baseTurnDir, Time.deltaTime);
    }

    public void ChangeFireMode()
    {
        arcFire = !arcFire;
    }

    private float z;
    public void TurnBarrel()
    {
         Vector3 H_levelPos = targetObject.position;
        H_levelPos.y = cannonBarrel.position.y;

        float Rx = Vector3.Distance(H_levelPos, cannonBarrel.position);

        float k = 0.5f * Physics.gravity.y * Rx * Rx * (1 / (fireVelocity * fireVelocity));

        float h = targetObject.position.y - cannonBarrel.position.y;

        float j = (Rx * Rx) - (4 * (k * (k - h)));

        if(j >= 0)
        {
            if(arcFire) z = (-Rx - Mathf.Sqrt(j)) / (2 * k);

            else z = (-Rx + Mathf.Sqrt(j)) / (2 * k);
        } 
        cannonBarrel.localEulerAngles = new Vector3(Mathf.LerpAngle(cannonBarrel.localEulerAngles.x,  Mathf.Clamp((Mathf.Atan(z) * 57.2958f), minMaxAngle.x, minMaxAngle.y) * -1, Time.deltaTime * 10), 0, 0);
    }

    public void FireCannon()
    {
        if (!reloading)
        {
            cannonSmoke.Play();
            GameObject fireObj = Instantiate(projectile, cannonBarrel.position, cannonBarrel.rotation);
            Rigidbody rb = fireObj.GetComponent<Rigidbody>();
            rb.velocity = cannonBarrel.forward * fireVelocity;
            Destroy(fireObj, 10);
            reloading = true;
            fireRateCounter = fireRate;
            reloadBar.health = 0;
            reloadBar.TakeDamage(2);

            if (reloadPrefab != null)
            {
                GameObject m_reloadUi = null;
                m_reloadUi = Instantiate(reloadPrefab, transform.position, Quaternion.identity, transform);
                Destroy(m_reloadUi, fireRate);
            }
        }
        else
        {
            
            return;
        } 
    }

}
