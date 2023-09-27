using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] GameObject Balls;
    [SerializeField] float minX = -5f;
    [SerializeField] float maxX = 5f;
    [SerializeField] float minY = -5f;
    [SerializeField] float maxY = 5f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float maxBounceForce = 2f;
    [SerializeField] float minStopDuration = 5f;
    [SerializeField] float maxStopDuration = 10f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Vector3> currentVelocities = new List<Vector3>();
    private List<float> stopTimers = new List<float>();
    private List<bool> isMovingList = new List<bool>();

    public void SpawnRandomObjectAndMove()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

        GameObject newObject = Instantiate(Balls, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(newObject);
        currentVelocities.Add(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized * moveSpeed);
        stopTimers.Add(Random.Range(minStopDuration, maxStopDuration)); 
        isMovingList.Add(true); 

        Rigidbody rb = newObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (isMovingList[i])
            {
                MoveObject(i);
            }
            else
            {
                stopTimers[i] -= Time.deltaTime;

               
                if (stopTimers[i] <= 0f)
                {
                    isMovingList[i] = true;
                }
            }
        }
    }

    private void MoveObject(int index)
    {
        Vector3 newPosition = spawnedObjects[index].transform.position + currentVelocities[index] * Time.deltaTime;

        if (newPosition.x > maxX || newPosition.x < minX)
        {
            currentVelocities[index] = new Vector3(-currentVelocities[index].x, currentVelocities[index].y, currentVelocities[index].z);
            currentVelocities[index] += RandomBounceForce();
        }

        if (newPosition.y > maxY || newPosition.y < minY)
        {
            currentVelocities[index] = new Vector3(currentVelocities[index].x, -currentVelocities[index].y, currentVelocities[index].z);
            currentVelocities[index] += RandomBounceForce();
        }

        spawnedObjects[index].transform.position = newPosition;

        if (Random.value < 0.001) 
        {
            isMovingList[index] = false;
            stopTimers[index] = Random.Range(minStopDuration, maxStopDuration);
        }
    }

    private Vector3 RandomBounceForce()
    {
        return new Vector3(Random.Range(-maxBounceForce, maxBounceForce), Random.Range(-maxBounceForce, maxBounceForce), 0f);
    }
}