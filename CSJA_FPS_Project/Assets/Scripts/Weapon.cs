using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;

public class Weapon : MonoBehaviour
{
    [Header("Properties")]
    public float range = 100f;
    public int totalBullets = 30;
    public int bulletsLeft;
    public int currentBullet;

    public float fireRate = 0.1f;
    public float spreadFactor;
    private float fireTimer;

    [Header("Shoot Config")]
    public Transform shootPoint;
    public ParticleSystem fireEffect;

    public GameObject hitEffect;
    public GameObject bulletImpact;

    private Animator anim;

    private bool isReloading;
    [Header("Sounds")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    [Header("Aim")]
    public Vector3 aimPos;
    public float aimSpeed;
    private Vector3 originalPos;

    [Header("UI")]
    public Text ammoTxt;


    public enum ShootMode
    {
        Auto,
        Semi
    }
    public ShootMode shootMode;
    private bool shootInput;

    public int damage;




    private void OnEnable()
    {
        UpdateAmmotText();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentBullet = totalBullets;
        originalPos = transform.localPosition;

        UpdateAmmotText();
    }


    void Update()
    {
        //if (Input.GetButton("Fire1"))
        //{
        //    if (currentBullet > 0)
        //    {
        //        Fire();
        //    }
        //    else if (bulletsLeft > 0)
        //    {
        //        DoReload();
        //    }
        //}

        switch (shootMode)
        {
            case ShootMode.Auto:
                shootInput = Input.GetButton("Fire1");
                break;

            case ShootMode.Semi:
                shootInput = Input.GetButtonDown("Fire1");
                break;

            default:
                break;
        }

        if (shootInput)
        {
            if (currentBullet > 0)
            {
                Fire();
            }
            else if (bulletsLeft > 0)
            {
                DoReload();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentBullet < totalBullets && bulletsLeft > 0)
            {
                DoReload();
            }
        }

        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }

        ToAim();
    }

    private void Fire()
    {
        if (fireTimer < fireRate || isReloading || currentBullet <= 0)
            return;

        RaycastHit hit;

        //Better results
        Vector3 shootDirection = shootPoint.transform.forward;
        shootDirection = shootDirection + shootPoint.TransformDirection(new Vector3(Random.Range(-spreadFactor, spreadFactor), Random.Range(-spreadFactor, spreadFactor)));

        //Imprecise results
        //shootDirection.x += Random.Range(-spreadFactor, spreadFactor);
        //shootDirection.y += Random.Range(-spreadFactor, spreadFactor);

        //if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        if (Physics.Raycast(shootPoint.position, shootDirection, out hit, range))
        {
            GameObject hitParcticle = Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            GameObject bullet = Instantiate(bulletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            bullet.transform.SetParent(hit.transform);


            Destroy(hitParcticle, 1f);
            Destroy(bullet, 3f);

            if (hit.transform.GetComponent<ObjectHealth>())
            {
                hit.transform.GetComponent<ObjectHealth>().ApplyDamage(damage);
            }

            if (hit.transform.GetComponent<soldier>())
            {
                Destroy(bullet);
                hit.transform.GetComponent<soldier>().ApplyDamage(damage);
            }

        }

        anim.CrossFadeInFixedTime("Fire", 0.01f);
        fireEffect.Play();
        PlayShootSound();
        currentBullet--;
        fireTimer = 0f;
        UpdateAmmotText();
    }

    public void ToAim()
    {
        if (Input.GetButton("Fire2") && !isReloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPos, Time.deltaTime * aimSpeed);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * aimSpeed);
        }
    }

    private void FixedUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        isReloading = info.IsName("Reload");
    }

    void DoReload()
    {
        if (isReloading)
            return;
        anim.CrossFadeInFixedTime("Reload", 0.01f);
        //UpdateAmmotText();
    }

    public void Reload()
    {
        if (bulletsLeft <= 0)
        {
            return;
        }

        int bulletsToLoad = totalBullets - currentBullet;
        int bulletsToDeduct = (bulletsLeft >= bulletsToLoad) ? bulletsToLoad : bulletsLeft;

        bulletsLeft -= bulletsToDeduct;
        currentBullet += bulletsToDeduct;
        UpdateAmmotText();
    }

    void PlayShootSound()
    {
        audioSource.PlayOneShot(shootSound);
    }

    void UpdateAmmotText()
    {
        ammoTxt.text = currentBullet + " / " + bulletsLeft;
    }
}
