using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    private Vector3 offset;
    void Start()
    {
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + offset;
    }
}
