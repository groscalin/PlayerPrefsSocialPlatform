/**
* @file PPrefsSocialPlatform.cs
* @brief most code from Unity3d's Local impl. add some PlayerPrefsSerializer feat by me
* @author groscalin (amugana@gmail.com)
* @version 1.0
* @date 2012-06-19
*/

using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System;
using System.Collections.Generic;

namespace UnityEngine.SocialPlatforms.PPrefs
{
    public class PPrefsSocialPlatform : Singleton<PPrefsSocialPlatform>, ISocialPlatform
    {
        public const int MAX_RANK = 100;

        private static string settingPath = "LocalSocialSetting";
        private static LocalUser _localUser;
        private List<UserProfile> _friends = new List<UserProfile> ();
        private List<UserProfile> _users = new List<UserProfile> ();
        public List<AchievementDescription> _achievementDescriptions = new List<AchievementDescription> ();
        public List<Achievement> _achievements = new List<Achievement> ();
        public List<Leaderboard> _leaderboards = new List<Leaderboard> ();
        public Texture2D _defaultTexture;

        public ILocalUser localUser 
        {
            get 
            { 
                if(PPrefsSocialPlatform._localUser == null)
                {
                    PPrefsSocialPlatform._localUser = new LocalUser();
                }
                return PPrefsSocialPlatform._localUser; 
            }
        }

        public static void SetLocalSettingName(string path)
        {
            settingPath = path;
        }

        public IAchievement CreateAchievement()
        {
            return (IAchievement)new Achievement();
        }

        public ILeaderboard CreateLeaderboard()
        {
            return new Leaderboard();
        }

        public void LoadAchievementDescriptions(System.Action<IAchievementDescription[]> callback)
        {
            //load from Resources folder
            if(!VerifyUser())
                return;
            if(callback != null)
                callback(_achievementDescriptions.ToArray());
        }

        public void ReportProgress (string id, double progress, Action<bool> callback)
        {
            //Debug.Log("ReportProgress");
            if (!this.VerifyUser ())
            {
                return;
            }
            foreach (Achievement current in _achievements)
            {
                if (current.id == id && current.percentCompleted <= progress)
                {
                    if (progress >= 100.0)
                    {
                        current.SetCompleted (true);
                    }
                    current.SetHidden (false);
                    current.SetLastReportedDate (DateTime.Now);
                    current.percentCompleted = progress;
                    if (callback != null)
                    {
                        callback (true);
                    }
#if !UNITY_WEBPLAYER
                    PlayerPrefsSerializer.Save("achieve_"+id, current);
#endif
                    return;
                }
            }
            foreach (AchievementDescription current2 in _achievementDescriptions)
            {
                if (current2.id == id)
                {
                    bool completed = progress >= 100.0;
                    Achievement item = new Achievement (id, progress, completed, false, DateTime.Now);
                    _achievements.Add (item);
                    if (callback != null)
                    {
                        callback (true);
                    }
                    //Debug.Log(item);
#if !UNITY_WEBPLAYER
                    PlayerPrefsSerializer.Save("achieve_"+id, item);
#endif
                    return;
                }
            }
            Debug.LogError ("Achievement ID not found");
            if (callback != null)
            {
                callback (false);
            }
        }

        public void ReportScore (long score, string board, Action<bool> callback)
        {
            //Debug.Log("ReportScore");
            if (!this.VerifyUser ())
            {
                return;
            }
            foreach (Leaderboard current in _leaderboards)
            {
                if (current.id == board)
                {
                    List<Score> scoreList = new List<Score> ((Score[])current.scores)
                    {
                        new Score (board, score, this.localUser.id, DateTime.Now, score + " points", 0)
                    };

                    scoreList.Sort ((Score s1, Score s2) => s2.value.CompareTo (s1.value));
                    for (int i = 0; i < scoreList.Count; i++)
                    {
                        scoreList [i].SetRank (i + 1);
                    }

                    current.SetScores (scoreList.GetRange(0,Mathf.Min(MAX_RANK, scoreList.Count)).ToArray());

                    if (callback != null)
                    {
                        callback (true);
                    }
#if !UNITY_WEBPLAYER
                    PlayerPrefsSerializer.Save("scores_"+board, current.scores);
#endif
                    return;
                }
            }
            Debug.LogError ("Leaderboard not found");
            if (callback != null)
            {
                callback (false);
            }
        }

        public void LoadAchievements(System.Action<IAchievement[]> callback)
        {
            //load from pprefs
            if (!this.VerifyUser ())
            {
                return;
            }
            if (callback != null)
            {
                callback (_achievements.ToArray ());
            }
        }

        public void LoadScores(string leaderboardID, System.Action<IScore[]> callback)
        {
            //Debug.Log("LoadScores");
            //load from pprefs
            if (!this.VerifyUser ())
            {
                return;
            }
            foreach (Leaderboard current in _leaderboards)
            {
                if (current.id == leaderboardID)
                {
                    this.SortScores (current);
                    if (callback != null)
                    {
                        callback (current.scores);
                    }
                    return;
                }
            }
            Debug.LogError ("Leaderboard not found");
            if (callback != null)
            {
                callback (new Score[0]);
            }
        }

