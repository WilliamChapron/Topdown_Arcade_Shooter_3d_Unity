using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using static Power;
using System.Drawing;

public class Shooting : MonoBehaviour
{
    public Slider slider;
    private float shootcurrentTime;
    private float startTime;
    public bool haveshoot;
    public float TimeToCharge;

    public Transform playerTransform;
    public bool canShoot;

    public float _maxLaserRange = 100f;
    public float laserWidth = 0.1f;
    public Material laserMaterial;
    protected LineRenderer lineRenderer;

    public List<ShootingPower> shootingPowers;

    public LayerMask layerCollide;

    public void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.material = laserMaterial;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        // Shooting Components
        shootingPowers = new List<ShootingPower>();

        //ShootingPush pushShooting = gameObject.AddComponent<ShootingPush>();
        //shootingPowers.Add(pushShooting);

        ShootingExplosion explosionShooting = gameObject.AddComponent<ShootingExplosion>();
        shootingPowers.Add(explosionShooting);

        haveshoot = true;
        slider.value = 1;
        //slider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(233f / 255f, 79f / 255f, 55f / 255f);
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot && haveshoot)
        {
            FireLaser();
            haveshoot = false;
            startTime = Time.time;  
        }
        else if (!haveshoot) 
        {
            reload();
        }
    }

    private void PerformRaycast(RaycastHit hit, Vector3 startPoint, Vector3 endPoint)
    {
        //Debug.Log("RAYCAST !!!!" + hit.collider.gameObject.name);

        endPoint = hit.point;

        Collider collider = hit.collider;


        if (collider != null)
        {
            //Debug.Log("Objet touch� : " + collider.gameObject.name);
            shootingPowers[0].OnCollision(hit.collider);
        }

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.enabled = true;

        Invoke("DisableLaser", 0.1f);


        shootingPowers[0].PerformExplosion(endPoint);

    }


    public void PerformNoRaycast(RaycastHit hit, Vector3 startPoint, Vector3 endPoint)
    {
        //Debug.Log("NO RAYCAST !!!!");
        endPoint = startPoint + transform.forward * _maxLaserRange;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.enabled = true;

        Invoke("DisableLaser", 0.1f);


        //Debug.Log("We touch nothing but we perform Explode");
        shootingPowers[0].PerformExplosion(endPoint);
    }

    protected void FireLaser()
    {
        RaycastHit hit;
        Vector3 endPoint = Vector3.zero;

        Vector3 startPoint = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);

        bool isRaycast = Physics.Raycast(startPoint, transform.forward, out hit, _maxLaserRange, layerCollide);

        if (isRaycast)
        {
            PerformRaycast(hit, startPoint, endPoint);
        }
        else
        {
            PerformNoRaycast(hit, startPoint, endPoint);
        }


    }

    protected void DisableLaser()
    {
        lineRenderer.enabled = false;
    }

    private void reload()
    {
        if (shootcurrentTime < TimeToCharge)
        {
            shootcurrentTime = Time.time - startTime;
            slider.value = (shootcurrentTime * 100f / TimeToCharge) / 100f;
        }
        else
        {
            slider.value = 1;
            shootcurrentTime = 0.0f;
            haveshoot = true;
        }
    }
}

