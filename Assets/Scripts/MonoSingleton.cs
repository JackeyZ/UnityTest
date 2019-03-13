using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : class {
    public static T Instance;
    void Awake()
    {
        Instance = this as T;
    }
}
