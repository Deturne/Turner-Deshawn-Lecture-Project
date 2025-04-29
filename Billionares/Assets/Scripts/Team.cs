using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField] public string teamName; // The name representing this team
    public GameObject teamBase; // The base associated with this team
    public List<Billion> billions = new List<Billion>(); // List of billions belonging to this team

    public int experiencePoints = 0; // Instance variable for XP
    [SerializeField] public int rank = 1;
    public static int nextRankThreshold = 10; // Shared threshold for rank-up

    public RadialHealthBar xpBar; // Reference to the XP bar

    void Start()
    {
        // Initialize team-specific logic here if needed
        //if (xpBar != null)
        //{
        //    xpBar.SetMaxHealth(nextRankThreshold); // Set the max value for the XP bar
        //}

        experiencePoints = 0; // Initialize experience points
    }

    public void AwardExperience(int amount)
    {
        experiencePoints += amount;

        Debug.Log($"Team {teamName} awarded {amount} XP. Total XP: {experiencePoints}");

        // Update the XP bar visually
        if (xpBar != null)
        {
            float xpPercentage = (float)experiencePoints / nextRankThreshold;
            xpBar.IncreaseXP(xpPercentage); // Update the XP bar
        }

        if (experiencePoints >= nextRankThreshold)
        {
            RankUp();
        }
    }

    private void RankUp()
    {
        rank++;
        experiencePoints -= nextRankThreshold; // Carry over excess XP
        Debug.Log($"Team {teamName} ranked up to {rank}!");

        // Reset the XP bar for the next rank
        if (xpBar != null)
        {
            xpBar.SetMaxHealth(nextRankThreshold);
            xpBar.IncreaseXP((float)experiencePoints / nextRankThreshold);
        }
    }

    public int GetRank()
    {
        return rank;
    }
}
