using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILevelGroupManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI rareScore;
    [SerializeField]
    private TextMeshProUGUI goldScore;
    [SerializeField]
    private TextMeshProUGUI totalScore;
    [SerializeField]
    private UIStarsManager starsManager;

    public void SetScore(int level, int rare, int maxRare, int gold, int maxGold, int score, int stars)
    {
        levelText.SetText("Level " + level.ToString());
        rareScore.SetText(rare.ToString() + "/" + maxRare.ToString());
        goldScore.SetText(gold.ToString() + "/" + maxGold.ToString());
        totalScore.SetText(score.ToString());
        starsManager.ShowStars(stars);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
