using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum BlockType
{
    Move,
    Sleep,
    Loop
}

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isTemplate = false;

    // ----- Placeholder -----
    private GameObject placeholder = null;
    private Transform placeholderParent = null;

    private CanvasGroup canvasGroup;
    private RectTransform rt;
    private LayoutElement le;

    // ----- Program Block -----
    public BlockType blockType = BlockType.Move;
    public ProgramBlock programBlock;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
        le = GetComponent<LayoutElement>();
        switch (blockType)
        {
            case BlockType.Move:
                programBlock = new MoveBlock(BotMoveDirection.Up);
                break;
            case BlockType.Sleep:
                programBlock = new SleepBlock();
                break;
            case BlockType.Loop:
                programBlock = new LoopBlock(2);
                break;
            default:
                break;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CreatePlaceholder();

        // ----- Handle template -----
        if (isTemplate)
        {
            // If is template, create a new instance for dragging.
            GameObject newBlock = Instantiate(gameObject, transform.root);
            DraggableItem newItemDrag = newBlock.GetComponent<DraggableItem>();
            newItemDrag.isTemplate = false;
            newItemDrag.canvasGroup.blocksRaycasts = false;
            newItemDrag.placeholder = placeholder;
            // Hand over the new insstance to drag control.
            eventData.pointerDrag = newBlock;
            Debug.Log("Begin drag on template");
            return;  // Template will not involve in dragging.
        }

        // Set the parent to root so it can freely move on top of everything.
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        Debug.Log("Begin drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isTemplate) { return; }

        // Element follow mouse.
        transform.position = eventData.position;

        // Get the slot currently under mouse raycast, and set it as parent.
        DropSlot newParentSlot = GetDropSlot(eventData);
        placeholderParent = newParentSlot != null ? newParentSlot.transform : null;

        if (placeholderParent == null)
        {
            return;
        }

        if (placeholder.transform.parent != placeholderParent)
        {
            placeholder.transform.SetParent(placeholderParent);
        }

        // Determine and set the new sibling index.
        int newSiblingIndex = placeholderParent.childCount;
        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            // Do not compare with self.
            if (placeholderParent.GetChild(i) == placeholder.transform) { continue; }
            // If mouse y higher than child y, insert to its front.
            if (eventData.position.y > placeholderParent.GetChild(i).transform.position.y)
            {
                newSiblingIndex = i;
                break;
            }
        }
        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isTemplate) { return; }

        // If the block is not inside a slot, destroy it.
        if (placeholderParent == null)
        {
            Destroy(placeholder);
            Destroy(gameObject);
            return;
        }

        if (placeholderParent != transform.root)
        {
            transform.SetParent(placeholderParent);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        }

        Destroy(placeholder);
        placeholder = null;

        canvasGroup.blocksRaycasts = true;
    }

    private void CreatePlaceholder()
    {
        // Create placeholder.
        placeholder = new GameObject("DragPlaceholder");
        placeholder.transform.SetParent(isTemplate ? transform.root : transform.parent);
        placeholder.transform.SetSiblingIndex(isTemplate ? 0 : transform.GetSiblingIndex());
        // Set placeholder size to be identical as the dragged element.
        RectTransform placeholderRt = placeholder.AddComponent<RectTransform>();
        LayoutElement placeholderLe = placeholder.AddComponent<LayoutElement>();
        placeholderRt.sizeDelta = rt.sizeDelta;
        placeholderLe.preferredHeight = le.preferredHeight;
        placeholderLe.preferredWidth = le.preferredWidth;
    }

    private DropSlot GetDropSlot(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null) { return null; }
        DropSlot dropSlot = eventData.pointerEnter.GetComponentInParent<DropSlot>();
        if (dropSlot == null || dropSlot.dropDisabled) { return null; }
        return dropSlot;
    }

    public void SetMoveDirection(int directionIdx)
    {
        Debug.Log("Set move direction: " + directionIdx);
        MoveBlock moveBlock = programBlock as MoveBlock;
        switch (directionIdx)
        {
            case 0:
                moveBlock.direction = BotMoveDirection.Up;
                break;
            case 1:
                moveBlock.direction = BotMoveDirection.Down;
                break;
            case 2:
                moveBlock.direction = BotMoveDirection.Left;
                break;
            case 3:
                moveBlock.direction = BotMoveDirection.Right;
                break;
            default:
                break;
        }
    }
    
    public void SetLoopTimes(string timesStr)
    {
        Debug.Log("Set loop times: " + timesStr);
        LoopBlock loopBlock = programBlock as LoopBlock;
        loopBlock.times = string.IsNullOrEmpty(timesStr) ? 1 : int.Parse(timesStr);
    }
}
