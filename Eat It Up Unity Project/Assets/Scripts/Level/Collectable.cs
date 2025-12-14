using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectable : MonoBehaviour
{
    [SerializeField] 
    private FloatingText floatingTextPrefab;
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
    private TMPro.TMP_Text myScoreText;

    [Header("Spawn Chances")]
    [SerializeField] private float normalChance = 40f;
    [SerializeField] private float rareChance = 40f;
    [SerializeField] private float goldChance = 20f;


    [SerializeField]
    private Animator myAnimator;

    public CollectableType MyCollectableType {get{ return myType; }}
    public int MyScore {
        get
        {
            switch (myType)
            {
                case CollectableType.Normal:
                    return normalScore;
                case CollectableType.Rare:
                    return rareScore;
                case CollectableType.Gold:
                    return goldScore;
                default:
                    return normalScore;
            }
        }
    }

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
        SetRandomCollectableType(); 
        SetCollectable();
        
    }

    public void SetCollectable()
    {
        switch (myType)
        {
            case CollectableType.Normal:
                spriteToChange.gameObject.transform.localScale = normalSpriteScale;
                myAnimator.SetTrigger(Normal);
                if (normalSprite != null)
                    spriteToChange.sprite = normalSprite;
                break;
            case CollectableType.Rare:
                spriteToChange.gameObject.transform.localScale = rareSpriteScale;
                myAnimator.SetTrigger(Rare);
                if (rareSprite != null)
                    spriteToChange.sprite = rareSprite;
                break;
            case CollectableType.Gold:
                spriteToChange.gameObject.transform.localScale = goldSpriteScale;
                myAnimator.SetTrigger(Gold);
                if (goldSprite != null)
                    spriteToChange.sprite = goldSprite;
                break;
            default:
                break;
        }

        myScoreText.SetText(MyScore.ToString("D2"));
    } 

    private void SetRandomCollectableType()
    {
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll < goldChance)
        {
            myType = CollectableType.Gold;
        }
        else if (roll < goldChance + rareChance)
        {
            myType = CollectableType.Rare;
        }
        else
        {
            myType = CollectableType.Normal;
        }
    }
    public void ObjectCollected()
    {
        // Spawn del texto
        FloatingText ft = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        int multiplier = ScoreMultiplierManager.Instance.GetCurrentMultiplier();
        int finalScore = MyScore * multiplier;
        ft.SetText("+" + finalScore);

        // Sonidos y score como ya tienes
        SoundManager.instance.PlaySFX(SoundManager.SoundFXType.Collectable);
        Score.Instance.AddScore(finalScore);

        gameObject.SetActive(false);
    }

    public void SetType(CollectableType newType)
    {
        myType = newType;
        SetCollectable();
    }
}
