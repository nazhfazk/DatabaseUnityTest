using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject characterContainer;
    public GameObject planeObject;
    public float pollingInterval = 1f;

    private bool isPolling = false;

    private void Start()
    {
        Debug.Log("CharacterSpawner Start method called.");
        StartCoroutine(PollForData());
    }

    private IEnumerator PollForData()
    {
        while (true)
        {
            if (!isPolling)
            {
                isPolling = true;

                Debug.Log("Fetching data from PHP script...");
                UnityWebRequest request = UnityWebRequest.Get("http://localhost/Unity/KonekDB/quisioner.php");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string response = request.downloadHandler.text;

                    // Log the retrieved data
                    Debug.Log("Data Retrieved: " + response);

                    // Parse JSON data
                    CharacterData[] characterData = JsonUtility.FromJson<CharacterData[]>(response);

                    foreach (CharacterData data in characterData)
                    {
                        Debug.Log("Character Data - Username: " + data.username + ", Gender: " + data.gender + ", Age: " + data.age);
                        Debug.Log("Spawning character for " + data.username + " (Gender: " + data.gender + ", Age: " + data.age + ")");
                        SpawnCharacter(data.username, data.gender, data.age);
                    }
                }
                else
                {
                    Debug.LogError("Failed to retrieve data: " + request.error);
                }

                isPolling = false;
            }

            yield return new WaitForSeconds(pollingInterval);
        }
    }

    private void SpawnCharacter(string username, string gender, int age)
    {
        GameObject characterPrefab = null;
        string characterPrefabPath = "Resources/";

        switch (gender)
        {
            case "male":
                if (age == 20)
                {
                    characterPrefab = Resources.Load<GameObject>(characterPrefabPath + "Naruto");
                }
                else if (age >= 20 && age <= 30)
                {
                    characterPrefab = Resources.Load<GameObject>(characterPrefabPath + "Sasuke");
                }
                break;

            case "female":
                if (age == 20)
                {
                    characterPrefab = Resources.Load<GameObject>(characterPrefabPath + "Hinata");
                }
                else if (age >= 20 && age <= 30)
                {
                    characterPrefab = Resources.Load<GameObject>(characterPrefabPath + "Sakura");
                }
                break;

        }

        if (characterPrefab != null)
        {
            float minX = planeObject.transform.position.x - planeObject.transform.localScale.x / 2;
            float maxX = planeObject.transform.position.x + planeObject.transform.localScale.x / 2;
            float minZ = planeObject.transform.position.z - planeObject.transform.localScale.z / 2;
            float maxZ = planeObject.transform.position.z + planeObject.transform.localScale.z / 2;

            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), 0.0f, Random.Range(minZ, maxZ));

            GameObject spawnedCharacter = Instantiate(characterPrefab, spawnPosition, Quaternion.identity, characterContainer.transform);
            TextMesh usernameText = spawnedCharacter.GetComponentInChildren<TextMesh>();

            if (usernameText != null)
            {
                usernameText.text = username;
            }
            Debug.Log("Character spawned for " + username);
        }
    }
}

[System.Serializable]
public class CharacterData
{
    public string username;
    public string gender;
    public int age;
}