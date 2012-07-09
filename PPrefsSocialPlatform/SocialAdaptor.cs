using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.PPrefs;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SocialPlatforms.GameCenter;


public static class SocialAdaptor 
{
    static bool authRequest = false;
    static bool onlinePlatform = true;

    public static void Authenticate()
    {
        if(authRequest) return;

        if(Social.localUser.authenticated) return;

        authRequest = true;

        //select social platform
        bool useOffline = false;
#if (UNITY_EDITOR || UNITY_ANDROID || UNITY_WEBPLAYER)
        useOffline = true;
#endif
        if(Application.internetReachability == NetworkReachability.NotReachable)
            useOffline = true;

        if(useOffline){
            Social.Active = PPrefsSocialPlatform.Instance;
            Debug.Log("USE_LOCAL_LEADERBOARD");
            onlinePlatform = false;
        }

#if UNITY_IPHONE
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
#endif
        Debug.Log("SocialAdaptor.Authenticate");
        Social.localUser.Authenticate(success => {
            if(success){
                Debug.Log ("Authentication successful");
                string userInfo = "Username: " + Social.localUser.userName + 
                    "\nUser ID: " + Social.localUser.id + 
                    "\nIsUnderage: " + Social.localUser.underage;
                Debug.Log (userInfo);
                //GameObjectUtils.BroadCastMessage("OnAuthenticateSuccess", true);
            }
            else {
                Debug.Log("Authentication Fail");

                Social.Active = PPrefsSocialPlatform.Instance;
                Social.localUser.Authenticate(null);
                Debug.Log("Authenticate fail, USE_LOCAL_LEADERBOARD");
                onlinePlatform = false;
                //GameObjectUtils.BroadCastMessage("OnAuthenticateSuccess", false);
            }
            authRequest = false;
        });
    }

    public static bool IsAuthenticated()
    {
        return Social.localUser.authenticated;
    }

    public static bool IsOnline()
    {
        return onlinePlatform;
    }
}
