using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update()
    {
        transform.rotation *= Quaternion.Euler(0, 0, speed * Time.deltaTime);
    }
}