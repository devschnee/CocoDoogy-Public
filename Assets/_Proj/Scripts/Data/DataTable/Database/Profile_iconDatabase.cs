using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Profile_iconData", menuName = "GameData/Profile_iconData")]
public class Profile_iconDatabase : ScriptableObject
{
    public List<Profile_iconData> profileList = new List<Profile_iconData>();
}
