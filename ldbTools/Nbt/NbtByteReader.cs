using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Struct for reading NBT bytes. All bytes are read as little endian.
    /// </summary>
    public ref struct NbtByteReader {
        /// <summary>
        /// Span of bytes to be read by this NbtByteReader
        /// </summary>
        public ReadOnlySpan<byte> Input { get; private set; }

        /// <summary>
        /// Gets or sets the current position of this reader.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Creates a new NbtByteReader.
        /// </summary>
        /// <param name="input">Bytes to be read by this reader.</param>
        public NbtByteReader(ReadOnlySpan<byte> input) {
            Input = input;
            Position = 0;
        }

        /// <summary>
        /// Check if the end of the span has been reached.
        /// </summary>
        /// <returns>True if there is still more data to be read, false if the position of this reader is at the end of its span.</returns>
        public bool HasNext() {
            return Position != Input.Length;
        }

        /// <summary>
        /// Returns the next byte of this reader without advancing the position.
        /// </summary>
        /// <returns>Next byte of this reader or -1 if the end of the reader has been reached.</returns>
        public int PeekNext() {
            return HasNext() ? Input[Position] : -1;
        }

        /// <summary>
        /// Parses the next byte as a TagType
        /// </summary>
        /// <returns>TagType of the bytes after the tag.</returns>
        public TagType ParseTagType() {
            byte nextByte = Input[Position];
            Position++;

            TagType nextTag;
            if (Enum.IsDefined(typeof(TagType), nextByte)) {
                nextTag = (TagType)nextByte;
            } else {
                throw new Exception("Next byte does not match any known tag types.");
            }

            return nextTag;
        }

        /// <summary>
        /// Parses the entirety of the next tag, TagType included.
        /// </summary>
        /// <returns>The next NBT tag.</returns>
        public NbtTag ParseNext() {
            return ParseNext(ParseTagType());
        }

        /// <summary>
        /// Parses the entirety of the next tag, assuming TagType has already been read.
        /// </summary>
        /// <param name="nextTagType">The TagType the next bytes should be parsed as.</param>
        /// <returns>The next NbtTag.</returns>
        public NbtTag ParseNext(TagType nextTagType) {
            switch (nextTagType) {
                case TagType.End:
                    throw new ArgumentException("Cannot parse TAG_End because it doesn't have a payload.");
                case TagType.Byte:
                    return new NbtByte(ReadPrefixedString(), ReadSByte());
                case TagType.Short:
                    return new NbtShort(ReadPrefixedString(), ReadShort());
                case TagType.Int:
                    return new NbtInt(ReadPrefixedString(), ReadInt());
                case TagType.Long:
                    return new NbtLong(ReadPrefixedString(), ReadLong());
                case TagType.Float:
                    return new NbtFloat(ReadPrefixedString(), ReadFloat());
                case TagType.Double:
                    return new NbtDouble(ReadPrefixedString(), ReadDouble());
                case TagType.ByteArray:
                    return new NbtByteArray(ReadPrefixedString(), ReadSBytes(ReadInt()));
                case TagType.String:
                    return new NbtString(ReadPrefixedString(), ReadPrefixedString());
                case TagType.List:
                    return ParseListPayload(ReadPrefixedString());
                case TagType.Compound:
                    return ParseCompoundPayload(ReadPrefixedString());
                case TagType.IntArray:
                    return new NbtIntArray(ReadPrefixedString(), ReadInts(ReadInt()));
                case TagType.LongArray:
                    return new NbtLongArray(ReadPrefixedString(), ReadLongs(ReadInt()));
                default:
                    throw new Exception("Could not recognize TagType.");
            }
        }

        /// <summary>
        /// Parses the entirety of the next unnamed tag. Unnamed tags are also not prefixed by their TagType, so a TagType must be given.
        /// </summary>
        /// <remarks>Unnamed NbtTags should only ever appear in an NbtList.</remarks>
        /// <param name="nextTagType">The TagType the next bytes should be parsed as.</param>
        /// <returns>The next unnamed NbtTag.</returns>
        public NbtTag ParseNextUnnamed(TagType nextTagType) {
            switch (nextTagType) {
                case TagType.End:
                    throw new ArgumentException("Cannot parse TAG_End because it doesn't have a payload.");
                case TagType.Byte:
                    return new NbtByte("", ReadSByte());
                case TagType.Short:
                    return new NbtShort("", ReadShort());
                case TagType.Int:
                    return new NbtInt("", ReadInt());
                case TagType.Long:
                    return new NbtLong("", ReadLong());
                case TagType.Float:
                    return new NbtFloat("", ReadFloat());
                case TagType.Double:
                    return new NbtDouble("", ReadDouble());
                case TagType.ByteArray:
                    return new NbtByteArray("", ReadSBytes(ReadInt()));
                case TagType.String:
                    return new NbtString("", ReadPrefixedString());
                case TagType.List:
                    return ParseListPayload("");
                case TagType.Compound:
                    return ParseCompoundPayload("");
                case TagType.IntArray:
                    return new NbtIntArray("", ReadInts(ReadInt()));
                case TagType.LongArray:
                    return new NbtLongArray("", ReadLongs(ReadInt()));
                default:
                    throw new Exception("Could not recognize TagType.");
            }
        }

        /// <summary>
        /// Parses the payload of TAG_List.
        /// </summary>
        /// <remarks>All NbtTags in TAG_List are unnamed.</remarks>
        /// <param name="name">Name of the new NbtList</param>
        /// <returns>A new named NbtList populated with the data that was read.</returns>
        public NbtList ParseListPayload(string name) {
            TagType listType = ParseTagType();
            NbtList retVal = new NbtList(name, listType);
            int length = ReadInt();

            for (int i = 0; i < length; i++) {
                retVal.Value.Add(ParseNextUnnamed(listType));
            }

            return retVal;
        }

        /// <summary>
        /// Parses the payload of TAG_Compound.
        /// </summary>
        /// <param name="name">Name of the new NbtCompound</param>
        /// <returns>A new named NbtCompound populated with the data that was read.</returns>
        public NbtCompound ParseCompoundPayload(string name) {
            NbtCompound retVal = new NbtCompound(name);

            // loop until we find TAG_End
            TagType nextTagType = ParseTagType();
            while (nextTagType != TagType.End) {
                retVal.Value.Add(ParseNext(nextTagType));
                nextTagType = ParseTagType();
            }

            return retVal;
        }

        /// <summary>
        /// Reads the next byte as an unsigned byte.
        /// </summary>
        /// <returns>The next unsigned byte.</returns>
        public byte ReadByte() {
            byte nextByte = Input[Position];
            Position++;

            return nextByte;
        }

        /// <summary>
        /// Reads the next bytes as an array of unsigned bytes.
        /// </summary>
        /// <param name="count">The number of unsigned bytes to read.</param>
        /// <returns>The unsigned bytes that were read.</returns>
        public byte[] ReadBytes(int count) {
            byte[] nextBytes = Input.Slice(Position, count).ToArray();
            Position += count;

            return nextBytes;
        }

        /// <summary>
        /// Reads the next byte as a signed byte.
        /// </summary>
        /// <returns>The next signed byte.</returns>
        public sbyte ReadSByte() {
            sbyte nextSByte = (sbyte)Input[Position];
            Position++;

            return nextSByte;
        }

        /// <summary>
        /// Reads the next bytes as an array of signed bytes.
        /// </summary>
        /// <param name="count">The number of signed bytes to read.</param>
        /// <returns>The signed bytes that were read.</returns>
        public sbyte[] ReadSBytes(int count) {
            sbyte[] nextSBytes = new sbyte[count];

            for (int i = 0; i < count; i++) {
                nextSBytes[i] = ReadSByte();
            }

            return nextSBytes;
        }

        /// <summary>
        /// Reads the next 2 bytes as a signed short.
        /// </summary>
        /// <returns>The next signed short.</returns>
        public short ReadShort() {
            short nextShort = BinaryPrimitives.ReadInt16LittleEndian(Input.Slice(Position));
            Position += sizeof(short);

            return nextShort;
        }

        /// <summary>
        /// Reads the next 2 bytes as an unsigned short.
        /// </summary>
        /// <returns>The next unsigned short.</returns>
        public ushort ReadUShort() {
            ushort nextUShort = BinaryPrimitives.ReadUInt16LittleEndian(Input.Slice(Position));
            Position += sizeof(ushort);

            return nextUShort;
        }

        /// <summary>
        /// Reads the next 4 bytes as a signed int.
        /// </summary>
        /// <returns>The next signed int.</returns>
        public int ReadInt() {
            int nextInt = BinaryPrimitives.ReadInt32LittleEndian(Input.Slice(Position));
            Position += sizeof(int);

            return nextInt;
        }

        /// <summary>
        /// Reads the next bytes as an array of signed ints.
        /// </summary>
        /// <param name="count">The number of signed ints to read.</param>
        /// <returns>The signed ints that were read.</returns>
        public int[] ReadInts(int count) {
            int[] nextInts = new int[count];

            for (int i = 0; i < count; i++) {
                nextInts[i] = ReadInt();
            }

            return nextInts;
        }

        /// <summary>
        /// Reads the next 8 bytes as a signed long.
        /// </summary>
        /// <returns>The next signed long.</returns>
        public long ReadLong() {
            long nextLong = BinaryPrimitives.ReadInt64LittleEndian(Input.Slice(Position));
            Position += sizeof(long);

            return nextLong;
        }

        /// <summary>
        /// Reads the next bytes as an array of signed longs.
        /// </summary>
        /// <param name="count">The number of longs to read.</param>
        /// <returns>The signed longs that were read.</returns>
        public long[] ReadLongs(int count) {
            long[] nextInts = new long[count];

            for (int i = 0; i < count; i++) {
                nextInts[i] = ReadLong();
            }

            return nextInts;
        }

        /// <summary>
        /// Reads the next 4 bytes as a single precision float.
        /// </summary>
        /// <returns>The next single precision float.</returns>
        public float ReadFloat() {
            float nextFloat = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(Input.Slice(Position)));
            Position += sizeof(float);

            return nextFloat;
        }

        /// <summary>
        /// Reads the next 8 bytes as a double precision float.
        /// </summary>
        /// <returns>The next double precision float.</returns>
        public double ReadDouble() {
            double nextDouble = BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(Input.Slice(Position)));
            Position += sizeof(double);

            return nextDouble;
        }

        /// <summary>
        /// Reads the next bytes as a UTF8 encoded string.
        /// </summary>
        /// <param name="length">The length of the string to be read.</param>
        /// <returns>The next UTF8 encoded string.</returns>
        public string ReadString(ushort length) {
            string nextString = Encoding.UTF8.GetString(Input.Slice(Position, length));
            Position += length;

            return nextString;
        }

        /// <summary>
        /// Reads the next bytes as a UTF8 encoded string, prefixed by its length.
        /// </summary>
        /// <returns>The next UTF8 encoded string.</returns>
        public string ReadPrefixedString() {
            return ReadString(ReadUShort());
        }
    }
}
