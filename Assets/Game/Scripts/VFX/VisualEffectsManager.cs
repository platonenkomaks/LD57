using UnityEngine;

public class VisualEffectsManager : Singleton<VisualEffectsManager>
{
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject wallHitEffect;
    
   protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        G.VisualEffectsManager = this;
    }

    public void PlayEffect(string effectId, Vector2 position, Quaternion rotation)
    {
        switch (effectId)
        {
            
            case "MuzzleFlash":
            {
               
                var effect = Instantiate(muzzleFlash, position, rotation);
                Destroy(effect, 0.1f);
                break;
            }
           
            case "HitEffect":
            {
               
                var effect = Instantiate(hitEffect, position, rotation);
                Destroy(effect, 0.1f);
                break;
            }
            
            case "WallHitEffect":
            {
                
                var effect = Instantiate(wallHitEffect, position, rotation);
                Destroy(effect, 0.1f);
                break;
            }
        }
    }
}

