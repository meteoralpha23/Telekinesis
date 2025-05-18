using UnityEngine;
using System.Collections; // This line is important for IEnumerator

public class TelekinesisEffect : MonoBehaviour
{
    public ParticleSystem liftEffect;
    public ParticleSystem holdEffect;
    public ParticleSystem throwEffect;

    private ParticleSystem currentHoldEffect;

    private ParticleSystem activeLiftEffect; // Store reference to the active lift effect

    public void PlayLiftEffect(Vector3 position)
    {
        StartCoroutine(DelayedLiftEffect(position, 0.6f)); // Start with 0.6s delay
    }

    private IEnumerator DelayedLiftEffect(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for 0.6s

        if (liftEffect != null)
        {
            if (activeLiftEffect != null)
            {
                Destroy(activeLiftEffect.gameObject); // Ensure no duplicate effects
            }

            activeLiftEffect = Instantiate(liftEffect, position, Quaternion.identity);
            activeLiftEffect.transform.SetParent(null);
            activeLiftEffect.transform.rotation = Quaternion.identity;
            activeLiftEffect.Play();
        }
    }

    public void StopLiftEffect()
    {
        if (activeLiftEffect != null)
        {
            activeLiftEffect.Stop();
            Destroy(activeLiftEffect.gameObject, 1f);
            activeLiftEffect = null;
        }
    }

    public void StartHoldEffect(Transform target)
    {
        StopHoldEffect(); // Ensure the previous effect is removed before creating a new one

        if (holdEffect != null)
        {
            currentHoldEffect = Instantiate(holdEffect, target.position, Quaternion.identity);
            currentHoldEffect.transform.SetParent(target); // Attach to the object
            currentHoldEffect.Play();
        }
    }

    public void StopHoldEffect()
    {
        if (currentHoldEffect != null)
        {
            currentHoldEffect.Stop();
            Destroy(currentHoldEffect.gameObject, 1f);
            currentHoldEffect = null; // Reset so a new effect can be created
        }
    }

    public void PlayThrowEffect(Vector3 position)
    {
        if (throwEffect != null)
        {
            ParticleSystem effect = Instantiate(throwEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 1f);
        }
    }
}
