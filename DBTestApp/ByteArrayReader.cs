
using System;

public class ByteArrayReader
{
    private readonly byte[] _data;
    private int             _position;

    public ByteArrayReader(byte[] data)
    {
        _data     = data;
        _position = 0;
    }

    public int  Position  => _position;
    public bool HasMore   => _position < _data.Length;
    public int  BytesLeft => _data.Length - _position;

    // --------------------------------------------------------------------------------------------
    // MARK: Read Methods
    // --------------------------------------------------------------------------------------------

    public int ReadInt()
    {
        if (_position + sizeof(int) > _data.Length)
            throw new InvalidOperationException("Not enough data to read an int.");

        int result = BitConverter.ToInt32(_data, _position);
        _position += sizeof(int);
        return result;
    }

    public float ReadFloat()
    {
        if (_position + sizeof(float) > _data.Length)
            throw new InvalidOperationException("Not enough data to read a float.");

        float result = BitConverter.ToSingle(_data, _position);
        _position += sizeof(float);
        return result;
    }

    public double ReadDouble()
    {
        if (_position + sizeof(double) > _data.Length)
            throw new InvalidOperationException("Not enough data to read a double.");

        double result = BitConverter.ToDouble(_data, _position);
        _position += sizeof(double);
        return result;
    }

    public bool ReadBool()
    {
        if (_position + sizeof(bool) > _data.Length)
            throw new InvalidOperationException("Not enough data to read a bool.");

        bool result = BitConverter.ToBoolean(_data, _position);
        _position += sizeof(bool);
        return result;
    }

    public byte[] ReadBytes(int count)
    {
        if (_position + count > _data.Length)
            throw new InvalidOperationException("Not enough data to read the specified number of bytes.");

        byte[] result = new byte[count];
        Array.Copy(_data, _position, result, 0, count);
        _position += count;
        return result;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String
    // --------------------------------------------------------------------------------------------

    // More complex function to read a string from the byte array. Reads a size first, then a number
    // of characters to create a UTF8 string.

    public string ReadString()
    {
        int size = ReadInt();
        if (_position + size > _data.Length)
            throw new InvalidOperationException("Not enough data to read the specified number of bytes.");

        string result = System.Text.Encoding.UTF8.GetString(_data, _position, size);
        _position += size;
        return result;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Read Pos
    // --------------------------------------------------------------------------------------------

    public void ResetPosition()
    {
        _position = 0;
    }

    public void SkipBytes(int count)
    {
        if (_position + count > _data.Length)
            throw new InvalidOperationException("Not enough data to skip the specified number of bytes.");

        _position += count;
    }

    public void SkipToEnd()
    {
        _position = _data.Length;
    }
}
