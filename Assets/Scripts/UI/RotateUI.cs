using UnityEngine;

public class RotateUI : MonoBehaviour
{
    [SerializeField] private float speed = 60f;

    void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}