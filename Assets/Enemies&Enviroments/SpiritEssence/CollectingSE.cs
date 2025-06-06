using UnityEngine;

public class CollectingSE : MonoBehaviour
{
    public bool findPlayer;
    public Vector3 targetPos;

    [SerializeField] float speed;

    void Start()
    {
        findPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (findPlayer) { ToThePlayer(); }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            findPlayer = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            findPlayer = true;
            targetPos = other.transform.position;
        }
    }

    void ToThePlayer()
    {
        if (targetPos!=transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            gameObject.GetComponent<Animator>().SetTrigger("FindPlayer");
        }
        if (targetPos == transform.position)
        {
            FindObjectOfType<InGameCanvasController>().UpdateSECount(5);
            Destroy(gameObject);
        }
    }
}
