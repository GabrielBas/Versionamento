using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    public static event Action OnBackgroundChanged;

    public static void ChangeBackground()
    {
        OnBackgroundChanged?.Invoke();
    }
}
