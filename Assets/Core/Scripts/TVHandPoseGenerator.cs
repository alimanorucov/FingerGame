using UnityEngine;
using System.Collections;

public class TVHandsPoseGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Finger
    {
        public Transform[] joints;
        [HideInInspector] public Quaternion[] openRotations;
        [HideInInspector] public bool isClosed;
        [HideInInspector] public Quaternion[] currentRotations;
    }

    [System.Serializable]
    public class Hand
    {
        public Finger thumb;
        public Finger index;
        public Finger middle;
        public Finger ring;
        public Finger pinky;
    }

    public Hand leftHand;
    public Hand rightHand;
    public float closeAngle = -60f;

    [Header("Animation Settings")]
    public float transitionSpeed = 5f;

    [Header("TV Settings")]
    public string currentPoseName = "RANDOM";

    private bool[] currentPose;
    private bool[] targetPose;
    private bool isTransitioning = false;

    void Awake()
    {
        CacheOpen(leftHand);
        CacheOpen(rightHand);
        InitializeCurrentRotations();
    }

    void Start()
    {
        ShowRandomPose();
    }

    void InitializeCurrentRotations()
    {
        UpdateCurrentRotations(leftHand);
        UpdateCurrentRotations(rightHand);
    }

    void UpdateCurrentRotations(Hand hand)
    {
        UpdateFingerCurrentRotations(hand.thumb);
        UpdateFingerCurrentRotations(hand.index);
        UpdateFingerCurrentRotations(hand.middle);
        UpdateFingerCurrentRotations(hand.ring);
        UpdateFingerCurrentRotations(hand.pinky);
    }

    void UpdateFingerCurrentRotations(Finger finger)
    {
        if (finger.joints == null) return;

        if (finger.currentRotations == null || finger.currentRotations.Length != finger.joints.Length)
        {
            finger.currentRotations = new Quaternion[finger.joints.Length];
        }

        for (int i = 0; i < finger.joints.Length; i++)
        {
            if (finger.joints[i] != null)
            {
                finger.currentRotations[i] = finger.joints[i].localRotation;
            }
        }
    }

    [ContextMenu("Show Random Pose")]
    public void ShowRandomPose()
    {
        if (isTransitioning) return;

        targetPose = GenerateRandomPose();
        StartCoroutine(TransitionToPose(targetPose));
        currentPoseName = "RANDOM";
    }

    public bool[] GenerateRandomPose()
    {
        bool[] randomPose = new bool[10];

        for (int i = 0; i < 10; i++)
        {
            randomPose[i] = Random.Range(0, 2) == 0;
        }

        return randomPose;
    }

    IEnumerator TransitionToPose(bool[] targetPoseData)
    {
        isTransitioning = true;

        SaveCurrentPoseAsCurrentRotations();

        if (targetPoseData.Length >= 10)
        {
            leftHand.thumb.isClosed = targetPoseData[0];
            leftHand.index.isClosed = targetPoseData[1];
            leftHand.middle.isClosed = targetPoseData[2];
            leftHand.ring.isClosed = targetPoseData[3];
            leftHand.pinky.isClosed = targetPoseData[4];

            rightHand.thumb.isClosed = targetPoseData[5];
            rightHand.index.isClosed = targetPoseData[6];
            rightHand.middle.isClosed = targetPoseData[7];
            rightHand.ring.isClosed = targetPoseData[8];
            rightHand.pinky.isClosed = targetPoseData[9];
        }

        Quaternion[][][] targetRotations = CalculateTargetRotations();

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;

            AnimateFinger(leftHand.thumb, targetRotations[0][0], t);
            AnimateFinger(leftHand.index, targetRotations[0][1], t);
            AnimateFinger(leftHand.middle, targetRotations[0][2], t);
            AnimateFinger(leftHand.ring, targetRotations[0][3], t);
            AnimateFinger(leftHand.pinky, targetRotations[0][4], t);

            AnimateFinger(rightHand.thumb, targetRotations[1][0], t);
            AnimateFinger(rightHand.index, targetRotations[1][1], t);
            AnimateFinger(rightHand.middle, targetRotations[1][2], t);
            AnimateFinger(rightHand.ring, targetRotations[1][3], t);
            AnimateFinger(rightHand.pinky, targetRotations[1][4], t);

            yield return null;
        }

        ApplyHandImmediate(leftHand);
        ApplyHandImmediate(rightHand);

        UpdateCurrentRotations(leftHand);
        UpdateCurrentRotations(rightHand);

        currentPose = targetPoseData;
        isTransitioning = false;
    }

    void SaveCurrentPoseAsCurrentRotations()
    {
        UpdateCurrentRotations(leftHand);
        UpdateCurrentRotations(rightHand);
    }

    Quaternion[][][] CalculateTargetRotations()
    {
        Quaternion[][][] targetRotations = new Quaternion[2][][];

        targetRotations[0] = new Quaternion[5][];
        targetRotations[0][0] = CalculateFingerTargetRotations(leftHand.thumb);
        targetRotations[0][1] = CalculateFingerTargetRotations(leftHand.index);
        targetRotations[0][2] = CalculateFingerTargetRotations(leftHand.middle);
        targetRotations[0][3] = CalculateFingerTargetRotations(leftHand.ring);
        targetRotations[0][4] = CalculateFingerTargetRotations(leftHand.pinky);

        targetRotations[1] = new Quaternion[5][];
        targetRotations[1][0] = CalculateFingerTargetRotations(rightHand.thumb);
        targetRotations[1][1] = CalculateFingerTargetRotations(rightHand.index);
        targetRotations[1][2] = CalculateFingerTargetRotations(rightHand.middle);
        targetRotations[1][3] = CalculateFingerTargetRotations(rightHand.ring);
        targetRotations[1][4] = CalculateFingerTargetRotations(rightHand.pinky);

        return targetRotations;
    }

    Quaternion[] CalculateFingerTargetRotations(Finger finger)
    {
        if (finger.joints == null) return new Quaternion[0];

        Quaternion[] targetRotations = new Quaternion[finger.joints.Length];

        for (int i = 0; i < finger.joints.Length; i++)
        {
            if (finger.isClosed)
            {
                float multiplier = 1f + (i * 0.2f);
                targetRotations[i] = finger.openRotations[i] *
                    Quaternion.Euler(closeAngle * multiplier, 0f, 0f);
            }
            else
            {
                targetRotations[i] = finger.openRotations[i];
            }
        }

        return targetRotations;
    }

    void AnimateFinger(Finger finger, Quaternion[] targetRotations, float t)
    {
        if (finger.joints == null || finger.currentRotations == null) return;

        for (int i = 0; i < finger.joints.Length && i < finger.currentRotations.Length; i++)
        {
            if (finger.joints[i] != null && i < targetRotations.Length)
            {
                finger.joints[i].localRotation = Quaternion.Slerp(
                    finger.currentRotations[i],
                    targetRotations[i],
                    t
                );
            }
        }
    }

    void CacheOpen(Hand hand)
    {
        CacheFinger(hand.thumb);
        CacheFinger(hand.index);
        CacheFinger(hand.middle);
        CacheFinger(hand.ring);
        CacheFinger(hand.pinky);
    }

    void CacheFinger(Finger finger)
    {
        if (finger.joints == null) return;

        finger.openRotations = new Quaternion[finger.joints.Length];
        for (int i = 0; i < finger.joints.Length; i++)
            finger.openRotations[i] = finger.joints[i].localRotation;
    }

    void ApplyHandImmediate(Hand hand)
    {
        ApplyFingerImmediate(hand.thumb);
        ApplyFingerImmediate(hand.index);
        ApplyFingerImmediate(hand.middle);
        ApplyFingerImmediate(hand.ring);
        ApplyFingerImmediate(hand.pinky);
    }

    void ApplyFingerImmediate(Finger finger)
    {
        if (finger.joints == null) return;

        for (int i = 0; i < finger.joints.Length; i++)
        {
            if (finger.isClosed)
            {
                float multiplier = 1f + (i * 0.2f);
                finger.joints[i].localRotation = finger.openRotations[i] *
                    Quaternion.Euler(closeAngle * multiplier, 0f, 0f);
            }
            else
            {
                finger.joints[i].localRotation = finger.openRotations[i];
            }
        }
    }

    public bool[] GetCurrentPose() => currentPose;
    public string GetCurrentPoseName() => currentPoseName;

    public string GetPoseAsString()
    {
        if (currentPose == null) return "No Pose";

        string result = "Poz: ";
        for (int i = 0; i < currentPose.Length; i++)
        {
            result += currentPose[i] ? "1" : "0";
            if (i == 4) result += " | ";
        }
        return result;
    }
}