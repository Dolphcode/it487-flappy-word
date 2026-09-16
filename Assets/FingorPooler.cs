using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FingorPooler : MonoBehaviour
{
    [SerializeField] private GameObject fingor;
    [SerializeField] private int poolSize;
    [SerializeField] private float timeBetweenSpawns = 2f;
    [SerializeField] private float minHeight = -40f;
    [SerializeField] private float maxHeight = 0f;
    private List<FingorMover> inactiveFingors;
    private List<FingorMover> activeFingors;

    private float timer = 0f;
    private bool readyToSpawn = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inactiveFingors = new();
        activeFingors = new();
        StartCoroutine(SpawnFingors());
    }

    // Update is called once per frame
    void Update()
    {
        if (readyToSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= timeBetweenSpawns)
            {
                FingorMover newFingor;
                if (inactiveFingors.Count > 0)
                {
                    newFingor = inactiveFingors[0];
                    inactiveFingors.RemoveAt(0);
                }
                else
                {
                    newFingor = Instantiate<GameObject>(fingor).GetComponent<FingorMover>();
                    newFingor.rb = newFingor.GetComponent<Rigidbody2D>();
                }
                
                activeFingors.Add(newFingor);
                newFingor.isActiveFingor = true;
                newFingor.rb.position = new Vector2(transform.position.x, Random.Range(minHeight, maxHeight));
                timer = 0f;
            }
        }
    }

    public void ResetFingor(FingorMover fingor)
    {
        if (activeFingors.Contains(fingor))
        {
            activeFingors.Remove(fingor);
            inactiveFingors.Add(fingor);
            fingor.isActiveFingor = false;
        }
    }
    
    IEnumerator SpawnFingors()
    {
        AsyncInstantiateOperation op = InstantiateAsync(fingor, poolSize, transform.position, Quaternion.identity);
        yield return op;
        
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = op.Result[i] as GameObject;
            inactiveFingors.Add(obj.GetComponent<FingorMover>());
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        readyToSpawn = true;
    }
}
