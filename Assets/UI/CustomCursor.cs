using System;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    
    static CustomCursor instance;
    public static CustomCursor GetInstance() => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public Texture2D cursorTexture;
        public Vector2 hotSpot = Vector2.zero; // Punto de clic (ej. centro para una mira)
        public CursorMode cursorMode = CursorMode.Auto;
    
        void Start()
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
}
