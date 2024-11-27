using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed = 15f;

    private bool isSet;

    void Start()
    {
        isSet = false;
    }

    void FixedUpdate()
    {
        if (isSet)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

    public void SetNote()
    {
        isSet = !isSet;
    }
}
