using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InitializeLevel : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawns;
    [SerializeField] private MainGameManager mainGameManager;
    [SerializeField] private GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
        var OnlinePlayerConfigs = OnlinePlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();


        if (playerConfigs.Length == 0)
        {
            for (int i = 0; i < OnlinePlayerConfigs.Length; i++)
            {
                
            }

            mainGameManager.setIsOnline(true);
            mainGameManager.setOnlinePlayerConfigurationManager(OnlinePlayerConfigurationManager.Instance.gameObject);
        }
        else
        {
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation,
                    gameObject.transform);
                player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i]);
            }
            mainGameManager.setPlayerConfigurationManager(PlayerConfigurationManager.Instance.gameObject);

        }
    }
    
}
