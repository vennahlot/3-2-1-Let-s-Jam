using System.Collections.Generic;

public class LoopBlock : ProgramBlock
{
    public int times;
    public List<ProgramBlock> commands = new();
    
    // ----- CONSTRUCTOR -----
    public LoopBlock(int t) { times = t; }
}
