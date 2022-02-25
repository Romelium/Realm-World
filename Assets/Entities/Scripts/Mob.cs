using UnityEngine;

[System.Serializable]
public class Mob
{
    [Tooltip("The object that instantiate on spawn.")]
    public GameObject mob;
    [Range(0, 1), Tooltip("The mob rarity to try to spawn.\n\n0: 0% chance to try to spawn.\n1: 100% chance to try to spawn.")]
    public float chance = 1;
    [Tooltip("The mob point where mob try to spawn over the height but not under.\n\n0: Can try to spawn over 0 units but not under.\n100: Can try to spawn over 100 units but not under.")]
    public float heightCutOff = 0;
    [Range(0, 1), Tooltip("The mob steepness where to spawn.\n\n0: Can try to spawn in any steepness.\n\n0.9: Can only try to spawn in very flat ground.")]
    public float allowedSteepness = 0.5f;
}