using System.Collections.Generic;
using UnityEngine;

public class SpawnFlags : MonoBehaviour
{
    private Vector2 mouse;
    [SerializeField] private int maxFlags = 2;

    [Header("Prefabs")]
    [SerializeField] private GameObject greenPrefab;
    [SerializeField] private GameObject yellowPrefab;
    [SerializeField] private GameObject redPrefab;
    [SerializeField] private GameObject bluePrefab;

    [Header("Spawned Flags")]
    [SerializeField] private List<GameObject> greenFlags = new List<GameObject>();
    [SerializeField] private List<GameObject> yellowFlags = new List<GameObject>();
    [SerializeField] private List<GameObject> redFlags = new List<GameObject>();
    [SerializeField] private List<GameObject> blueFlags = new List<GameObject>();
    
    void Update()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        TrySpawnOrMove(0, greenPrefab, greenFlags);
        TrySpawnOrMove(1, yellowPrefab, yellowFlags);
        TrySpawnOrMove(2, redPrefab, redFlags);
        TrySpawnOrMove(3, bluePrefab, blueFlags);
    }

    private void TrySpawnOrMove(int mouseButton, GameObject prefab, List<GameObject> list)
    {
        if (!Input.GetMouseButtonDown(mouseButton)) return;

        // If we're under the cap, spawn a new one

        if(Draggable.draggingFlag) return;

        if (list.Count < maxFlags)
        {
            var go = Instantiate(prefab, mouse, Quaternion.identity);
            list.Add(go);
        }
        else 
        {
            // Otherwise just move the closest
            MoveClosestFlag(mouse, list);
        }
    }

    private void MoveClosestFlag(Vector2 worldPos, List<GameObject> list)
    {
        GameObject closest = null;
        float minSqrDist = float.MaxValue;


        foreach (var f in list)
        {
            float sqr = ((Vector2)f.transform.position - worldPos).sqrMagnitude;
            
            if (sqr < minSqrDist)
            {

                minSqrDist = sqr;
                closest = f;
            }
        }

        if (closest != null)
            closest.transform.position = worldPos;
    }
}
