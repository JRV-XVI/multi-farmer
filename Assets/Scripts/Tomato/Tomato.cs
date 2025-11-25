using UnityEditor.ShaderGraph;
using UnityEngine;

public class Tomato : MonoBehaviour
{

    private StateMachine<Tomato> _stateMachine;

    private Rigidbody _rb;
    private BoxCollider _boxCollider;
    private Renderer _renderer;

    public bool isTaken;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
    }

    

    //Funciones del Tomato

    public void ChangeColor(Color newColor)
    {
        _renderer.material.color = newColor;
    }
}
