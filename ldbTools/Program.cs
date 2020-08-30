using ldbTools.Nbt;
using MiNET.LevelDB;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ldbTools {
    class Program {
        static void Main(string[] args) {
            string dbDir = @"C:\Users\Conrad\AppData\Local\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds\jmVIX8GHAAA=\db";

            byte[] subChunkKey = GetSubChunkKey(0, 0, 0);
            Console.Write("SubChunkPrefix key: ");
            PrintBytes(subChunkKey);

            Database db = new Database(new DirectoryInfo(dbDir));
            db.Open();

            byte[] chunk = db.Get(subChunkKey);
            //byte[] blockEntity = db.Get(blockEntityKey);

            db.Close();

            PrintBytes(chunk);

            byte subChunkVersion = chunk[0];
            byte storageCount = chunk[1];

            BlockStorage blockStorage = new BlockStorage(new Span<byte>(chunk, 2, chunk.Length - 2));

            Console.WriteLine("subChunkVersion: {0}", subChunkVersion);
            Console.WriteLine("storageCount: {0}", storageCount);

            for (int x = 0; x < blockStorage.Blocks.GetLength(0); x++) {
                for (int y = 0; y < blockStorage.Blocks.GetLength(1); y++) {
                    for (int z = 0; z < blockStorage.Blocks.GetLength(2); z++) {
                        Console.WriteLine("{0}, {1}, {2}: {3}, {4}", x, y, z, blockStorage.GetBlock(x, y, z).Name, blockStorage.Blocks[x, y, z]);
                    }
                }
            }

            foreach (BlockState blockState in blockStorage.BlockStatePalette) {
                Console.WriteLine(blockState.RootNbtCompound);
            }
        }

        public static byte[] GetSubChunkKey(int x, int z, int y) {
            if (y < 0 || y > 15) {
                throw new ArgumentOutOfRangeException("y subchunk must be between 0 and 15.");
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

        public static byte[] GetBlockEntityKey(int x, int z) {
            byte[] key = new byte[9];
            Span<byte> xSpan = new Span<byte>(key, 0, sizeof(int));
            Span<byte> zSpan = new Span<byte>(key, xSpan.Length, sizeof(int));

            BinaryPrimitives.WriteInt32LittleEndian(xSpan, x);
            BinaryPrimitives.WriteInt32LittleEndian(zSpan, z);
            key[8] = 0x31; // magic number for BlockEntity record

            return key;
        }

        public static void PrintBytes(Span<byte> printMe) {
            foreach (byte b in printMe) {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.WriteLine();
        }
    }
}
