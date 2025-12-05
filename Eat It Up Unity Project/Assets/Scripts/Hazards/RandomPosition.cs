using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    // Posición inicial (se toma automáticamente del objeto)
    private float initialX;

    // Offset en X para la segunda posible posición
    [SerializeField] private float offsetX = 3f;

    void Start()
    {
        // Guardamos la posición inicial del objeto
        initialX = transform.position.x;

        // Elegimos aleatoriamente entre 0 y 1
        int randomChoice = Random.Range(0, 2);

        // Calculamos la nueva posición
        float newX = (randomChoice == 0) ? initialX : initialX + offsetX;

        // Asignamos la posición al objeto
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
