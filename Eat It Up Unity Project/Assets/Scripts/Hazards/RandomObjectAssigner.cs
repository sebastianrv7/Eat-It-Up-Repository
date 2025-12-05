using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectAssigner : MonoBehaviour
{
    [Header("List of objects to assign (9)")]
    [SerializeField] private GameObject[] objects;

    [Header("List of existing positions (9)")]
    [SerializeField] private Transform[] positions;

    void Start()
    {
        if (objects.Length != positions.Length)
        {
            Debug.LogError("The number of objects and positions must be equal.");
            return;
        }

        // Create a shuffled copy of the objects array
        GameObject[] shuffledObjects = ShuffleArray(objects);

        // Assign each object to a position
        for (int i = 0; i < positions.Length; i++)
        {
            shuffledObjects[i].transform.position = positions[i].position;
        }
    }

    // Fisher-Yates shuffle algorithm
    private GameObject[] ShuffleArray(GameObject[] array)
    {
        GameObject[] newArray = (GameObject[])array.Clone();
        for (int i = 0; i < newArray.Length; i++)
        {
            int randomIndex = Random.Range(i, newArray.Length);
            GameObject temp = newArray[i];
            newArray[i] = newArray[randomIndex];
            newArray[randomIndex] = temp;
        }
        return newArray;
    }
}
