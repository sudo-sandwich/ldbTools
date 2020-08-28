using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ldbTools.Nbt {
    public class NbtByteWriter : IDisposable {
        public MemoryStream Output { get; }

        public NbtByteWriter() {
            Output = new MemoryStream();
        }

        /*
        public void WriteNamedNbtTag(NbtTag value) {
            switch (value.Type) {
                case TagType.Byte:
                    WriteNbtByte((NbtByte)value);
                    break;
                case TagType.Short:
                    WriteNbtShort((NbtShort)value);
                    break;
                case TagType.Int:
                    WriteNbtInt((NbtInt)value);
                    break;
                case TagType.Long:
                    WriteNbtLong((NbtLong)value);
                    break;
                case TagType.Float:
                    WriteNbtFloat((NbtFloat)value);
                    break;
                case TagType.Double:
                    WriteNbtDouble((NbtDouble)value);
                    break;
                case TagType.ByteArray:
                    WriteNbtByteArray((NbtByteArray)value);
                    break;
                case TagType.String:
                    WriteNbtString((NbtString)value);
                    break;
                case TagType.List:
                    WriteNbtList((NbtList)value);
                    break;
                case TagType.Compound:
                    WriteNbtCompound((NbtCompound)value);
                    break;
                case TagType.IntArray:
                    WriteNbtIntArray((NbtIntArray)value);
                    break;
                case TagType.LongArray:
                    WriteNbtLongArray((NbtLongArray)value);
                    break;
                default:
                    throw new Exception("Could not determine TagType.");
            }
        }
        */

        public void WriteNbtTag(NbtTag tagToWrite) {
            WriteTagType(tagToWrite.Type);
            WriteString(tagToWrite.Name);
            WriteNbtTagValue(tagToWrite);
        }

        public void WriteNbtTagValue(NbtTag tagToWrite) {
            switch (tagToWrite.Type) {
                case TagType.Byte:
                    WriteSByte(((NbtByte)tagToWrite).Value);
                    break;
                case TagType.Short:
                    WriteShort(((NbtShort)tagToWrite).Value);
                    break;
                case TagType.Int:
                    WriteInt(((NbtInt)tagToWrite).Value);
                    break;
                case TagType.Long:
                    WriteLong(((NbtLong)tagToWrite).Value);
                    break;
                case TagType.Float:
                    WriteSingle(((NbtFloat)tagToWrite).Value);
                    break;
                case TagType.Double:
                    WriteDouble(((NbtDouble)tagToWrite).Value);
                    break;
                case TagType.ByteArray:
                    NbtByteArray byteArrayToWrite = (NbtByteArray)tagToWrite;
                    WriteInt(byteArrayToWrite.Value.Count);
                    WriteSBytes(byteArrayToWrite.Value.ToArray());
                    break;
                case TagType.String:
                    WriteString(((NbtString)tagToWrite).Value);
                    break;
                case TagType.List:
                    NbtList listToWrite = (NbtList)tagToWrite;
                    WriteTagType(listToWrite.ContentType);
                    WriteInt(listToWrite.Value.Count);
                    foreach (NbtTag element in ((NbtList)tagToWrite).Value) {
                        WriteNbtTagValue(element);
                    }
                    break;
                case TagType.Compound:
                    foreach (NbtTag element in ((NbtCompound)tagToWrite).Value) {
                        WriteNbtTag(element);
                    }
                    WriteTagType(TagType.End);
                    break;
                case TagType.IntArray:
                    NbtIntArray intArrayToWrite = (NbtIntArray)tagToWrite;
                    WriteInt(intArrayToWrite.Value.Count);
                    WriteInts(intArrayToWrite.Value.ToArray());
                    break;
                case TagType.LongArray:
                    NbtLongArray longArrayToWrite = (NbtLongArray)tagToWrite;
                    WriteInt(longArrayToWrite.Value.Count);
                    WriteLongs(longArrayToWrite.Value.ToArray());
                    break;
                default:
                    throw new Exception("Could not determine TagType.");
            }
        }

        /*
        public void WriteNbtByte(NbtByte value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteSByte(value.Value);
        }

        public void WriteNbtShort(NbtShort value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteShort(value.Value);
        }

        public void WriteNbtInt(NbtInt value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteInt(value.Value);
        }

        public void WriteNbtLong(NbtLong value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteLong(value.Value);
        }

        public void WriteNbtFloat(NbtFloat value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteSingle(value.Value);
        }

        public void WriteNbtDouble(NbtDouble value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteDouble(value.Value);
        }

        public void WriteNbtByteArray(NbtByteArray value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteInt(value.Value.Count);
            WriteSBytes(value.Value.ToArray());
        }

        public void WriteNbtString(NbtString value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteString(value.Value);
        }

        public void WriteNbtList(NbtList value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteTagType(value.Type);
            WriteInt(value.Value.Count);
            
            foreach (NbtTag element in value.Value) {
                WriteUnnamedNbtTag(element);
            }
        }

        public void WriteNbtCompound(NbtCompound value) {
            WriteTagType(value.Type);
            WriteString(value.Name);

            foreach (NbtTag element in value.Value) {
                WriteNamedNbtTag(element);
            }

            WriteTagType(TagType.End);
        }

        public void WriteNbtIntArray(NbtIntArray value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteInt(value.Value.Count);
            WriteInts(value.Value.ToArray());
        }

        public void WriteNbtLongArray(NbtLongArray value) {
            WriteTagType(value.Type);
            WriteString(value.Name);
            WriteInt(value.Value.Count);
            WriteLongs(value.Value.ToArray());
        }
        */

        public void WriteTagType(TagType value) {
            Output.WriteByte((byte)value);
        }

        public void WriteByte(byte value) {
            Output.WriteByte(value);
        }

        public void WriteBytes(byte[] value) {
            Output.Write(value, 0, value.Length);
        }

        public void WriteSByte(sbyte value) {
            Output.WriteByte((byte)value);
        }

        public void WriteSBytes(sbyte[] value) {
            // stolen from: https://stackoverflow.com/questions/829983/how-to-convert-a-sbyte-to-byte-in-c
            WriteBytes((byte[])(Array)value);
        }

        public void WriteShort(short value) {
            Span<byte> bytes = stackalloc byte[sizeof(short)];
            BinaryPrimitives.WriteInt16LittleEndian(bytes, value);
            Output.Write(bytes);
        }

        public void WriteUShort(ushort value) {
            Span<byte> bytes = stackalloc byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16LittleEndian(bytes, value);
            Output.Write(bytes);
        }

        public void WriteInt(int value) {
            Span<byte> bytes = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(bytes, value);
            Output.Write(bytes);
        }

        public void WriteInts(int[] value) {
            foreach (int element in value) {
                WriteInt(element);
            }
        }

        public void WriteLong(long value) {
            Span<byte> bytes = stackalloc byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(bytes, value);
            Output.Write(bytes);
        }

        public void WriteLongs(long[] value) {
            foreach (long element in value) {
                WriteLong(element);
            }
        }

        public void WriteSingle(float value) {
            WriteInt(BitConverter.SingleToInt32Bits(value));
        }

        public void WriteDouble(double value) {
            WriteLong(BitConverter.DoubleToInt64Bits(value));
        }

        public void WriteString(string value) {
            WriteUShort((ushort)Encoding.UTF8.GetByteCount(value));
            Output.Write(Encoding.UTF8.GetBytes(value));
        }

        public void Dispose() {
            Output.Dispose();
        }
    }
}
