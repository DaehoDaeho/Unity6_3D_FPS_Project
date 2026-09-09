using UnityEngine;

public class LookAtTest : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.position);
    }
}