        public void LoadUsers(string[] userIDs, System.Action<IUserProfile[]> callback)
        {
        	List<UserProfile> list = new List<UserProfile> ();
            if (!this.VerifyUser ())
            {
                return;
            }
            for (int i = 0; i < userIDs.Length; i++)
            {
                //Debug.Log(userIDs[i]);
                string b = userIDs [i];
                foreach (UserProfile current in _users)
                {
                    if (current.id == b)
                    {
                        list.Add (current);
                    }
                }
                foreach (UserProfile current2 in _friends)
                {
                    if (current2.id == b)
                    {
                        list.Add (current2);
                    }
                }
            }
            callback (list.ToArray ());
        }

        public void ShowAchievementsUI()
        {
            Debug.Log ("ShowAchievementsUI not implemented");
        }

        public void ShowLeaderboardUI()
        {
            Debug.Log ("ShowLeaderboardUI not implemented");
        }

        void ISocialPlatform.Authenticate (ILocalUser user, Action<bool> callback)
        {
            LocalUser localUser = (LocalUser)user;
            //this._defaultTexture = CreateDummyTexture(32,32);
            this.PopulateStaticData ();
            localUser.SetAuthenticated (true);
            localUser.SetUnderage (false);
            localUser.SetUserID ("1000");
            localUser.SetUserName ("Me");
            _users.Add(localUser);
            //localUser.SetImage (this._defaultTexture);
            if (callback != null)
            {
                callback (true);
            }
        }

        bool ISocialPlatform.GetLoading (ILeaderboard board)
        {
            return this.VerifyUser () && ((Leaderboard)board).loading;
        }

        void ISocialPlatform.LoadFriends (ILocalUser user, Action<bool> callback)
        {
            if (!this.VerifyUser ())
            {
                return;
            }
            ((LocalUser)user).SetFriends (this._friends.ToArray ());
            if (callback != null)
            {
                callback (true);
            }
        }
        
		void ISocialPlatform.LoadScores (ILeaderboard board, Action<bool> callback)
		{
			if (!this.VerifyUser ())
			{
				return;
			}
			Leaderboard leaderboard = (Leaderboard)board;
			foreach (Leaderboard current in this._leaderboards)
			{
				if (current.id == leaderboard.id)
				{
					leaderboard.SetTitle (current.title);
					leaderboard.SetScores (current.scores);
					leaderboard.SetMaxRange ((uint)current.scores.Length);
				}
			}
			this.SortScores (leaderboard);
			this.SetLocalPlayerScore (leaderboard);
			if (callback != null)
			{
				callback (true);
			}
		}
        
        private bool VerifyUser()
        {
            if(!this.localUser.authenticated) {
                Debug.LogError("Must authenticated First");
                return false;
            }
            return true;
        }

        private void PopulateStaticData ()
        {
            PPrefsSetting lss = Resources.Load(settingPath) as PPrefsSetting;
            this._achievementDescriptions = lss.GetAchievementDescription();
            this._leaderboards = lss.GetLeaderboards();

            PlayerPrefsSerializer.handleNonSerializable = true;

            foreach(AchievementDescription adesc in _achievementDescriptions){
                //Debug.Log("  - AchieveDesc : "+adesc.id);
                if(PlayerPrefs.HasKey("achieve_"+adesc.id))
                    _achievements.Add(PlayerPrefsSerializer.Load<Achievement>("achieve_"+adesc.id));
            }

            foreach(Leaderboard lboard in _leaderboards){
                //Debug.Log("  - Leaderboard : "+lboard.id);
                if(PlayerPrefs.HasKey("scores_"+lboard.id))
                    lboard.SetScores(PlayerPrefsSerializer.Load<Score[]>("scores_"+lboard.id));
            }
        }
        
        private Texture2D CreateDummyTexture (int width, int height)
		{
			Texture2D texture2D = new Texture2D (width, height);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					Color color = ((j & i) <= 0) ? Color.gray : Color.white;
					texture2D.SetPixel (j, i, color);
				}
			}
			texture2D.Apply ();
			return texture2D;
		}
		
		private void SortScores (Leaderboard board)
		{
            //Debug.Log("SortScores");
			List<Score> list = new List<Score> ((Score[])board.scores);
			list.Sort ((Score s1, Score s2) => s2.value.CompareTo (s1.value));
			for (int i = 0; i < list.Count; i++)
			{
				list [i].SetRank (i + 1);
			}
            board.SetScores(list.ToArray());
		}
	
		private void SetLocalPlayerScore (Leaderboard board)
		{
            //Debug.Log("SetLocalPlayerScore");
			IScore[] scores = board.scores;
			for (int i = 0; i < scores.Length; i++)
			{
				Score score = (Score)scores [i];
				if (score.userID == this.localUser.id)
				{
					board.SetLocalUserScore (score);
					break;
				}
			}
		}
    }
}
