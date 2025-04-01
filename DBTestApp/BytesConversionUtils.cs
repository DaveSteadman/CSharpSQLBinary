using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

public static class BytesConversionUtils
{
    // --------------------------------------------------------------------------------------------
    // MARK: - Standard Types
    // --------------------------------------------------------------------------------------------

    // Integer Conversions
    public static byte[] IntToBytes(int value)    => BitConverter.GetBytes(value);
    public static int    BytesToInt(byte[] bytes) => BitConverter.ToInt32(bytes, 0);

    // Double Conversions
    // bytes[] b = BytesConversionUtils.DoubleToBytes(3.14159);
    public static byte[] DoubleToBytes(double value) => BitConverter.GetBytes(value);
    public static double BytesToDouble(byte[] bytes) => BitConverter.ToDouble(bytes, 0);

    // Boolean Conversions
    public static byte[] BoolToBytes(bool value)   => BitConverter.GetBytes(value);
    public static bool   BytesToBool(byte[] bytes) => BitConverter.ToBoolean(bytes, 0);

    // String Conversions
    public static byte[] StringToBytes(string value) => Encoding.UTF8.GetBytes(value);
    public static string BytesToString(byte[] bytes) => Encoding.UTF8.GetString(bytes);

    // DateTime Conversions
    public static byte[]   DateTimeToBytes(DateTime value) => BitConverter.GetBytes(value.ToBinary());
    public static DateTime BytesToDateTime(byte[] bytes)   => DateTime.FromBinary(BitConverter.ToInt64(bytes, 0));

    // Float Conversions
    public static byte[] FloatToBytes(float value)  => BitConverter.GetBytes(value);
    public static float  BytesToFloat(byte[] bytes) => BitConverter.ToSingle(bytes, 0);

    // Long Conversions
    public static byte[] LongToBytes(long value) => BitConverter.GetBytes(value);
    public static long   BytesToLong(byte[] bytes) => BitConverter.ToInt64(bytes, 0);

    // --------------------------------------------------------------------------------------------
    // MARK: - Basic Collections
    // --------------------------------------------------------------------------------------------

    public static byte[]       ListToBytes(List<string> list)  => JsonSerializer.SerializeToUtf8Bytes(list);
    public static List<string> BytesToList(byte[] bytes)       => JsonSerializer.Deserialize<List<string>>(bytes) ?? new List<string>();

    public static byte[]                     DictionaryToBytes(Dictionary<string, string> dictionary) => JsonSerializer.SerializeToUtf8Bytes(dictionary);
    public static Dictionary<string, string> BytesToDictionary(byte[] bytes)                          => JsonSerializer.Deserialize<Dictionary<string, string>>(bytes) ?? new Dictionary<string, string>();

}
