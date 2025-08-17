using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BotMoveDirection
{
    Up,
    Down,
    Left,
    Right
}


public class BotController : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private GameObject selectIndicator;
    [SerializeField] private GameObject programSlot;
    [SerializeField] private GameObject programInstruction;
    private ProgramExecutor executor;
    private SpriteRenderer spriteRenderer;
    private Vector2 originalPosition;

    // ----- UNITY LIFE CYCLE -----

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        originalPosition = transform.position;
    }

    void OnEnable()
    {
        SequenceManager.OnPrepareStarted += PrepareExecution;
        SequenceManager.OnSequenceStep += ExecuteOneStep;
    }

    void OnDisable()
    {
        SequenceManager.OnPrepareStarted -= PrepareExecution;
        SequenceManager.OnSequenceStep -= ExecuteOneStep;
    }

    // ----- Event Handlers -----
    
    void PrepareExecution()
    {
        transform.position = originalPosition;
        LoadProgram();
    }

    void ExecuteOneStep(int step, bool isBeat)
    {
        spriteRenderer.sprite = sprites[step % sprites.Count];
        executor?.ExecuteNextStep();
    }

    // ----- Program Actions -----

    public void ProcessMove(BotMoveDirection direction)
    {
        switch (direction)
        {
            case BotMoveDirection.Up:
                transform.position += Vector3.up;
                break;
            case BotMoveDirection.Down:
                transform.position += Vector3.down;
                break;
            case BotMoveDirection.Left:
                transform.position += Vector3.left;
                break;
            case BotMoveDirection.Right:
                transform.position += Vector3.right;
                break;
            default:
                break;
        }
    }

    public void ProcessSleep()
    {
        Debug.Log("Bot sleeping...");
    }

    // ----- Adapters -----

    public void LoadProgram()
    {
        List<ProgramBlock> program = new();
        LoadProgramRecursive(programSlot.transform, program);
        executor = new ProgramExecutor(this, program);
        Debug.Log("Program loaded.");
    }

    private void LoadProgramRecursive(Transform parentTransform, List<ProgramBlock> program)
    {
        foreach (Transform child in parentTransform)
        {
            print(child.name);
            DraggableItem item = child.GetComponent<DraggableItem>();
            if (item != null)
            {
                switch (item.blockType)
                {
                    case BlockType.Move:
                        program.Add(item.programBlock);
                        break;
                    case BlockType.Sleep:
                        program.Add(item.programBlock);
                        break;
                    case BlockType.Loop:
                        LoopBlock loopBlock = item.programBlock as LoopBlock;
                        List<ProgramBlock> loopProgram = new();
                        LoadProgramRecursive(child, loopProgram);
                        loopBlock.commands = loopProgram;
                        program.Add(loopBlock);
                        break;
                    default:
                        break;
                }
            }
            LoadProgramRecursive(child, program);
        }
    }

    public void HandleSelected()
    {
        selectIndicator.SetActive(true);
        programInstruction.SetActive(false);
        programSlot.SetActive(true);
    }

    public void HandleDeselected()
    {
        selectIndicator.SetActive(false);
        programInstruction.SetActive(true);
        programSlot.SetActive(false);
    }
    
}
