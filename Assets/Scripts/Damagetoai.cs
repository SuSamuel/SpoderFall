using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagetoai : MonoBehaviour
{
    // Start is called before the first frame update
    void OnCollisionEnter(Collision collision)

    {
        if (collision.collider.tag =="enemyai")
        {
            Destroy(collision.gameObject);
        }
    }
}

