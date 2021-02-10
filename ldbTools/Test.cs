using MiNET.LevelDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ldbTools {
    public static class Test {
        public static void GetAllBlocks(Database db) {
            StreamReader keysFile = new StreamReader("keys.txt");

            IList<BlockStorage> blockStorages = new List<BlockStorage>();

            string line = keysFile.ReadLine();
            while (line != null) {
                //Console.WriteLine(line);
                byte[] key = line[0..^1].Split(" ").Select(b => Convert.ToByte(b, 16)).ToArray();

                if (key[8] == 0x2f) {
                    byte[] subChunkData = db.Get(key);
                    if (subChunkData != null) {
                        blockStorages.Add(BlockStorage.Load(new Span<byte>(subChunkData, 2, subChunkData.Length - 2)));
                    } else {
                        Program.PrintBytes(key);
                    }
                }

                line = keysFile.ReadLine();
            }

            ISet<string> uniqueBlocks = new HashSet<string>();
            foreach (BlockStorage storage in blockStorages) {
                foreach (BlockStateWrapper state in storage.BlockStatePalette) {
                    uniqueBlocks.Add(state.Name);
                }
            }

            foreach (string block in uniqueBlocks) {
                Console.WriteLine(block);
            }
            Console.Write("\"");
            Console.WriteLine(string.Join("\", \"", uniqueBlocks));
        }
    }
}
