using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellDisperser : MonoBehaviour
{
    [SerializeField]
    private GameObject[] shells;

    private float timer = 2f;
    private int index=0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer-=Time.deltaTime;
        if (timer <= 0) {
            timer = 2f;

            var spawnObj = Instantiate(shells[index], shells[index].transform.position, shells[index].transform.rotation);
            spawnObj.SetActive(true);
            index = (index+1)%shells.Length;
        }

    }
}
