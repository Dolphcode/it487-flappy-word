using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Flappy : MonoBehaviour
{
    [SerializeField] private WordBuffer wordBuffer;
    [SerializeField] private float jumpVelocity = 50f;
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip death;
    
    private Rigidbody2D rb;
    [SerializeField] private string wordText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDestroy()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnEnable()
    {
        Keyboard.current.onTextInput += OnFap;
    }

    private void OnDisable()
    {
        Keyboard.current.onTextInput -= OnFap;
    }

    private char next = 'a';
    private string nextStr = string.Empty;
    private int indexInStr = 0;
    private bool queueJump = false;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (nextStr == string.Empty) nextStr = wordBuffer.GetNextString();
        if (queueJump)
        {
            rb.linearVelocity = Vector2.up * jumpVelocity;
            queueJump = false;
        }
    }

    void Update()
    {
        wordText = $"<color=white>{nextStr.Substring(0, indexInStr)}</color><color=black>{nextStr.Substring(indexInStr)}</color>";

    }

    void OnFap(char ch)
    {
        if (Char.ToLower(ch) == nextStr[indexInStr])
        {
            indexInStr++;
            if (indexInStr >= nextStr.Length)
            {
                nextStr = wordBuffer.GetNextString();
                indexInStr = 0;
            }
            queueJump = true;
        } 
    }

    void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
    {
        root.dataSource = this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Fingor")
        {
            Debug.Log("Fingorrrr");
            audioSource.PlayOneShot(death);
            audioSource.time = 0.8f;
        }
    }
}
