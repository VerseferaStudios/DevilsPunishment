using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{


    public List<Network_Player> playerlist;


    public string[] getPlayerlist()
    {
        string[] usernames = new string[playerlist.Count];
        int i = 0;
        foreach (Network_Player player in playerlist)
        {
            usernames[i] = player.getUsername();
        }

        return usernames;
    }

    public int getPlayerCount()
    {
        return playerlist.Count;
    }

    public List<Network_Player> getPlayers()
    {
        return playerlist;
    }

    public Network_Player findPlayer(string username)
    {
        foreach (Network_Player player in playerlist)
        {
            if(username == player.getUsername())
            {
               


                return player;
            }
        }
        throw new MissingComponentException("Whoops.. player "+username+" not found"); // nasty! take care
    }


    public void registerPlayer(Network_Player player)
    {
        playerlist.Add(player);
    }


}
