using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ldbTools.Nbt {
    /// <summary>
    /// Class for writing NbtTags into bytes.
    /// </summary>
    public class NbtByteWriter : IDisposable {
        /// <summary>
        /// Output stream. Populated after any tag is read.
        /// </summary>
        public MemoryStream Output { get; }

        /// <summary>
        /// Creates a new NbtByteWriter.
        /// </summary>
        public NbtByteWriter() {
            Output = new MemoryStream();
        }

        /// <summary>
        /// Writes the given tag to Output.
        /// </summary>
        /// <param name="tagToWrite">Tag to write.</param>
        public void WriteNbtTag(NbtTag tagToWrite) {
            WriteTagType(tagToWrite.Type);
            WriteString(tagToWrite.Name);
            WriteNbtTagValue(tagToWrite);
        }

        /// <summary>
        /// Writes the payload of the given tag to Output (ie TagType and Name are not written).
        /// </summary>
        /// <param name="tagToWrite">Tag to write.</param>
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

        /// <summary>
        /// Writes the byte representation of a TagType to Output.
        /// </summary>
        /// <param name="value">TagType to write.</param>
        public void WriteTagType(TagType value) {
            Output.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes a byte to Output.
        /// </summary>
        /// <param name="value">Byte to write.</param>
        public void WriteByte(byte value) {
            Output.WriteByte(value);
        }

        /// <summary>
        /// Writes an array of bytes to Output.
        /// </summary>
        /// <param name="value">Array of bytes to write.</param>
        public void WriteBytes(byte[] value) {
            Output.Write(value, 0, value.Length);
        }

        /// <summary>
        /// Writes a signed byte to Output.
        /// </summary>
        /// <param name="value">Signed byte to write.</param>
        public void WriteSByte(sbyte value) {
            Output.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes an array of signed bytes to Output.
        /// </summary>
        /// <param name="value">Array of signed bytes to write.</param>
        public void WriteSBytes(sbyte[] value) {
            // stolen from: https://stackoverflow.com/questions/829983/how-to-convert-a-sbyte-to-byte-in-c
            WriteBytes((byte[])(Array)value);
        }

        /// <summary>
        /// Writes a signed short to Output.
        /// </summary>
        /// <param name="value">Signed short to write.</param>
        public void WriteShort(short value) {
            Span<byte> bytes = stackalloc byte[sizeof(short)];
            BinaryPrimitives.WriteInt16LittleEndian(bytes, value);
            Output.Write(bytes);
        }

        /// <summary>
        /// Writes an unsigned short to Output.
        /// </summary>
        /// <param name="value">Unsigned short to write.</param>
        public void WriteUShort(ushort value) {
            Span<byte> bytes = stackalloc byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16LittleEndian(bytes, value);
            Output.Write(bytes);
        }

        /// <summary>
        /// Writes a signed int to Output.
        /// </summary>
        /// <param name="value">Signed int to write.</param>
        public void WriteInt(int value) {
            Span<byte> bytes = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(bytes, value);
            Output.Write(bytes);
        }

        /// <summary>
        /// Writes an array of signed ints to Output.
        /// </summary>
        /// <param name="value">Array of signed ints to write.</param>
        public void WriteInts(int[] value) {
            foreach (int element in value) {
                WriteInt(element);
            }
        }

        /// <summary>
        /// Writes a signed long to Output.
        /// </summary>
        /// <param name="value">Signed long to write.</param>
        public void WriteLong(long value) {
            Span<byte> bytes = stackalloc byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(bytes, value);
            Output.Write(bytes);
        }

        /// <summary>
        /// Writes an array of signed longs to Output.
        /// </summary>
        /// <param name="value">Array of signed longs to write.</param>
        public void WriteLongs(long[] value) {
            foreach (long element in value) {
                WriteLong(element);
            }
        }

        /// <summary>
        /// Writes a single precision float to Output.
        /// </summary>
        /// <param name="value">Single precision float to write.</param>
        public void WriteSingle(float value) {
            WriteInt(BitConverter.SingleToInt32Bits(value));
        }

        /// <summary>
        /// Writes a double precision float to Output.
        /// </summary>
        /// <param name="value">Double precision float to write.</param>
        public void WriteDouble(double value) {
            WriteLong(BitConverter.DoubleToInt64Bits(value));
        }

        /// <summary>
        /// Writes a string to Output. The string will be encoded as UTF8.
        /// </summary>
        /// <param name="value">String to write.</param>
        public void WriteString(string value) {
            WriteUShort((ushort)Encoding.UTF8.GetByteCount(value));
            Output.Write(Encoding.UTF8.GetBytes(value));
        }

        /// <inheritdoc/>
        public void Dispose() {
            Output.Dispose();
        }
    }
}
