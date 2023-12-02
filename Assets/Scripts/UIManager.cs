using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Swap UI variables
    public GameObject swapCooldownUI;
    public GameObject noSwapUI;
    private Image cooldownImage;

    private float cooldown;
    private float cooldownTimer;
    private bool noSwapping = true;

    // Replay UI variables
    public GameObject replayUI;

    // Level Complete UI variables
    public GameObject levelCompleteUI;
    public string nextSceneName;

    public bool finalLevel = false;
    public GameObject playtestCompleteUI;

    // Start is called before the first frame update
    void Start()
    {
        cooldownImage = swapCooldownUI.GetComponent<Image>();

        if (!finalLevel)
        {
            playtestCompleteUI = null;
        }
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (replayUI.activeInHierarchy)
            {
                EnableLevelCompleteUI();
            }
            else if (levelCompleteUI.activeInHierarchy)
            {
                LoadNextLevel();
            }
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

    public void EnableReplayUI()
    {
        replayUI.SetActive(true);
    }

    public void EnableLevelCompleteUI()
    {
        if (replayUI.activeInHierarchy)
        {
            replayUI.SetActive(false);
            Debug.Log("Disabled Replay UI.");
        }

        levelCompleteUI.SetActive(true);
    }

    public void LoadNextLevel()
    {
        if (!finalLevel && nextSceneName != null)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else if (finalLevel)
        {
            levelCompleteUI.SetActive(false);
            playtestCompleteUI.SetActive(true);
        }
    }
}
