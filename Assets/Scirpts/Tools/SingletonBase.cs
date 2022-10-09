using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBase<T> where T:new()
{
    private static T instance;
    private static readonly object locker = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (locker)
                {
                    instance = new T();
                }
            }
            return instance;
        }
    }
}
