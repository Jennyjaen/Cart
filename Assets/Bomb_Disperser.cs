using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Disperser : MonoBehaviour
{
    [SerializeField]
    private GameObject[] bombs;

    private float timer = 5f;
    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 5f;

            var spawnObj = Instantiate(bombs[index], bombs[index].transform.position, bombs[index].transform.rotation);
            spawnObj.SetActive(true);
            index = (index + 1) % bombs.Length;
        }

    }
}
