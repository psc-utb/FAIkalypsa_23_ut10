using UnityEngine;
using System.Collections;

public class BulletCCScript : MonoBehaviour {

	[Tooltip("Furthest distance bullet will look for target")]
	public float maxDistance = 1000000;
	RaycastHit hit;
	[Tooltip("Prefab of wall damange hit. The object needs 'LevelPart' tag to create decal on it.")]
	public GameObject decalHitWall;
	[Tooltip("Decal will need to be sligtly infront of the wall so it doesnt cause rendeing problems so for best feel put from 0.01-0.1.")]
	public float floatInfrontOfWall;
    [Tooltip("Name of layer for Walls")]
    [SerializeField]
    private string wallLayerName;
	[Tooltip("Blood prefab particle this bullet will create upoon hitting enemy")]
	public GameObject bloodEffect;
    [Tooltip("Name of layer for characters who can bleed")]
	[SerializeField]
    private string charactersLayerName;
    [Tooltip("Put Weapon layer and Player layer to ignore bullet raycast.")]
	public LayerMask ignoreLayer;

	/*
	* Uppon bullet creation with this script attatched,
	* bullet creates a raycast which searches for corresponding tags.
	* If raycast finds somethig it will create a decal of corresponding tag.
	*/
	void Update () {

		if(Physics.Raycast(transform.position, transform.forward,out hit, maxDistance, ~ignoreLayer)){
			if(decalHitWall){
				if(hit.collider.gameObject.layer == LayerMask.NameToLayer(wallLayerName)){
					Instantiate(decalHitWall, hit.point + hit.normal * floatInfrontOfWall, Quaternion.LookRotation(hit.normal));
					Destroy(gameObject);
				}
				if(hit.collider.gameObject.layer == LayerMask.NameToLayer(charactersLayerName))
                {
					Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
					Destroy(gameObject);
				}
			}		
			Destroy(gameObject);
		}
		Destroy(gameObject, 0.1f);
	}

}
