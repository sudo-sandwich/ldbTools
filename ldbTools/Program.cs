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
            //string dbDir = @"C:\Users\Conrad\AppData\Local\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds\JB9MX2NLswA=\db";
            string dbDir = @"C:\Users\Conrad\AppData\Local\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds\tzdZXxzEXgA\db";

            Console.WriteLine("Opening levelDB...");
            Database db = new Database(new DirectoryInfo(dbDir));
            db.Open();

            Test.GetAllBlocks(db);
            /*
            byte[] subChunkData = db.Get(GetSubChunkKey(0, 0, 0));
            byte[] blockEntityData = db.Get(GetBlockEntityKey(0, 0));
            */

            db.Close();
            Console.WriteLine("Closed levelDB.");

            /*
            BlockStorage subChunk = new BlockStorage(new Span<byte>(subChunkData, 2, subChunkData.Length - 2));

            NbtByteReader reader = new NbtByteReader(blockEntityData);
            IList<NbtTag> blockEntities = new List<NbtTag>();
            while (reader.HasNext()) {
                blockEntities.Add(reader.ParseNext());
            }

            foreach (BlockStateWrapper state in subChunk.BlockStatePalette) {
                Console.WriteLine(state.RootNbtCompound);
            }
            foreach (NbtTag tag in blockEntities) {
                Console.WriteLine(tag);
            }
            */
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

        public static byte[] CreateSphereSubChunkBytes() {
            byte[,,] sphere = new byte[16, 16, 16];
            CreateSphere(new Vector3(7, 7, 7), 7, sphere, 1);

            BlockStateWrapper air = new BlockStateWrapper("minecraft:air");
            BlockStateWrapper glass = new BlockStateWrapper("minecraft:glass");

            BlockStorage newSubChunk = new BlockStorage();
            newSubChunk.BlockStatePalette.Add(air);
            newSubChunk.BlockStatePalette.Add(glass);
            newSubChunk.Blocks = sphere;

            byte[] newSubChunkBytes = newSubChunk.GetBytes();
            byte[] newSubChunkPrefix = new byte[2 + newSubChunkBytes.Length];
            newSubChunkPrefix[0] = 0x08;
            newSubChunkPrefix[1] = 0x01;
            Buffer.BlockCopy(newSubChunkBytes, 0, newSubChunkPrefix, 2, newSubChunkBytes.Length);

            return newSubChunkPrefix;
        }

        public static void CreateSphere(Vector3 center, int radius, byte[ , , ] destination, byte value) {
            for (int x = 0; x < destination.GetLength(0); x++) {
                for (int y = 0; y < destination.GetLength(1); y++) {
                    for (int z = 0; z < destination.GetLength(2); z++) {
                        if ((int)Math.Sqrt(Math.Pow(x - center.x, 2) + Math.Pow(y - center.y, 2) + Math.Pow(z - center.z, 2)) == radius) {
                            destination[x, y, z] = value;
                        }
                    }
                }
            }
        }

        public struct Vector3 {
            public int x;
            public int y;
            public int z;

            public Vector3(int x, int y, int z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
    }
}
