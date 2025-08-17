using UnityEngine;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
    // ----- Singleton -----
    public static ControlManager Instance { get; private set; }

    // ----- Cursor -----
    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] Texture2D defaultCursor;
    private readonly Vector2 hotspot = Vector2.zero;
    private readonly CursorMode cursorMode = CursorMode.Auto;
    
    // ----- States -----
    private Vector2 pointerPos;
    private GameObject hoveredGameObject = null;
    private GameObject selectedGameObject = null;

    // ----- References -----
    private InputSystem_Actions inputActions;

    // ----- Unity Lifecycle -----

    void Awake()
    {
        // Create singleton.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);            
        }
        // Initialize input system and subscribe to events.
        inputActions = new InputSystem_Actions();
    }
    
    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.PointerPosition.performed += OnPointerPosition;
        inputActions.Player.LeftClick.performed += OnClick;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.PointerPosition.performed -= OnPointerPosition;
        inputActions.Player.LeftClick.performed -= OnClick;
    }

    // ----- Event Handlers -----

    void OnPointerPosition(InputAction.CallbackContext ctx)
    {
        pointerPos = ctx.ReadValue<Vector2>();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(pointerPos);
        RaycastHit2D hit = Physics2D.Raycast(
            worldPos,
            Vector2.zero,
            Mathf.Infinity,
            LayerMask.GetMask("Interactable")
        );

        if (hit.collider != null)
        {
            if (hoveredGameObject != hit.collider.gameObject)
            {
                hoveredGameObject = hit.collider.gameObject;
                Cursor.SetCursor(hoverCursor, hotspot, cursorMode);
            }
        }
        else
        {
            if (hoveredGameObject != null)
            {
                hoveredGameObject = null;
                Cursor.SetCursor(defaultCursor, hotspot, cursorMode);
            }
        }
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(pointerPos);
        RaycastHit2D hit = Physics2D.Raycast(
            worldPos,
            Vector2.zero,
            Mathf.Infinity,
            LayerMask.GetMask("Interactable")
        );

        if (hit.collider != null)
        {
            if (selectedGameObject != null && selectedGameObject != hit.collider.gameObject)
            {
                selectedGameObject.GetComponent<BotController>().HandleDeselected();
            }
            selectedGameObject = hit.collider.gameObject;
            selectedGameObject.GetComponent<BotController>().HandleSelected();
            Debug.Log("Selected object: " + selectedGameObject.name);
        }
    }
}
