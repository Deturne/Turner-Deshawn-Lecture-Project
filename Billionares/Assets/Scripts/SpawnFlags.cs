using System.Collections.Generic;
using UnityEngine;

public class SpawnFlags : MonoBehaviour
{
    private Vector2 mouse;
    [SerializeField] GameObject green;
    [SerializeField] GameObject yellow;

    [SerializeField] List<GameObject> greenFlags;
    [SerializeField] List<GameObject> yellowFlags;

    private int greenCount;
    private int yellowCount;

    private int maxFlags = 2;

    private int deleteIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            spawnFlags(green, greenFlags);

        }
        else if (Input.GetMouseButtonDown(1))
        {

            spawnFlags(yellow, yellowFlags);

        }

    }

    void spawnFlags(GameObject flag, List<GameObject> flagList)
    {
        if (!Draggable.draggingFlag)
        {
            GameObject spawned = Instantiate(flag, mouse, Quaternion.identity);
            flagList.Add(spawned);

            if (deleteIndex >= flagList.Count)
            {
                deleteIndex = 0;
            }

            if (flagList.Count > maxFlags)
            {
                deleteIndex = (deleteIndex + 1) % flagList.Count;

                GameObject deletedFlag = flagList[deleteIndex];


                flagList.RemoveAt(deleteIndex);
                Destroy(deletedFlag);

            }
        }


    }
}

            

              



            
       

