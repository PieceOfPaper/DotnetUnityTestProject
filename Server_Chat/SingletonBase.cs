using System.Collections;
using System.Collections.Generic;

public abstract class SingletonTemplate<T> where T : class, new()
{
    static T m_Instance = null;
    public static T Instance
    {
        get
        {
            if (m_Instance == null) m_Instance = new T();
            return m_Instance;
        }
    }

    public static void ResetInstance() { m_Instance = null; }
}