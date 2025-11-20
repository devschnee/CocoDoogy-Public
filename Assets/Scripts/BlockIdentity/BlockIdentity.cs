using UnityEngine;
public enum BlockType
{
    Normal, Slope, Water, FlowWater, Box, Switch, Turret, Tower, Door, Ironball, Hog, Tortoise, Buffalo, Start, End, Treasure, Dialogue, Prop
}

public class BlockIdentity : MonoBehaviour
{
    
    public string blockName;
    public BlockType blockType;
}

public class StartIdentity : BlockIdentity
{

}

public class EndIdentity : BlockIdentity
{

}


