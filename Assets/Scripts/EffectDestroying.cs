using UnityEngine;

namespace DefaultNamespace
{
    public class EffectDestroying : MonoBehaviour
    {
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}