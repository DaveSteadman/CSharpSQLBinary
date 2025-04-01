using System;
using System.Collections.Generic;

public class ByteArrayWriter
{
    private readonly List<byte> _data;

    public ByteArrayWriter()
    {
        _data = new List<byte>();
    }

    public byte[] ToArray() => _data.ToArray();
    public void WriteInt(int value)       { _data.AddRange(BitConverter.GetBytes(value)); }
    public void WriteFloat(float value)   { _data.AddRange(BitConverter.GetBytes(value)); }
    public void WriteDouble(double value) { _data.AddRange(BitConverter.GetBytes(value)); }
    public void WriteBool(bool value)     { _data.AddRange(BitConverter.GetBytes(value)); }
    public void WriteBytes(byte[] value)  { _data.AddRange(value); }

    // --------------------------------------------------------------------------------------------
    // MARK: String
    // --------------------------------------------------------------------------------------------

    // Function to write a UTF8 string, starting by writing its length, then the byte array.
    // Pairs with a ReadString function in the reader.

    public void WriteString(string value)
    {
        if (value == null || value.Length == 0 || string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value), "String value cannot be null.");

        // convert the string to bytes
        byte[] data = System.Text.Encoding.UTF8.GetBytes(value);

        WriteInt(data.Length); // Write the size of the string first
        WriteBytes(data); // Write the byte array
    }

}