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
    private bool noSwapping = false;

    // Replay UI variables
    public GameObject replayUI;

    // Level Complete UI variables
    public GameObject levelCompleteUI;

    private List<Transform> levComColours = new List<Transform>();
    private Transform levComPanel;
    private Transform levComText;
    private Transform nextLevButton;
    private Transform nextLevText;
    private Transform repLevButton;
    private Transform repLevText;

    private List<float> levComAlphas = new List<float>();
    private float lcpTargetAlpha = 0.39f;
    private float lctTargetAlpha = 1f;
    private float nlbTargetAlpha = 0.35f;
    private float nltTargetAlpha = 1f;

    private float fadeInDuration = 1f;

    private bool onMainMenu = false;

    // Next level variables
    public string nextSceneName;
    public bool finalLevel = false;
    public GameObject gameCompleteUI;

    // Start is called before the first frame update
    void Start()
    {
        if (swapCooldownUI != null)
        {
            cooldownImage = swapCooldownUI.GetComponent<Image>();
        }

        if (!finalLevel)
        {
            gameCompleteUI = null;
        }

        if (levelCompleteUI != false)
        {
            levComPanel = levelCompleteUI.transform.GetChild(0);
            levComColours.Add(levComPanel);
            levComAlphas.Add(lcpTargetAlpha);

            levComText = levelCompleteUI.transform.GetChild(1);
            levComColours.Add(levComText);
            levComAlphas.Add(lctTargetAlpha);

            nextLevButton = levelCompleteUI.transform.GetChild(2);
            levComColours.Add(nextLevButton);
            levComAlphas.Add(nlbTargetAlpha);

            nextLevText = levelCompleteUI.transform.GetChild(2).GetChild(0);
            levComColours.Add(nextLevText);
            levComAlphas.Add(nltTargetAlpha);

            repLevButton = levelCompleteUI.transform.GetChild(3);
            levComColours.Add(repLevButton);
            levComAlphas.Add(nlbTargetAlpha);

            repLevText = levelCompleteUI.transform.GetChild(3).GetChild(0);
            levComColours.Add(repLevText);
            levComAlphas.Add(nltTargetAlpha);

            for (int i = 0; i < levComColours.Count; i++)
            {
                SetAlphaToZero(levComColours[i]);
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            onMainMenu = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (swapCooldownUI != null)
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

        // Return key shortcuts
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (replayUI != null && replayUI.activeInHierarchy)
            {
                EnableLevelCompleteUI();
            }
            else if (levelCompleteUI != null && levelCompleteUI.activeInHierarchy || onMainMenu)
            {
                LoadNextLevel();
            }
        }

        if ( levelCompleteUI != null && levelCompleteUI.activeInHierarchy)
        {
            for (int i = 0; i < levComColours.Count; i++)
            {
                FadeInAlpha(levComColours[i], levComAlphas[i]);
            }
        }
    }

    private void SetAlphaToZero(Transform transform)
    {
        Image image = transform.GetComponent<Image>();

        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        }
        else
        {
            TextMeshProUGUI text = transform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
            }
        }
    }

    private void FadeInAlpha(Transform transform, float targetAlpha)
    {
        Image image = transform.GetComponent<Image>();

        if (image != null)
        {
            image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, targetAlpha), Time.deltaTime / fadeInDuration);
        }
        else
        {
            TextMeshProUGUI text = transform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, targetAlpha), Time.deltaTime / fadeInDuration);
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
        }

        levelCompleteUI.SetActive(true);
    }

    public void LoadNextLevel()
    {
        if (!finalLevel)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else if (finalLevel)
        {
            levelCompleteUI.SetActive(false);
            gameCompleteUI.SetActive(true);
            finalLevel = false;
        }
    }

    public void OpenMainMenuPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void CloseMainMenuPanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
