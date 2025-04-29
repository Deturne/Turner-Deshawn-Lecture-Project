using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpawn : MonoBehaviour
{
    public List<GameObject> basePrefab; // Prefab for the base
    public Vector2 spawnPoint; // Spawn point for the base
    public List<GameObject> spawnedBases = new List<GameObject>(); // List to keep track of spawned bases
    public GameObject healthBarPrefab; // Prefab for the health bar
    public GameObject xpBarPrefab; // Prefab for the XP bar
    public List<Team> teams; // Reference to the Team script

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            // Set spawn points for each base
            basePrefab[0].GetComponent<Spawner>().spawnPoint = new Vector2(-6.151789f, 2.833129f);
            basePrefab[1].GetComponent<Spawner>().spawnPoint = new Vector2(5.42999983f, 2.83312941f);
            basePrefab[2].GetComponent<Spawner>().spawnPoint = new Vector2(5.5999999f, -2.88774347f);
            basePrefab[3].GetComponent<Spawner>().spawnPoint = new Vector2(-6.01638365f, -2.88774347f);

            // Spawn the base
            spawnPoint = basePrefab[i].GetComponent<Spawner>().spawnPoint;
            GameObject newBase = Instantiate(basePrefab[i].gameObject, spawnPoint, Quaternion.identity);
            spawnedBases.Add(newBase);

            // Attach health and XP bars
            AssignTeamToBase(newBase, i);
            AttachHealthBar(newBase);
            AttachXPBar(newBase, i);
        }




    }

    void AssignTeamToBase(GameObject baseObject, int teamIndex)
    {
        // Ensure the base has a Team component
        Team team = baseObject.GetComponent<Team>();
        if (team == null)
        {
            team = baseObject.AddComponent<Team>();
        }

        // Assign a unique team name and other properties
        team.teamName = $"Team {teamIndex + 1}";
        team.teamBase = baseObject;

        // Add the team to the list
        if (!teams.Contains(team))
        {
            teams.Add(team);
        }
    }

    void AttachHealthBar(GameObject baseObject)
    {
        if (healthBarPrefab != null)
        {
            // Instantiate the health bar as a child of the base
            GameObject healthBarInstance = Instantiate(healthBarPrefab, baseObject.transform.position, Quaternion.identity);
            healthBarInstance.transform.SetParent(baseObject.transform);

            // Link the health bar to the base's Damage script
            Damage damageScript = baseObject.GetComponent<Damage>();
            if (damageScript != null)
            {
                RadialHealthBar healthBar = healthBarInstance.GetComponent<RadialHealthBar>();
                if (healthBar != null)
                {
                    damageScript.radialHealthBar = healthBar;
                }
            }
        }
    }

    void AttachXPBar(GameObject baseObject, int teamIndex)
    {
        if (xpBarPrefab != null && teamIndex < teams.Count)
        {
            // Instantiate the XP bar as a child of the base
            Vector3 offset = new Vector3(0, 1.5f, 0); // Offset the XP bar above the base
            GameObject xpBarInstance = Instantiate(xpBarPrefab, baseObject.transform.position, Quaternion.identity);
            //GameObject xPfill = Instantiate(xpBarPrefab., baseObject.transform.position, Quaternion.identity);
            xpBarInstance.transform.SetParent(baseObject.transform);
            RadialHealthBar xpBar = xpBarInstance.GetComponent<RadialHealthBar>();
            if (xpBar != null)
            {
                // Assign the XP bar to the team
                Team team = teams[teamIndex];
                if (team != null)
                {
                    team.xpBar = xpBar;
                    xpBar.xpSlider.value = 0;
                }
            }


        }
    }
}   

