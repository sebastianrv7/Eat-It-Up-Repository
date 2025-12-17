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
    [SerializeField] private Sprite epicSprite;
    [SerializeField] private int epicScore;
    [SerializeField] private Vector3 epicSpriteScale;

    [SerializeField] private Sprite legendarySprite;
    [SerializeField] private int legendaryScore;
    [SerializeField] private Vector3 legendarySpriteScale;



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
                case CollectableType.Epic:
                    return epicScore;
                case CollectableType.Legendary:
                    return legendaryScore;
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
        Gold,
        Epic,
        Legendary
    }

    void Awake()
    {
        if (myScoreText == null)
            myScoreText = GetComponentInChildren<TMPro.TMP_Text>(true);

        SetRandomCollectableType();
        SetCollectable();
    }

    public void SetCollectable()
    {
        switch (myType)
        {
            case CollectableType.Normal:
                spriteToChange.transform.localScale = normalSpriteScale;
                myAnimator.SetTrigger(Normal);
                spriteToChange.sprite = normalSprite;
                break;

            case CollectableType.Rare:
                spriteToChange.transform.localScale = rareSpriteScale;
                myAnimator.SetTrigger(Rare);
                spriteToChange.sprite = rareSprite;
                break;

            case CollectableType.Gold:
                spriteToChange.transform.localScale = goldSpriteScale;
                myAnimator.SetTrigger(Gold);
                spriteToChange.sprite = goldSprite;
                break;

            case CollectableType.Epic:
                spriteToChange.transform.localScale = epicSpriteScale;
                myAnimator.SetTrigger("Epic");
                spriteToChange.sprite = epicSprite;
                break;

            case CollectableType.Legendary:
                spriteToChange.transform.localScale = legendarySpriteScale;
                myAnimator.SetTrigger("Legendary");
                spriteToChange.sprite = legendarySprite;
                break;
        }

        //  PROTECCIÓN CRÍTICA
        if (myScoreText != null)
            myScoreText.SetText(MyScore.ToString("D2"));
        else
            Debug.LogWarning($"Collectable sin TMP_Text en {gameObject.name}");
    }

    private void SetRandomCollectableType()
    {
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll < 10f)
            myType = CollectableType.Legendary;
        else if (roll < 25f)
            myType = CollectableType.Epic;
        else if (roll < 45f)
            myType = CollectableType.Gold;
        else if (roll < 70f)
            myType = CollectableType.Rare;
        else
            myType = CollectableType.Normal;
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
