using UnityEngine;

public class FingorResetter : MonoBehaviour
{
    [SerializeField] private FingorPooler pooler;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Fingor")
        {
            FingorMover fingor = other.GetComponent<FingorMover>();
            pooler.ResetFingor(fingor);
        }
    }
}
