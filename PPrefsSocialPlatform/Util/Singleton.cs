using UnityEngine;
using System.Collections;



public class Singleton<T> where T : class, new()
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }


    public void Idle()
    {
    }
}

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : class
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        Debug.Log(GetType().Name+".Awake");
        if (null != instance && (this as T) != instance)
        {
            Debug.LogWarning("Destory for SingletonMonoBehaviour");
            GameObject.Destroy(this.gameObject);
            return;
        }
        instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    public void Idle()
    {
    }

    public void Test()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(T)) as T;
            if (instance == null)
            {
                string name = typeof(T).FullName;
                GameObject obj = new GameObject(name);
                instance = obj.AddComponent(name) as T;

            }
        }
    }

}
