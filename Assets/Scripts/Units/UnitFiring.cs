using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;

    [ServerCallback]
    private void Update()
    {
        Targetable targetable = targeter.GetTarget();

        if (targetable == null ) { return; }

        if (!CanFireAtTarget(targetable)) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(
            targetable.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(
                targetable.GetAimAtPoint().position - projectileSpawnPoint.transform.position);

            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }


    private bool CanFireAtTarget(Targetable targetable)
    {
        return (targetable.transform.position - transform.position).sqrMagnitude
            <= fireRange * fireRange;
    }
}
