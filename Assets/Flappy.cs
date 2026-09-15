using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Flappy : MonoBehaviour
{
    private Rigidbody2D rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
    private bool queueJump = false;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (queueJump)
        {
            rb.AddForce(Vector2.up * 300f * Time.fixedDeltaTime);
            queueJump = false;
        }
    }

    void OnFap(char ch)
    {
        if (Char.ToLower(ch) == next)
        {
            next = (char)(Random.Range(0, 26) + 97);
            Debug.Log($"Next char is {next}");
            queueJump = true;
        } 
    }
}
