using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField]
    private GameManager gameManager;

    private int currentScore;
    private int starsEarned;
    private int maxScorePerLevel;

    private static List<List<Collectable>> rareCollectablesPerLevel = new List<List<Collectable>>();
    private static List<List<Collectable>> rareCollectablesCollectedPerLevel = new List<List<Collectable>>();
    private static List<List<Collectable>> goldCollectablesPerLevel = new List<List<Collectable>>();
    private static List<List<Collectable>> goldCollectablesCollectedPerLevel = new List<List<Collectable>>();
    private static List<int> scorePerLevel = new List<int>();
    private static List<int> starsPerLevel = new List<int>();


    public int RareCollectableCollectedPerLevel { get { return rareCollectablesCollectedPerLevel[gameManager.CurrentLevel].Count; } }
    public int GoldCollectableCollectedPerLevel { get { return goldCollectablesCollectedPerLevel[gameManager.CurrentLevel].Count; } }
    public int MaxRareCollectablePerLevel { get { return rareCollectablesPerLevel[gameManager.CurrentLevel].Count; } }
    public int MaxGoldCollectablePerLevel { get { return goldCollectablesPerLevel[gameManager.CurrentLevel].Count; } }
    public int Levels { get { return scorePerLevel.Count; } }
    public int CurrentScore { get { return currentScore; } }
    public int StarsEarned { get { return starsEarned; } }

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
        {
            rareCollectablesPerLevel.RemoveAt(gameManager.CurrentLevel);
            rareCollectablesCollectedPerLevel.RemoveAt(gameManager.CurrentLevel);
            goldCollectablesPerLevel.RemoveAt(gameManager.CurrentLevel);
            goldCollectablesCollectedPerLevel.RemoveAt(gameManager.CurrentLevel);
        }

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
            scorePerLevel.RemoveAt(gameManager.CurrentLevel);

        scorePerLevel.Add(currentScore);
    }

    public void UpdateScorePerLevel()
    {
        scorePerLevel[gameManager.CurrentLevel] = currentScore;
        CalculateStars();
    }

    [ContextMenu("Print MaxScore")]
    public void CalculateStars()
    {
        maxScorePerLevel = 0;
        foreach (Collectable item in rareCollectablesPerLevel[gameManager.CurrentLevel])
        {
            maxScorePerLevel += item.MyScore;
        }

        foreach (Collectable item in goldCollectablesPerLevel[gameManager.CurrentLevel])
        {
            maxScorePerLevel += item.MyScore;
        }

        float starScore = (float)currentScore / (float)maxScorePerLevel;
        if (starScore <= 0.33f)
            starsEarned = 0;
        else if (starScore <= 0.66f)
            starsEarned = 1;
        else if (starScore <= 0.99f)
            starsEarned = 2;
        else if (starScore >= 1f)
            starsEarned = 3;

        starsPerLevel.Add(starsEarned);
        //print(starsEarned);
    }

    public int GetCollectablesCollectedPerLevel(Collectable.CollectableType type, int level)
    {
        switch (type)
        {
            case Collectable.CollectableType.Normal:
                return 0;
            case Collectable.CollectableType.Rare:
                return rareCollectablesCollectedPerLevel[level].Count;
            case Collectable.CollectableType.Gold:
                return goldCollectablesCollectedPerLevel[level].Count;
            default:
                break;
        }
        return -1;
    }
    public int GetMaxCollectablesPerLevel(Collectable.CollectableType type, int level)
    {
        switch (type)
        {
            case Collectable.CollectableType.Normal:
                return 0;
            case Collectable.CollectableType.Rare:
                return rareCollectablesPerLevel[level].Count;
            case Collectable.CollectableType.Gold:
                return goldCollectablesPerLevel[level].Count;
            default:
                break;
        }
        return -1;
    }

    public int GetScorePerLevel(int level)
    {
        return scorePerLevel[level];
    }

    public int GetStarsPerLevel(int level)
    {
        return starsPerLevel[level];
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
