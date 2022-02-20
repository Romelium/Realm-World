using System;

public static class Bitcast
{
    public static uint IntToUInt(int value)
    {
        return BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
    }
    public static float IntToFloat(int value)
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
    }
    public static int UIntToInt(uint value)
    {
        return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
    }
    public static float UIntToFloat(uint value)
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
    }
    public static uint FloatToUInt(float value)
    {
        return BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
    }
    public static int FloatToInt(float value)
    {
        return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
    }
}