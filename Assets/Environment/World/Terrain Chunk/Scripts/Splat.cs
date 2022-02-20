using Array2DExtensions;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public static class Splat
{
    /// <summary>
    /// Generate SplatMap for terrain.terrainData.SetAlphamaps.
    /// </summary>
    /// 
    /// </summary>
    /// <param name="splatSettings">The splatmap generation setting of each Terrain Layer in order</param>
    /// <param name="heightMap"></param>
    /// <returns></returns>
    //Modified from https://gist.github.com/Andros-Spica/d64c35a0bafd0be56947a4a99b7c73c7
    public static float[,,] GenerateSplatMap(SplatSetting[] splatSettings, float[,] heightMap, float heightScale, int heightmapResolution, int splatmapResolution, int terrainLayersLength)
    {
        if (terrainLayersLength > splatSettings.Length)
        {
            Debug.LogError("splatSettings needs to be greater or equal then terrainData.terrainLayers");
        }

        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[,,] splatmapWeights = new float[splatmapResolution, splatmapResolution, terrainLayersLength];


        for (int y = 0; y < splatmapResolution; y++)
        {
            for (int x = 0; x < splatmapResolution; x++)
            {
                // splatmapResolution x/y coordinates to heightMap range by linear conversion
                // NOTE: x = y, y = x. what the f...? 
                var heightMap_x = y * (heightmapResolution) / splatmapResolution;
                var heightMap_y = x * (heightmapResolution) / splatmapResolution;

                // get height and slope at corresponding point
                float height = heightMap[heightMap_x, heightMap_y];
                float slope = heightMap.GetSteepness(heightMap_y - y / splatmapResolution, heightMap_x - x / splatmapResolution, heightScale);

                // The point sum of all layers weight to be used in noramlizing later
                var sum = 0f;

                for (int i = 0; i < terrainLayersLength; i++)
                {
                    //Shortcuts
                    var layer = splatSettings[i];
                    var heightSettings = layer.heightSettings;
                    var slopeSettings = layer.slopeSettings;

                    //baseStrength
                    splatmapWeights[y, x, i] = layer.baseStrength;

                    //Fade in or fade out the layer 
                    var layerHeight = heightSettings.heightFadeSetting.Fade(height);

                    //Simply add heightStrength
                    layerHeight = layerHeight * heightSettings.heightStrength;

                    //How much will height effect the layer
                    splatmapWeights[y, x, i] *= math.lerp(1 - heightSettings.heightEffect, 1, layerHeight);

                    //Invert it. Use for ex: cliff texture
                    var layerSlope = slopeSettings.slopeInverted ? 1 - slope : slope;
                    layerSlope *= slopeSettings.slopeFadeSetting.Fade(layerSlope);

                    //Simply add slopeStrength
                    layerSlope *= slopeSettings.slopeStrength;

                    //How much will slope effect the layer
                    splatmapWeights[y, x, i] *= math.lerp(1 - slopeSettings.slopeEffect, 1, layerSlope);
                    sum += splatmapWeights[y, x, i];
                }

                for (int i = 0; i < terrainLayersLength; i++)
                {
                    // noramlizing for the sum to be 1
                    splatmapWeights[y, x, i] /= sum;
                }
            }
        }
        return splatmapWeights;
    }
}
/// <summary>
/// The splatmap generation setting of a Terrain Layer
/// </summary>
[System.Serializable]
public class SplatSetting
{
    [Range(0, 1), Tooltip("how much the layer weight will contribute to the splatmap")]
    public float baseStrength = 1;

    public HeightSettings heightSettings;
    public SlopeSettings slopeSettings;
}
[System.Serializable]
public class HeightSettings
{
    public float heightStrength = 1;
    [Range(0, 1), Tooltip("how much height will contribute to the layer weight")]
    public float heightEffect = 1; //if heightEffect is equals 0 the height would have no effect and vice versa
    public FadeSetting heightFadeSetting;
}
[System.Serializable]
public class SlopeSettings
{
    public float slopeStrength = 1;
    [Range(0, 1), Tooltip("how much slope will contribute to the layer weight")]
    public float slopeEffect = 1; //if slopeEffect is equals 0 the slope would have no effect and vice versa
    public FadeSetting slopeFadeSetting;
    public bool slopeInverted = true;
}
[System.Serializable]
public class FadeSetting
{
    [Range(0, 1)]
    public float startFade = 0;
    [Range(0, 1)]
    public float startValue = 0;
    [Range(0, 1)]
    public float endValue = 1;
    [Range(0, 1)]
    public float endFade = 1;
    /// <summary>
    /// fadein and fadeout 0 to 1 output.
    /// </summary>
    public float Fade(float value)
    {
        return Fade(value, startFade, startValue, endValue, endFade);
    }
    /// <summary>
    /// fadein and fadeout 0 to 1 output.
    /// </summary>
    public static float Fade(float value, float startFade, float startValue, float endValue, float endFade)
    {
        return
            value < startValue ? math.max((value - startFade) / (startValue - startFade), 0) :
            value > endValue ? math.max((value - endFade) / (endValue - endFade), 0) :
            1
        ;
    }
}
