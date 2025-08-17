using System.Collections.Generic;

class ProgramExecutor
{
    // ----- VARIABLES -----
    private readonly BotController bot;
    private readonly Stack<ExecutionContext> executionStack = new();

    // ----- CONSTRUCTOR -----
    public ProgramExecutor(BotController bot, List<ProgramBlock> program)
    {
        this.bot = bot;
        if (program == null || program.Count == 0)
        {
            return;
        }
        executionStack.Push(new ExecutionContext(program));
    }

    // ----- METHODS -----
    public void ExecuteNextStep()
    {
        // Program finished.
        if (executionStack.Count == 0)
        {
            return;
        }

        bool stepFinished = false;
        while (!stepFinished)
        {
            ExecutionContext currentContext = executionStack.Peek();

            if (currentContext.programCounter >= currentContext.commands.Count)
            {
                // Execution finished, pop the context.
                executionStack.Pop();
                // Need to re-push if it's a unfinished for loop.
                if (currentContext.iterationsLeft > 1)
                {
                    currentContext.iterationsLeft--;
                    currentContext.programCounter = 0;
                    executionStack.Push(currentContext);
                }
            }

            ProgramBlock command = currentContext.commands[currentContext.programCounter];
            currentContext.programCounter++;

            if (command is MoveBlock move)
            {
                bot.ProcessMove(move.direction);
                stepFinished = true;
            }
            else if (command is SleepBlock)
            {
                bot.ProcessSleep();
                stepFinished = true;
            }
            else if (command is LoopBlock loopBlock)
            {
                if (loopBlock.commands.Count > 0 && loopBlock.times > 0)
                {
                    executionStack.Push(new ExecutionContext(loopBlock));
                }
            }
            else
            {
                break;
            }
        }
    }
}
