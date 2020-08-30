using ldbTools.Nbt;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ldbTools {
    public class BlockStorage {
        public IList<BlockState> BlockStatePalette { get; }

        public byte[, ,] Blocks { get; } // stored as Blocks[x, y, z]

        public int BitsPerBlock => (int)Math.Ceiling(Math.Log(BlockStatePalette.Count, 2));

        public byte StorageVersion => (byte)(BitsPerBlock << 1);

        public BlockStorage() {
            BlockStatePalette = new List<BlockState>();
            Blocks = new byte[16, 16, 16];
        }

        public BlockStorage(Span<byte> data) {
            BlockStatePalette = new List<BlockState>();
            Blocks = new byte[16, 16, 16];

            // number of bits for each BlockStatePalette index
            int bitsPerBlock = data[0] >> 1;

            // bitmask used for each index in a word
            int bitmask = (int)Math.Pow(2, bitsPerBlock) - 1;

            // number of block indices stored per word
            int blocksPerWord = 32 / bitsPerBlock;

            // total number of integers used to store each BlockStatePalette index
            int totalWords = (int)Math.Ceiling(4096.0 / blocksPerWord);

            /*
            // bits of padding at the start of each word. 0 in every case except when bitsPerBlock is 3 (padding is 2 in that case).
            int padding = 32 % bitsPerBlock;
            */

            // position of the block we are current reading. least significant 4 bits are y coordinate, next 4 bits are z coordinate, next 4 bits are x coordinate
            int blockPosition = 0;

            // parse all words except the last one
            for (int i = 0; i < totalWords - 1; i++) {
                int nextWord = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(1 + i * sizeof(int)));
                for (int j = 0; j < blocksPerWord; j++, blockPosition++) {
                    Blocks[blockPosition >> 8 & 15, blockPosition & 15, blockPosition >> 4 & 15] = (byte)(nextWord >> (j * bitsPerBlock) & bitmask);
                }
            }

            // last word might have some extra padding so we need to parse it slightly differently
            int finalWord = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(1 + (totalWords - 1) * sizeof(int)));
            for (int i = 0; blockPosition < 16 * 16 * 16; i++, blockPosition++) {
                Blocks[blockPosition >> 8 & 15, blockPosition & 15, blockPosition >> 4 & 15] = (byte)(finalWord >> (i * bitsPerBlock) & bitmask);
            }

            NbtByteReader nbtReader = new NbtByteReader(data.Slice(1 + (totalWords + 1) * sizeof(int)));
            while (nbtReader.HasNext()) {
                NbtCompound nextState = (NbtCompound)nbtReader.ParseNext();

                BlockStatePalette.Add(new BlockState(nextState));
            }
        }

        public BlockState GetBlock(int x, int y, int z) => BlockStatePalette[Blocks[x, y, z]];
    }
}
