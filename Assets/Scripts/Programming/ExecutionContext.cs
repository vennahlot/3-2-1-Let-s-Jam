using System.Collections.Generic;

public class ExecutionContext
{
    // ----- PROGRAM -----
    public readonly List<ProgramBlock> commands;
    public int programCounter = 0;

    // ----- FOR LOOP -----
    public readonly int totalIterations;
    public int iterationsLeft;

    // ----- CONSTRUCTORS -----
    public ExecutionContext(LoopBlock forBlock)
    {
        commands = forBlock.commands;
        totalIterations = forBlock.times;
        iterationsLeft = forBlock.times;
    }

    public ExecutionContext(List<ProgramBlock> mainProgram)
    {
        commands = mainProgram;
        totalIterations = 1;
        iterationsLeft = 1;
    }
}
