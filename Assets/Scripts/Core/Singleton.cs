using UnityEngine;

public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;

        Destroy(gameObject);
    }
}

/// <summary>
/// 싱글톤 패턴을 구현한 제네릭 클래스
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);

            return;
        }
        base.Awake();
    }
}

/// <summary>
/// Finally, we have the persistent version of the singleton. This will survive through scene loads.
/// Perfect for system classes which require stateful, persistent data. Or audio sources where music
/// plays through loading scenes, etc.
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        // If this was a duplicate, base.Awake() scheduled it for destruction; skip further work.
        if (Instance != this) return;

        // Only apply DontDestroyOnLoad to root GameObjects. If not root, skip.
        if (transform.parent != null) return;

        // If already in the DontDestroyOnLoad scene (e.g., due to hot-reload), avoid double registration.
        var currentScene = gameObject.scene;
        if (currentScene.IsValid() && currentScene.name == "DontDestroyOnLoad") return;

        DontDestroyOnLoad(gameObject);
    }
}