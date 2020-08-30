using ldbTools.Nbt;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ldbTools {
    public class BlockStorage {
        /// <summary>
        /// Number of blocks per side of this subchunk.
        /// </summary>
        public const int BlocksPerSide = 16;

        /// <summary>
        /// Palette of BlockStates used in this subchunk. Every unique block in the subchunk is stored here.
        /// </summary>
        public IList<BlockState> BlockStatePalette { get; }

        /// <summary>
        /// Index of the BlockState for each block in this subchunk.
        /// </summary>
        public byte[ , , ] Blocks { get; } // stored as Blocks[x, y, z]

        public int BitsPerBlock => (int)Math.Ceiling(Math.Log(BlockStatePalette.Count, 2));

        public byte StorageVersion => (byte)(BitsPerBlock << 1);

        /// <summary>
        /// Creates a new, empty BlockStorage.
        /// </summary>
        public BlockStorage() {
            BlockStatePalette = new List<BlockState>();
            Blocks = new byte[BlocksPerSide, BlocksPerSide, BlocksPerSide];
        }

        /// <summary>
        /// Parses the first bytes of <paramref name="data"/> into a new BlockStorage. Following bytes will be ignored.
        /// </summary>
        /// <param name="data">Block storage data provided by Minecraft to parse.</param>
        public BlockStorage(Span<byte> data) {
            BlockStatePalette = new List<BlockState>();
            Blocks = new byte[BlocksPerSide, BlocksPerSide, BlocksPerSide];

            // number of bits for each BlockStatePalette index
            int bitsPerBlock = data[0] >> 1;

            // bitmask used for each index in a word
            int bitmask = (int)Math.Pow(2, bitsPerBlock) - 1;

            // number of block indices stored per word
            int blocksPerWord = 32 / bitsPerBlock;

            // total number of integers used to store each BlockStatePalette index
            int totalWords = (int)Math.Ceiling(4096.0 / blocksPerWord);

            // position of the block we are current parsing. least significant 4 bits are y coordinate, next 4 bits are z coordinate, next 4 bits are x coordinate
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
            for (int i = 0; blockPosition < BlocksPerSide * BlocksPerSide * BlocksPerSide; i++, blockPosition++) {
                Blocks[blockPosition >> 8 & 15, blockPosition & 15, blockPosition >> 4 & 15] = (byte)(finalWord >> (i * bitsPerBlock) & bitmask);
            }

            // parse BlockStatePalette
            int paletteSize = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(1 + totalWords * sizeof(int)));
            NbtByteReader nbtReader = new NbtByteReader(data.Slice(1 + (totalWords + 1) * sizeof(int)));
            for (int i = 0; i < paletteSize; i++) {
                NbtCompound nextState = (NbtCompound)nbtReader.ParseNext();
                BlockStatePalette.Add(new BlockState(nextState));
            }
        }

        /// <summary>
        /// Gets the BlockState of the block at the given position.
        /// </summary>
        /// <param name="x">X coordinate of the block.</param>
        /// <param name="y">Y coordinate of the block.</param>
        /// <param name="z">Z coordinate of the block.</param>
        /// <returns></returns>
        public BlockState GetBlock(int x, int y, int z) => BlockStatePalette[Blocks[x, y, z]];
    }
}
