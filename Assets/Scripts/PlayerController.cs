using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotatorSpeed = 0f;
    [SerializeField] private float recoil = 1f;
    [SerializeField] private float timeBetweenShots = 0f;
    [SerializeField] private int numOfShots = 1;
    [SerializeField] private float spreadOfShots = 90f;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject shotParticle;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private Transform rotator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private UIManager UIM;
    private float timeOfLastShot;
    Rigidbody RB;
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }
    public void Aim(Vector3 mousePos)
    {
        Vector3 aimPos = new Vector3(mousePos.x, transform.position.y, mousePos.z);
        Vector3 lookVec = (aimPos - rotator.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(lookVec, Vector3.up);
        rotator.rotation = Quaternion.Slerp(rotator.rotation, lookRot, rotatorSpeed * Time.deltaTime);
    }
    public void Attack()
    {
        if(Time.time < timeOfLastShot + timeBetweenShots){return;}
        RB.AddForce(-rotator.forward * recoil, ForceMode.Impulse);
        Instantiate(shotParticle, firePoint.position, rotator.rotation);
        for(int i = 0; i < numOfShots; i++)
        {
            float rotation = (-spreadOfShots/2) + (spreadOfShots/numOfShots) * (i + 1);
            if(numOfShots == 1)
            {
                rotation = 0;
            }
            GameObject proj = Instantiate(projectile, firePoint.position, rotator.rotation * Quaternion.Euler(0, rotation, 0));
            RhythmManager.mainRM.AddSyncable(proj.GetComponent<ProjectileController>());
            RhythmManager.mainRM.AddSyncable(proj.GetComponent<SyncedAnimation>());
            proj.GetComponent<ProjectileController>().OnSync();
            proj.GetComponent<SyncedAnimation>().OnSync();
        }
        timeOfLastShot = Time.time;
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.collider.tag == "Projectile" && !isInvincible)
        {
            UIM.PlayerDied();
            Instantiate(deathParticle, transform.position, Quaternion.identity);
            RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<PlayerInput>());
            RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<SyncedAnimation>());
            GetComponentInParent<PlayerInput>().isControlling = false;
            this.gameObject.SetActive(false);
        }
    }
}
