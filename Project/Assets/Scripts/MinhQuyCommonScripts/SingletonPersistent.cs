using UnityEngine;

public class SingletonPersistent<T> : MonoBehaviour where T : Component
{
    protected virtual void Awake()
    {
        if (Instance == null)
            Instance = (this as T);
        else if (Instance != this)
        {
            Debug.LogError("Duplicate SingletonPersistent " + typeof(T) + " at go: " + name + ". Origin go: " + Instance.name);
            Destroy(gameObject);
        }

        if (transform.parent == null)
            DontDestroyOnLoad(gameObject);
    }

    public static T Instance { get; private set; } = null;
    public static bool IsInstanced { get { return Instance != null; } }
}