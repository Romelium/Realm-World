using System;
using Array2DExtensions;
using UnityEngine;
[Serializable]
public class TreePlanter
{
    public bool lockWidthToHeight = true;
    public bool randomRotation = true;
    public bool allowHeightVar = true;
    public bool allowWidthVar = true;
    public int numberOfTrees = 10;
    public float heightCutOff = 0;
    public float treeColorAdjustment = .4f;
    public float treeHeight = 1;
    public float treeHeightVariation = .1f;
    public float treeWidth = 1;
    public float treeWidthVariation = .1f;
    public float allowedSteepness = 0.5f;

    public TreeInstance[] GenerateInstances(float[,] heightMap, int heightmapResolution, float treePrototypesLength, Vector3 size, Func<float> rng_01)
    {
        if (treePrototypesLength == 0)
        {
            Debug.LogError("Can't place trees because no prototypes are defined");
            return null;
        }

        TreeInstance[] instances = new TreeInstance[numberOfTrees];
        float scale = size.y * heightmapResolution / (size.x + size.z / 2);
        int i = 0;
        while (i < numberOfTrees)
        {
            TreeInstance instance = new TreeInstance();
            float x = rng_01(), y = rng_01();
            instance.position = new Vector3(x, heightMap.GetValue(y, x), y);
            if (heightMap.GetSteepness(instance.position.x, instance.position.z, scale) > allowedSteepness && instance.position.y > heightCutOff)
            {
                instance.color = Color.white;
                instance.lightmapColor = Color.white;
                instance.prototypeIndex = (int)(rng_01() * treePrototypesLength);

                var v = allowHeightVar ? treeHeightVariation : 0.0f;
                instance.heightScale = treeHeight * rng_01() * 2 * v + 1 - v;
                v = allowWidthVar ? treeWidthVariation : 0.0f;
                instance.widthScale = lockWidthToHeight ? instance.heightScale : treeWidth * rng_01() * 2 * v + 1 - v;

                instance.rotation = randomRotation ? rng_01() * 2 * Mathf.PI : 0f;
            }
            instances[i++] = instance;
        }
        return instances;
    }
}