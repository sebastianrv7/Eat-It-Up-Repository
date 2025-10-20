using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem collectedVFX;

    public void ObjectCollected()
    {
        Instantiate(collectedVFX, gameObject.transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
