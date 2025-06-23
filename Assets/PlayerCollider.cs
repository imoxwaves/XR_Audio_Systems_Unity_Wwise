using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColider : MonoBehaviour
{

    public Transform head;
    public Transform Floor;

    CapsuleCollider MyCollider;

    // Start is called before the first frame update
    void Start()
    {
        MyCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float height = head.position.y - Floor.position.y;
        MyCollider.height = height;
        transform.position = head.position - Vector3.up * height / 2;



    }
}