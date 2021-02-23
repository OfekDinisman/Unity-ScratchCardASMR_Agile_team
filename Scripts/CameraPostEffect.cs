using UnityEngine;

public class CameraPostEffect : MonoBehaviour
{
    public delegate void postEffect();
    public event postEffect OnPostEffect;

    private void OnPostRender()
    {
        if (OnPostEffect != null)
        {
            OnPostEffect.Invoke();
        }
    }

}
