using UnityEngine;
using System.Collections.Generic;

public static class TVPoseLibrary
{

    public static bool[] GetRandomPose()
    {
        bool[] randomPose = new bool[10];

        for (int i = 0; i < 10; i++)
        {
            // Her parmak için %50 þansla kapalý, %50 þansla açýk
            randomPose[i] = Random.Range(0, 2) == 0;
        }

        return randomPose;
    }



}