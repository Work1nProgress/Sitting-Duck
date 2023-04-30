using System.Linq;
using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        /**
	      Returns the instance of this singleton.
	   */
        public static T Instance
        {
            get
            {
                if (instance != null && instance.gameObject is { activeSelf: true, activeInHierarchy: true })
                    return instance;
                
                //instance = FindObjectsOfType ( typeof ( T ) );

                var query = from c in FindObjectsOfType(typeof(T))
                    where ((MonoBehaviour)c).gameObject is { activeSelf: true, activeInHierarchy: true }
                    select c;

                instance = (T)query.FirstOrDefault();

                if (instance == null)
                {
                    Debug.LogWarning($"An instance of {typeof(T)} is needed in the scene, but there is none. Adding one now.");
                    instance = new GameObject(nameof(T)).AddComponent<T>();
                }

                return instance;
            }
        }
    }
}