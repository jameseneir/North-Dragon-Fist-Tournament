using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public int gameMode;
    [HideInInspector]
    public List<CharacterStats> players;
    [HideInInspector]
    public List<CharacterStats> enemies;
    
}
