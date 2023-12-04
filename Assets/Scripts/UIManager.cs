using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    
    //private Image levelCompletePanel;
    //private TextMeshProUGUI levelCompleteText;
    //private Image nextLevelButton;
    //private TextMeshProUGUI nextLevelText;

    //private float lcpTargetAlpha = 0.39f;
    //private float lctTargetAlpha = 1f;
    //private float nlbTargetAlpha = 0.35f;
    //private float nltTargetAlpha = 1f;

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

        //levelCompletePanel = levelCompleteUI.GetComponent<Image>();
        //levelCompletePanel.color = new Color(levelCompletePanel.color.r, levelCompletePanel.color.g, levelCompletePanel.color.b, 0);

        //levelCompleteText = levelCompleteUI.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //levelCompleteText.color = new Color(levelCompletePanel.color.r, levelCompletePanel.color.g, levelCompletePanel.color.b, 0);

        //nextLevelButton = levelCompleteUI.gameObject.transform.GetChild(1).GetComponent<Image>();
        //nextLevelButton.color = new Color(levelCompletePanel.color.r, levelCompletePanel.color.g, levelCompletePanel.color.b, 0);

        //nextLevelText = levelCompleteUI.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        //nextLevelText.color = new Color(levelCompletePanel.color.r, levelCompletePanel.color.g, levelCompletePanel.color.b, 0);
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

        // Return key shortcuts
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

        //if (levelCompleteUI.activeInHierarchy)
        //{

        //}
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

    //private void FadeInAlpha(float targetAlpha, float currentAlpha)
    //{
    //    float fadeDuration = 2f;


    //}

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
