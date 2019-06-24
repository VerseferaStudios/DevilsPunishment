using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class PlayerInformation : MonoBehaviour
{
    private CSteamID steamid = (CSteamID)0;
    private Vector3 playersPos;
    private Quaternion playersRot;

	public CSteamID getSteamid() {
		return this.steamid;
	}

	public void setSteamid(CSteamID steamid) {
		this.steamid = steamid;
	}

	public Vector3 getPlayersPos() {
		return this.playersPos;
	}

	public void setPlayersPos(Vector3 playersPos) {
		this.playersPos = playersPos;
	}

	public Quaternion getPlayersRot() {
		return this.playersRot;
	}

	public void setPlayersRot(Quaternion playersRot) {
		this.playersRot = playersRot;
	}



}
