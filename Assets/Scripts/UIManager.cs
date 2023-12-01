using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject swapCooldownUI;
    public GameObject noSwapUI;
    private Image cooldownImage;

    private float cooldown;
    private float cooldownTimer;
    private bool noSwapping = true;

    // Start is called before the first frame update
    void Start()
    {
        cooldownImage = swapCooldownUI.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.nextSwapAllowed > Time.time && !swapCooldownUI.activeInHierarchy && !noSwapping)
        {
            cooldown = PlayerManager.nextSwapAllowed - Time.time;
            cooldownTimer = cooldown;
            swapCooldownUI.SetActive(true);
        }

        if (swapCooldownUI.activeInHierarchy)
        {
            cooldownImage.fillAmount = (cooldownTimer -= Time.deltaTime) / cooldown;
        }

        if (PlayerManager.nextSwapAllowed <= Time.time && swapCooldownUI.activeInHierarchy)
        {
            swapCooldownUI.SetActive(false);
            cooldownImage.fillAmount = 1f;
        }
    }

    public void EnableNoSwapUI()
    {
        noSwapping = true;
        
        if (swapCooldownUI.activeInHierarchy)
        {
            swapCooldownUI.SetActive(false);
            cooldownImage.fillAmount = 1f;
        }

        noSwapUI.SetActive(true);
    }

    public void DisableNoSwapUI()
    {
        noSwapping = false;
        noSwapUI.SetActive(false);
    }
}
