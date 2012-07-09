/**
* @file PPrefsSetting.cs
* @brief Local Container of AchievementDescriptions & Leaderboards Setting
* @author groscalin (amugana@gmail.com)
* @version 1.0
* @date 2012-06-19
*/

using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections.Generic;

public class PPrefsSetting : ScriptableObject
{
    [System.Serializable]
    public class SeAchivementDescription
    {
        public string id;
        public string title;
        public string unAchievedDesc;
        public string achievedDesc;
        public bool hidden;
        public int points;
        public Texture2D image;
        public AchievementDescription Create()
        {
            return new AchievementDescription(id, title, image, achievedDesc, unAchievedDesc, hidden, points);
        }
    }

    [System.Serializable]
    public class SeLeaderboard
    {
        public string id;
        public string title;
        public Leaderboard Create()
        {
            Leaderboard lb = new Leaderboard();
            lb.id = id;
            lb.SetTitle(title);
            return lb;
        }
    }

    public SeAchivementDescription[] achievements;
    public SeLeaderboard[] leaderboards;

    public List<AchievementDescription> GetAchievementDescription()
    {
        List<AchievementDescription> adlist = new List<AchievementDescription>();
        for(int i=0; i<achievements.Length; ++i){
            adlist.Add(achievements[i].Create());
        }
        return adlist;
    }

    public List<Leaderboard> GetLeaderboards()
    {
        List<Leaderboard> bdlist = new List<Leaderboard>();
        for(int i=0; i<leaderboards.Length; ++i){
            bdlist.Add(leaderboards[i].Create());
        }
        return bdlist;
    }
}
