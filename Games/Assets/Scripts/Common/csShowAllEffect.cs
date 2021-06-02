using UnityEngine;
using System.Collections;

namespace Effects
{
    public class csShowAllEffect : MonoBehaviour
    {
        public string[] EffectNames;
        public string[] Effect2Names;
        public Transform[] Effect;

        //public GameObject mCurrentPS = null;

        public IEnumerator Coroutine_ShowEFX(int i, Vector3 position, float duration = 1.0f)
        {
            Transform obj = Instantiate(Effect[i], position, Quaternion.identity);
            yield return new WaitForSeconds(duration);
            Destroy(obj.gameObject);
        }

        public void ShowEFX(int i, Vector3 position, float duration = 0.2f)
        {
            StartCoroutine(Coroutine_ShowEFX(i, position, duration));
        }
    }
}