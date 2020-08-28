using ldbTools.Nbt;
using MiNET.LevelDB;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ldbTools {
    class Program {
        static void Main(string[] args) {
            string dbDir = @"C:\Users\Conrad\AppData\Local\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds\jmVIX8GHAAA=\db";

            byte[] subChunkKey = GetSubChunkKey(0, 0, 0);
            Console.Write("SubChunkPrefix key: ");
            PrintBytes(subChunkKey);

            byte[] blockEntityKey = GetBlockEntityKey(0, 0);
            Console.Write("BlockEntity key: ");
            PrintBytes(blockEntityKey);

            Database db = new Database(new DirectoryInfo(dbDir));
            db.Open();

            byte[] chunk = db.Get(subChunkKey);
            byte[] blockEntity = db.Get(blockEntityKey);

            db.Close();

            PrintBytes(chunk);
            Console.WriteLine("chunk is {0} bytes", chunk.Length);

            byte subChunkVersion = chunk[0];
            byte storageCount = chunk[1];
            byte storageVersion = chunk[2];

            int bitsPerBlock = storageVersion >> 1;
            int blocksPerWord = 32 / bitsPerBlock;
            int numBlockStates = (int)Math.Ceiling(4096.0 / blocksPerWord);

            Span<byte> blockStatesAsIndices = new Span<byte>(chunk, 3, numBlockStates * sizeof(int));

            byte[] paletteSizeBytes = new byte[4];
            Array.Copy(chunk, 3 + numBlockStates * 4, paletteSizeBytes, 0, 4);
            if (!BitConverter.IsLittleEndian) {
                Array.Reverse(paletteSizeBytes);
            }

            int paletteSize = BinaryPrimitives.ReadInt32LittleEndian(new Span<byte>(chunk, 3 + blockStatesAsIndices.Length, sizeof(int)));
            int blockStatesOffsetIndex = 3 + numBlockStates * 4 + sizeof(int);
            Span<byte> blockStatesBytes = new Span<byte>(chunk, blockStatesOffsetIndex, chunk.Length - blockStatesOffsetIndex);

            Console.WriteLine("subChunkVersion: {0}", subChunkVersion);
            Console.WriteLine("storageCount: {0}", storageCount);
            Console.WriteLine("storageVersion: {0}", storageVersion);
            Console.WriteLine("blockStatesAsIndices: ");
            PrintBytes(blockStatesAsIndices);
            Console.WriteLine("paletteSize: {0}", paletteSize);
            Console.WriteLine("blockStates: ");
            PrintBytes(blockStatesBytes);

            NbtByteReader reader = new NbtByteReader(blockStatesBytes);
            IList<NbtCompound> blockStates = new List<NbtCompound>();
            while (reader.HasNext()) {
                if (reader.ParseTagType() != TagType.Compound) {
                    throw new Exception("Expecting TAG_Compound, got something else instead.");
                }
                blockStates.Add((NbtCompound)reader.ParseNext(TagType.Compound));
            }

            foreach (NbtTag compound in blockStates) {
                Console.WriteLine(compound.ToString());
            }

            PrintBytes(blockEntity);

            blockStates.Clear();
            reader = new NbtByteReader(blockEntity);
            while (reader.HasNext()) {
                if (reader.ParseTagType() != TagType.Compound) {
                    throw new Exception("Expecting TAG_Compound, got something else instead.");
                }
                blockStates.Add((NbtCompound)reader.ParseNext(TagType.Compound));
            }

            foreach (NbtTag compound in blockStates) {
                Console.WriteLine(compound.ToString());
            }
        }

        public static byte[] GetSubChunkKey(int x, int z, int y) {
            if (y < 0 || y > 15) {
                throw new ArgumentException("y subchunk must be between 0 and 15.");
            }

            byte[] key = new byte[10];
            Span<byte> xSpan = new Span<byte>(key, 0, sizeof(int));
            Span<byte> zSpan = new Span<byte>(key, xSpan.Length, sizeof(int));

            BinaryPrimitives.WriteInt32LittleEndian(xSpan, x);
            BinaryPrimitives.WriteInt32LittleEndian(zSpan, z);
            key[8] = 0x2F; // magic number for SubChunkPrefix record
            key[9] = (byte)y;

            return key;
        }

        public static void PrintBytes(Span<byte> printMe) {
            foreach (byte b in printMe) {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.WriteLine();
        }

        public static byte[] GetBlockEntityKey(int x, int z) {
            byte[] key = new byte[9];
            Span<byte> xSpan = new Span<byte>(key, 0, sizeof(int));
            Span<byte> zSpan = new Span<byte>(key, xSpan.Length, sizeof(int));

            BinaryPrimitives.WriteInt32LittleEndian(xSpan, x);
            BinaryPrimitives.WriteInt32LittleEndian(zSpan, z);
            key[8] = 0x31; // magic number for BlockEntity record

            return key;
        }
    }
}
