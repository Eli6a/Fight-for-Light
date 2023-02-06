using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] private float speed = 3;
    [SerializeField] private Transform target;
    [SerializeField] private float minDistance = 1.0f;
    [SerializeField] private float maxDistance = 7.0f;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag(Globals.PLAYER_TAG).transform;
    }

    void Update()
    {
        if ((Vector2.Distance(transform.position, target.position) > minDistance) && (Vector2.Distance(transform.position, target.position) < maxDistance))
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}
