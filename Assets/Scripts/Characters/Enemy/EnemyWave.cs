using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

public class EnemyWave : MonoBehaviour
{
    [SerializeField]
    List<EnemyBase> enemies;
    [SerializeField]
    EnemyStats[] enemiesStats;
    [SerializeField]
    Transform[] spawnPoints;
    [SerializeField]
    bool spawned;

    [SerializeField]
    EnemyManager manager;

    [SerializeField]
    PlayerBase player;

    [SerializeField]
    int roleReassignIntervalInMilliSeconds;

    [SerializeField]
    bool hardMode;

    [SerializeField]
    Slider enemyHealthSlider;


    private void OnEnable()
    {
        if (!spawned)
        {
            enemies = new List<EnemyBase>();
            int index = 0;
            foreach (EnemyStats stats in enemiesStats)
            {
                GameObject enemyGO = Instantiate(stats.enemyPrefab, spawnPoints[index].position, Quaternion.identity);
                enemyGO.GetComponent<Health>().HPSlider = enemyHealthSlider;
                EnemyBase enemy = enemyGO.GetComponent<EnemyBase>();
                enemy.AssignPlayer(player, this);
                enemies.Add(enemy);
                index++;
            }
            spawned = true;
        }
        else
        {
            foreach (EnemyBase enemy in enemies)
            {
                enemy.AssignPlayer(player, this);
                enemy.GetComponent<Health>().HPSlider = enemyHealthSlider;
            }
        }
        ReassignRoleAsync();
    }

    bool skipThisTime;

    private async void ReassignRoleAsync()
    {
        while(enemies.Count > 1)
        {
            await Task.Delay(roleReassignIntervalInMilliSeconds);
            if (skipThisTime)
            {
                skipThisTime = false;
            }
            else
            {
                AssignRole();
            }
        }
        
    }

    int random;
    void AssignRole()
    {
        
        if(enemies.Count == 1)
        {
            enemies[0].AssignRole(0);
        }
        else if(enemies.Count == 2)
        {
            enemies.Sort();
            
            if(enemies[0].State == 4)
            {
                random = Random.Range(0, 1);
                if(random == 0)
                {
                    enemies[0].AssignRole(1);
                    enemies[1].AssignRole(0);
                }
                else
                {
                    enemies[0].AssignRole(0);
                    enemies[1].AssignRole(1);
                }
            }
            else
            {
                enemies[0].AssignRole(0);
                enemies[1].AssignRole(1);
            }
        }
        else
        {
            enemies.Sort();
            
            if(enemies[0].State == 4)
            {
                random = Random.Range(0, 1);
                if(random == 0 && hardMode)
                {
                    enemies[1].AssignRole(0);
                    enemies[0].AssignRole(1);
                    int half = Mathf.RoundToInt(enemies.Count / 2);
                    if (half > 2)
                    {
                        for (int i = 2; i < half; i++)
                        {
                            enemies[i].AssignRole(1);
                        }
                        for (int l = half; l < enemies.Count; l++)
                        {
                            enemies[l].AssignRole(2);
                        }
                    }
                    else
                    {
                        for (int i = 2; i < enemies.Count; i++)
                        {
                            enemies[i].AssignRole(1);
                        }
                    }
                }
                else
                {
                    enemies[0].AssignRole(0);
                    int half = Mathf.RoundToInt(enemies.Count / 2);
                    for (int i = 1; i < half; i++)
                    {
                        enemies[i].AssignRole(1);
                    }
                    for (int l = half + 1; l < enemies.Count; l++)
                    {
                        enemies[l].AssignRole(2);
                    }
                }                
            }
            else
            {
                enemies[0].AssignRole(0);
                int half = Mathf.RoundToInt(enemies.Count / 2);
                for (int i = 1; i < half; i++)
                {
                    enemies[i].AssignRole(1);
                }
                for (int l = half + 1; l < enemies.Count; l++)
                {
                    enemies[l].AssignRole(2);
                }
            }
            
        }
    }


    public void EnemyDie(EnemyBase enemy)
    {
        enemies.Remove(enemy);
        if(enemies.Count == 0)
        {
            manager.EndWave();
        }
        else
        {
            AssignRole();
            skipThisTime = true;
        }
    }
}
