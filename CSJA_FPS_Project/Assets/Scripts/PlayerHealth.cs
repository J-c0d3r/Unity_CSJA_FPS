using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;

    public Image bloodImage;
    private Color alphaAmount;


    public int recoveryFactor;
    public float recoveryRate;
    private float recoveryTimer;

    public bool isDead;

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        recoveryTimer += Time.deltaTime;

        if (recoveryTimer > recoveryRate)
        {
            StartCoroutine(RecoveryHealth());
            //MethodRecoveryHealth();
        }
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;

        alphaAmount = bloodImage.color;
        alphaAmount.a += (damage / 100f);

        bloodImage.color = alphaAmount;

        if (health <= 0)
        {
            isDead = true;
            GameController.instance.ShowGameOver();
            Debug.Log("gameOver");
        }

        recoveryTimer = 0f;
    }

    IEnumerator RecoveryHealth()
    {
        while (health < maxHealth)
        {
            health += recoveryFactor;

            alphaAmount.a -= (recoveryFactor / 100f);
            bloodImage.color = alphaAmount;
            yield return new WaitForSeconds(2f);
        }
    }

    //private void MethodRecoveryHealth()
    //{
    //    while (health < maxHealth)
    //    {
    //        health += recoveryFactor;

    //        alphaAmount.a -= (recoveryFactor / 100f);
    //        bloodImage.color = alphaAmount;
    //        //yield return new WaitForSeconds(2f);
    //    }
    //}
}
