using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class HandPoseComparator : MonoBehaviour
{
    public FingerController fingerController;
    public TVHandsPoseGenerator tvPoseGenerator;

    [Header("Audio Settings")]
    public AudioSource winAudioSource;
    public AudioSource loseAudioSource;
    public AudioClip winSound;
    public AudioClip loseSound;

    [Header("Post Processing")]
    public Volume globalVolume;
    public float effectDuration = 1f;
    public Color winColor = new Color(0, 1, 0, 0.3f);
    public Color loseColor = new Color(1, 0, 0, 0.3f);

    [Header("UI Settings")]
    public TMPro.TextMeshProUGUI resultText;
    public TMPro.TextMeshProUGUI tvPoseText;

    private Color defaultColor = new Color(1, 1, 1, 0f);
    private bool isEffectPlaying = false;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        if (globalVolume != null && globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            colorAdjustments.colorFilter.value = defaultColor;
        }

        if (winAudioSource == null || loseAudioSource == null)
        {
            SetupAudioSources();
        }

        UpdateTVPoseText();
    }

    void SetupAudioSources()
    {
        if (winAudioSource == null)
        {
            GameObject winAudioObj = new GameObject("WinAudioSource");
            winAudioObj.transform.parent = transform;
            winAudioSource = winAudioObj.AddComponent<AudioSource>();
        }

        if (loseAudioSource == null)
        {
            GameObject loseAudioObj = new GameObject("LoseAudioSource");
            loseAudioObj.transform.parent = transform;
            loseAudioSource = loseAudioObj.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isEffectPlaying)
        {
            CompareHands();
        }

        UpdateTVPoseText();
    }

    void UpdateTVPoseText()
    {
        if (tvPoseText != null && tvPoseGenerator != null)
        {
            tvPoseText.text = tvPoseGenerator.GetPoseAsString();
        }
    }

    void CompareHands()
    {
        bool[] tvPose = tvPoseGenerator.GetCurrentPose();
        if (tvPose == null || tvPose.Length != 10)
        {
            return;
        }

        bool[] playerPose = GetPlayerHandState();
        bool isMatch = ArePosesEqual(tvPose, playerPose);

        if (isMatch)
        {
            StartCoroutine(PlayEffect(true));
            if (resultText != null)
                resultText.text = "DOÐRU!";
        }
        else
        {
            StartCoroutine(PlayEffect(false));
            if (resultText != null)
                resultText.text = "YANLIÞ!";
        }

        // Efekt bittikten sonra yeni poz göster
        StartCoroutine(ShowNewPoseAfterDelay(effectDuration));
    }

    bool[] GetPlayerHandState()
    {
        bool[] playerPose = new bool[10];

        playerPose[0] = fingerController.leftThumb.isClosed;
        playerPose[1] = fingerController.leftIndex.isClosed;
        playerPose[2] = fingerController.leftMiddle.isClosed;
        playerPose[3] = fingerController.leftRing.isClosed;
        playerPose[4] = fingerController.leftPinky.isClosed;

        playerPose[5] = fingerController.rightThumb.isClosed;
        playerPose[6] = fingerController.rightIndex.isClosed;
        playerPose[7] = fingerController.rightMiddle.isClosed;
        playerPose[8] = fingerController.rightRing.isClosed;
        playerPose[9] = fingerController.rightPinky.isClosed;

        return playerPose;
    }

    bool ArePosesEqual(bool[] pose1, bool[] pose2)
    {
        if (pose1.Length != pose2.Length || pose1.Length != 10) return false;
        for (int i = 0; i < pose1.Length; i++)
            if (pose1[i] != pose2[i]) return false;
        return true;
    }

    IEnumerator PlayEffect(bool isWin)
    {
        isEffectPlaying = true;

        if (isWin)
        {
            if (winAudioSource != null && winSound != null)
                winAudioSource.PlayOneShot(winSound);

            if (colorAdjustments != null)
                colorAdjustments.colorFilter.value = winColor;
        }
        else
        {
            if (loseAudioSource != null && loseSound != null)
                loseAudioSource.PlayOneShot(loseSound);

            if (colorAdjustments != null)
                colorAdjustments.colorFilter.value = loseColor;
        }

        yield return new WaitForSeconds(effectDuration);

        if (colorAdjustments != null)
            colorAdjustments.colorFilter.value = defaultColor;

        isEffectPlaying = false;
    }

    IEnumerator ShowNewPoseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Yeni rastgele poz göster (animasyonlu)
        tvPoseGenerator.ShowRandomPose();

        // UI temizle
        if (resultText != null)
            resultText.text = "";
    }
}