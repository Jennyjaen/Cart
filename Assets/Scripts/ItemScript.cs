using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    private bool hasItem = false;
    public GameObject[] itemGameobjects;
    public Sprite[] itemSprites;
    public Image yourSprite;

    public Animator ItemUIAnim;
    public Animator ItemUiScroll;

    int index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hasItem) useItem();
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ItemBox")
        {
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false; //question mark

            other.transform.GetChild(2).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false; //box
            other.transform.GetChild(2).GetChild(2).GetComponent<SkinnedMeshRenderer>().enabled = false; //box

            other.gameObject.GetComponent<Animator>().SetBool("Enlarge", false); //reset to start process
            StartCoroutine(getItem());
            ItemUIAnim.SetBool("ItemIn", true);
            ItemUiScroll.SetBool("Scroll", true);

            //re-enable
            yield return new WaitForSeconds(1);
            other.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = true; //question mark
            for (int i = 1; i < 3; i++)
            {
                other.transform.GetChild(2).GetChild(i).GetComponent<SkinnedMeshRenderer>().enabled = true; //box
            }
            other.gameObject.GetComponent<Animator>().SetBool("Enlarge", true);  //show the item box spawning with animation, even though it was already there
            other.gameObject.GetComponent<BoxCollider>().enabled = true;
        }

    }

    public IEnumerator getItem()
    {
        if (!hasItem)
        {
            index = Random.Range(0, itemGameobjects.Length);
            index = 1;
            yourSprite.sprite = itemSprites[index];
            yield return new WaitForSeconds(4f);

            //itemGameobjects[index].SetActive(true);
            //Instantiate(itemGameobjects[index],transform);
            hasItem = true;

        }
    }
    public void useItem()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            hasItem = false;
            ItemUIAnim.SetBool("ItemIn", false);
            ItemUiScroll.SetBool("Scroll", false);
            //itemGameobjects[index].SetActive(false);
            if (index == 0) { this.GetComponent<PlayerScript>().BoostTime = 3f; }
            else if(index==1)
            {
                var shell = Instantiate(itemGameobjects[index], transform.position + transform.forward * 5f + new Vector3(0, 0.1f, 0), transform.rotation);
                shell.SetActive(true);
                shell.GetComponent<GreenShell>().InitialShoot(100, new Vector3(transform.forward.x, 0, transform.forward.z));
            }
            else
            {
                var bomb = Instantiate(itemGameobjects[index], transform.position + transform.forward * 10f + new Vector3(0, 0.5f, 0), transform.rotation);
                bomb.SetActive(true);
                //bomb.GetComponent<Bomb>().bomb_thrown(5);
            }
        }
    }
}
