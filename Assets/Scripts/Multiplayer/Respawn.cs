using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    IDictionary<string, GameObject> clients;
    
    void Start()
    {
        clients = new Dictionary<string, GameObject>();
    }

    public void addClient(string id, Vector3 pos, Quaternion rot, GameObject objPrefab)
    {
        GameObject gameObject = Instantiate(objPrefab, pos, rot) as GameObject;
        gameObject.transform.rotation = rot;
        clients.Add(id, gameObject);
    }

    public void removeClient(string id)
    {
        Destroy(clients[id]);
        clients.Remove(id);
    }

    public void moveClient(string id, Vector3 pos, Quaternion rot)
    {
        GameObject gameObject = clients[id];
        gameObject.transform.position = pos;
        gameObject.transform.rotation = rot;
    }
}
