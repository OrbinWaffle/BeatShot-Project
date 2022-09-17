using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotatorSpeed = 0;
    [SerializeField] private float recoil = 1;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject shotParticle;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private Transform rotator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private UIManager UIM;
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
        RB.AddForce(-rotator.forward * recoil, ForceMode.Impulse);
        GameObject proj = Instantiate(projectile, firePoint.position, rotator.rotation);
        RhythmManager.mainRM.AddSyncable(proj.GetComponent<ProjectileController>());
        RhythmManager.mainRM.AddSyncable(proj.GetComponent<SyncedAnimation>());
        proj.GetComponent<ProjectileController>().OnSync();
        proj.GetComponent<SyncedAnimation>().OnSync();
        Instantiate(shotParticle, firePoint.position, rotator.rotation);
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.collider.tag == "Projectile")
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
