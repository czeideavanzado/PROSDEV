﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

	public GameObject terrain;
	public Transform generationPoint;
	public float distance;

	public float distanceMin;
	public float distanceMax;

	private float terrainWidth;

	// public GameObject[] terrainArray;
	private int terrainSelector;
	private float[] terrainWidths;

	public ObjectPooler[] objectPools;

	private float minHeight;
	public Transform maxHeightPoint;
	private float maxHeight;
	public float maxHeightChange;
	private float heightChange;

	public PlayerController playerController;

	public float powerupHeight;
	public ObjectPooler[] powerupPools;
	private int powerupSelector;
	public float[] powerupThresholds;

	public ObjectPooler[] obstaclePools;
	public float[] obstacleThresholds;
	private int obstacleSelector;

	private CoinGenerator coinGenerator;
	public float randomCoinThreshold;
	public float coinHeight;

	// Use this for initialization
	void Start () {
		// terrainWidth = terrain.GetComponent<BoxCollider2D>().size.x;
		terrainWidths = new float[objectPools.Length];

		for(int i = 0; i < objectPools.Length; i++) {
			terrainWidths[i] = objectPools[i].pooledObject.GetComponent<BoxCollider2D>().size.x;
		}

		minHeight = transform.position.y;
		maxHeight = maxHeightPoint.position.y;

		coinGenerator = FindObjectOfType<CoinGenerator>();
		
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.x < generationPoint.position.x) {
			distance = Random.Range (distanceMin, distanceMax);
			terrainSelector = Random.Range(0, objectPools.Length);
			
			heightChange = transform.position.y + Random.Range(maxHeightChange, -maxHeightChange);

			if(heightChange > maxHeight) {
				heightChange = maxHeight;
			} else if (heightChange < minHeight) {
				heightChange = minHeight;
			}

			if(playerController.hasJetPack) {
				distance = 0;
				heightChange = minHeight;
				terrainSelector = 0;
			}

			powerupSelector = Random.Range(0, powerupPools.Length);

			if(Random.Range(0f, 100f) < powerupThresholds[powerupSelector]) {
				GameObject newPowerup = powerupPools[powerupSelector].GetPooledObject();
				newPowerup.transform.position = transform.position + new Vector3(distance / 2f, Random.Range(powerupHeight / 2f, powerupHeight), 0f);
				newPowerup.SetActive(true);
			}

			transform.position = new Vector3(transform.position.x + (terrainWidths[terrainSelector] / 2) + distance, heightChange, transform.position.z);

			// Instantiate (terrainArray[terrainSelector], transform.position, transform.rotation);
			GameObject newTerrain = objectPools[terrainSelector].GetPooledObject();

			newTerrain.transform.position = transform.position;
			newTerrain.transform.rotation = transform.rotation;
			newTerrain.SetActive(true);

			if(Random.Range(0f,100f) < randomCoinThreshold)
				coinGenerator.SpawnCoins(new Vector3(transform.position.x, transform.position.y+coinHeight, transform.position.z));

			obstacleSelector = Random.Range(0, obstacleThresholds.Length);

			if(Random.Range(0f, 100f) < obstacleThresholds[obstacleSelector] && !playerController.hasJetPack) {
				GameObject newObstacle = obstaclePools[obstacleSelector].GetPooledObject();
				float obstacleXPosition = Random.Range(-terrainWidths[terrainSelector] / 2f + 1f, terrainWidths[terrainSelector] / 2f - 1f);
				
				Vector3 obstaclePosition = new Vector3(obstacleXPosition, 1.5f, 0f);

				newObstacle.transform.position = transform.position + obstaclePosition;
				newObstacle.transform.rotation = transform.rotation;
				newObstacle.SetActive(true);
			}

			

			transform.position = new Vector3(transform.position.x + (terrainWidths[terrainSelector] / 2), transform.position.y, transform.position.z);
		}
	}
}
