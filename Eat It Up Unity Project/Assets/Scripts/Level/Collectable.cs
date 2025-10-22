using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem collectedVFX;
    [SerializeField]
    private SpriteRenderer spriteToChange;
    [SerializeField]
    private Sprite normalSprite;
    [SerializeField]
    private int normalScore;
    [SerializeField]
    private Vector3 normalSpriteScale;
    [SerializeField]
    private Sprite rareSprite;
    [SerializeField]
    private int rareScore;
    [SerializeField]
    private Vector3 rareSpriteScale;
    [SerializeField]
    private Sprite goldSprite;
    [SerializeField]
    private int goldScore;
    [SerializeField]
    private Vector3 goldSpriteScale;
    [SerializeField]
    private CollectableType myType;
    [SerializeField]
    private Animator myAnimator;

    private static string Normal = "Normal";
    private static string Rare = "Rare";
    private static string Gold = "Gold";

    public enum CollectableType
    {
        Normal,
        Rare,
        Gold
    }

    void Awake()
    {
        SetCollectable();
    }

    private void SetCollectable()
    {
        switch (myType)
        {
            case CollectableType.Normal:
                spriteToChange.gameObject.transform.localScale = normalSpriteScale;
                myAnimator.SetTrigger(Normal);
                if(normalSprite != null)
                    spriteToChange.sprite = normalSprite;
                break;
            case CollectableType.Rare:
                spriteToChange.gameObject.transform.localScale = rareSpriteScale;
                myAnimator.SetTrigger(Rare);
                if(rareSprite != null)
                    spriteToChange.sprite = rareSprite;
                break;
            case CollectableType.Gold:
                spriteToChange.gameObject.transform.localScale = goldSpriteScale;
                myAnimator.SetTrigger(Gold);
                if(goldSprite != null)
                    spriteToChange.sprite = goldSprite;
                break;
            default:
                break;
        }
    }

    public void ObjectCollected()
    {
        Instantiate(collectedVFX, gameObject.transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
