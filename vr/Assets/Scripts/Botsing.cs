using UnityEngine;

public class Botsing : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.transform.position= new Vector3(gameObject.transform.position.x+1, gameObject.transform.position.y, gameObject.transform.position.z);
    }
}
