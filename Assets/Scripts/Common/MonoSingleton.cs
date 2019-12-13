using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    public static T Instance
    {
        get
        {

            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
            }
            
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            if (instance != this as T)
            {
                Destroy(gameObject);
                return;
            }
        }
        Init();
    }

    protected virtual void Init()
    {

    }
}

