using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;

    public GameObject damageRing;
    private Vector3 initialScale; // Store the initial scale of the object
    private int clickCount = 0; // Track the number of clicks
    private bool isTakingDamage = false; // Prevent multiple damage calls per click
    public bool isOnBase;

    // Radial health bar
    public GameObject radialHealthBarPrefab; // Assign the prefab in the Unity Editor
    public RadialHealthBar radialHealthBar;

    public float experiencePoints = 0;
    [SerializeField] public int rank = 1;
    private int nextRankThreshold = 1; // Initial threshold for rank up

    //public GameObject radialExperienceBarPrefab; // Prefab for the experience bar
    public RadialHealthBar radialExperienceBar;
    public Team team;

    public GameObject spawner;
    
    public TextMeshProUGUI rankDisplay;

    void Start()
    {
        health = maxHealth;


        if (damageRing != null)
        {
            damageRing = Instantiate(damageRing, transform.position, Quaternion.identity);
        }
        initialScale = transform.localScale; // Save the initial scale

        // Instantiate radial experience bar
       

       
    }

    void Update()
    {


        if (damageRing != null)
        {
            damageRing.transform.position = gameObject.transform.position;
        }
        if(rankDisplay != null)
        {
            //rankDisplay.transform.position = gameObject.transform.position + new Vector3(0, 1.5f, 0); // Adjust position above the object
            rankDisplay.text = rank.ToString(); // Update the rank display text
        }
        
    }

    public void TakeDamage(int damage, string attackerTeamName = null)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        


        if (radialHealthBar != null)
        {
            float healthPercentage = Mathf.Clamp01((float)health / maxHealth);
            Debug.Log($"Health %: {healthPercentage}"); // Should print 0.0 to 1.0
            radialHealthBar.UpdateHealthBar(healthPercentage);
        }
        
       


        float dmgProgress = 1 - ((float)health / maxHealth);

        // Ensure the object only shrinks and doesn't grow
        if (!isOnBase)
        {
            float scale = Mathf.Lerp(initialScale.x, 0, dmgProgress);
            if (scale < transform.localScale.x) // Only apply scaling if it's smaller than the current scale
            {
                transform.localScale = new Vector3(scale, scale, initialScale.z);
            }
        }
        

        Debug.Log("Health: " + health + ", Scale: " + transform.localScale);

        if (health <= 0)
        {
            // Notify the attacking team
            if (!string.IsNullOrEmpty(attackerTeamName) && attackerTeamName != team.teamName)
            {
                Team attackingTeam = FindTeamByName(attackerTeamName);
                attackingTeam.AwardExperience(1); // Award experience points to the attacking team
                Debug.Log($"Team {attackingTeam} awarded XP for destroying an object.");
            }

            if (damageRing != null)
            {
                Destroy(damageRing); // Destroy the damage ring
            }
            if (radialHealthBar != null)
            {
                Destroy(radialHealthBar.gameObject);
                Destroy(radialHealthBarPrefab.gameObject);// Destroy the radial health bar
            }
            if (radialExperienceBar != null)
            {
                Destroy(radialExperienceBar.gameObject);
                //Destroy(radialExperienceBarPrefab); // Destroy the radial experience bar

            }
            if (rankDisplay != null)
            {
                Destroy(rankDisplay.gameObject); // Destroy the rank display
            }

            //if (team != null)
            //{
            //    team.AwardExperience(1); // Award experience to the team
            //}









            Destroy(gameObject); // Destroy the main object
        }
    }

    public void UpdateExperienceBar(float expPercentage)
    {
        if (radialExperienceBar != null)
        {
            Debug.Log($"Updating Experience Bar with percentage: {expPercentage}");
            //radialExperienceBar.IncreaseXP(expPercentage * Team.nextRankThreshold); // Convert percentage to XP amount
        }
    }

    public void UpdateRankDisplay(int rank)
    {
        if (rankDisplay != null)
        {
            rankDisplay.text = rank.ToString();
        }
    }

    private Team FindTeamByName(string teamName)
    {
        Team[] allTeams = FindObjectsOfType<Team>();
        Debug.Log($"Found {allTeams} teams in the scene.");
        foreach (Team t in allTeams)
        {
            if (t.teamName == teamName)
            {
                return t;
            }
        }
        return null;
    }
}
