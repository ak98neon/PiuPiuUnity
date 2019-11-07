using UnityEngine;

public class PlayerObserver : MonoBehaviour
{
    private MultiListener listener;

    // Use this for initialization
    void Start()
    {
        listener = GameObject.FindGameObjectWithTag(MultiListener.respawnTag).GetComponent<MultiListener>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            listener.handleEvent(transform.position, transform.rotation, ClientAction.MOVE);
        }
    }
}
