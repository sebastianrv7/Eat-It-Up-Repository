using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField]
    private GameManager gameManager;

    private int currentScore;

    private static List<List<Collectable>> rareCollectablesPerLevel = new List<List<Collectable>>();
    private static List<List<Collectable>> rareCollectablesCollectedPerLevel = new List<List<Collectable>>();
    private static List<List<Collectable>> goldCollectablesPerLevel = new List<List<Collectable>>();
    private static List<List<Collectable>> goldCollectablesCollectedPerLevel = new List<List<Collectable>>();
    private static List<int> scorePerLevel = new List<int>();

    public int MaxRareCollectablePerLevel { get { return rareCollectablesPerLevel[gameManager.CurrentLevel].Count; } }
    public int MaxGoldCollectablePerLevel { get { return goldCollectablesPerLevel[gameManager.CurrentLevel].Count; } }
    public int CurrentScore { get { return currentScore; } }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void TrackCollectablesInLevel()
    {
        if (gameManager.CurrentLevel <= rareCollectablesCollectedPerLevel.Count - 1)
            return;

        rareCollectablesPerLevel.Add(new List<Collectable>());
        rareCollectablesCollectedPerLevel.Add(new List<Collectable>());
        goldCollectablesPerLevel.Add(new List<Collectable>());
        goldCollectablesCollectedPerLevel.Add(new List<Collectable>());

        Object[] collectablesFound = FindObjectsByType(typeof(Collectable), FindObjectsSortMode.None);
        foreach (Object collectable in collectablesFound)
        {
            Collectable collectableScript = (Collectable)collectable.GetComponent(typeof(Collectable));
            switch (collectableScript.MyCollectableType)
            {
                case Collectable.CollectableType.Normal:
                    break;
                case Collectable.CollectableType.Rare:
                    rareCollectablesPerLevel[gameManager.CurrentLevel].Add(collectableScript);
                    break;
                case Collectable.CollectableType.Gold:
                    goldCollectablesPerLevel[gameManager.CurrentLevel].Add(collectableScript);
                    break;
                default:
                    break;
            }
        }
    }

    public void CollectableCollected(Collectable collectableCollected)
    {
        currentScore += collectableCollected.MyScore;
        switch (collectableCollected.MyCollectableType)
        {
            case Collectable.CollectableType.Normal:
                break;
            case Collectable.CollectableType.Rare:
                rareCollectablesCollectedPerLevel[gameManager.CurrentLevel].Add(collectableCollected);
                break;
            case Collectable.CollectableType.Gold:
                goldCollectablesCollectedPerLevel[gameManager.CurrentLevel].Add(collectableCollected);
                break;
            default:
                break;
        }
    }

    public void InitializeScorePerLevel()
    {
        if (gameManager.CurrentLevel <= scorePerLevel.Count - 1)
            return;

        scorePerLevel.Add(currentScore);
    }

    public void UpdateScorePerLevel()
    {
        scorePerLevel[gameManager.CurrentLevel] = currentScore;
    }


    [ContextMenu("PrintCollectables")]
    public void PrintCollectablesScore()
    {
        /*
        for (int i = 0; i < rareCollectablesCollectedPerLevel.Count; i++)
        {
            print("Rare per Level " + i + ": " + rareCollectablesCollectedPerLevel[i].Count);

        }
        
        for (int i = 0; i < goldCollectablesCollectedPerLevel.Count; i++)
        {
            print("Gold Level " + i + ": "+ goldCollectablesCollectedPerLevel[i].Count);
            
        }
        for (int i = 0; i < rareCollectablesPerLevel.Count; i++)
        {
            print("Rare total per Level " + i + ": "+ rareCollectablesPerLevel[i].Count);
            
        }
        
        for (int i = 0; i < goldCollectablesPerLevel.Count; i++)
        {
            print("Gold total per Level " + i + ": "+ goldCollectablesPerLevel[i].Count);
            
        }
        */
        
        for (int i = 0; i < scorePerLevel.Count; i++)
        {
            print("score per Level " + i + ": " + scorePerLevel[i]);

        }

    }
}
