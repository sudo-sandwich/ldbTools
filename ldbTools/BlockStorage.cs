using ldbTools.Nbt;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ldbTools {
    /// <summary>
    /// Represents a 16 x 16 x 16 subchunk of blocks in Minecraft.
    /// </summary>
    /// <remarks>For more information on how block storage works in Minecraft: https://minecraft.gamepedia.com/Bedrock_Edition_level_format#SubChunkPrefix_record_.281.0_and_1.2.13_formats.29 </remarks>
    public class BlockStorage {
        /// <summary>
        /// Number of blocks per side of a subchunk in Minecraft.
        /// </summary>
        public const int BlocksPerSide = 16;

        /// <summary>
        /// Total number of blocks represented in a subchunk in Minecraft.
        /// </summary>
        public const int TotalBlockCount = BlocksPerSide * BlocksPerSide * BlocksPerSide;

        /// <summary>
        /// Palette of BlockStates used in this subchunk. Every unique block in the subchunk is stored here.
        /// </summary>
        public IList<BlockStateWrapper> BlockStatePalette { get; }

        /// <summary>
        /// Index of the BlockState for each block in this subchunk.
        /// </summary>
        public byte[ , , ] Blocks { get; set; } // stored as Blocks[x, y, z]

        /// <summary>
        /// Creates a new, empty BlockStorage.
        /// </summary>
        public BlockStorage() {
            BlockStatePalette = new List<BlockStateWrapper>();
            Blocks = new byte[BlocksPerSide, BlocksPerSide, BlocksPerSide];
        }

        /// <summary>
        /// Parses the first bytes of <paramref name="data"/> into a new BlockStorage. Following bytes will be ignored.
        /// </summary>
        /// <param name="data">Block storage data provided by Minecraft to parse.</param>
        public static BlockStorage Load(Span<byte> data) {
            BlockStorage bs = new BlockStorage();

            // number of bits for each BlockStatePalette index
            int bitsPerBlock = data[0] >> 1;

            // bitmask used for each index in a word
            int bitmask = (int)Math.Pow(2, bitsPerBlock) - 1;

            // number of block indices stored per word
            int blocksPerWord = 32 / bitsPerBlock;

            // total number of integers used to store each BlockStatePalette index
            int totalWords = (int)Math.Ceiling((float)TotalBlockCount / blocksPerWord);

            // position of the block we are currently parsing. least significant 4 bits are y coordinate, next 4 bits are z coordinate, next 4 bits are x coordinate
            int blockPosition = 0;

            // parse all words except the last one
            for (int i = 0; i < totalWords - 1; i++) {
                uint nextWord = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(1 + i * sizeof(int)));
                for (int j = 0; j < blocksPerWord; j++, blockPosition++) {
                    bs.Blocks[blockPosition >> 8 & 15, blockPosition & 15, blockPosition >> 4 & 15] = (byte)(nextWord >> (j * bitsPerBlock) & bitmask);
                }
            }

            // last word might have some extra padding so we need to parse it slightly differently
            uint finalWord = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(1 + (totalWords - 1) * sizeof(int)));
            for (int i = 0; blockPosition < TotalBlockCount; i++, blockPosition++) {
                bs.Blocks[blockPosition >> 8 & 15, blockPosition & 15, blockPosition >> 4 & 15] = (byte)(finalWord >> (i * bitsPerBlock) & bitmask);
            }

            // parse BlockStatePalette
            int paletteSize = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(1 + totalWords * sizeof(int)));
            NbtByteReader nbtReader = new NbtByteReader(data.Slice(1 + (totalWords + 1) * sizeof(int)));
            for (int i = 0; i < paletteSize; i++) {
                NbtCompound nextState = (NbtCompound)nbtReader.ParseNext();
                bs.BlockStatePalette.Add(new BlockStateWrapper(nextState));
            }

            return bs;
        }

        /// <summary>
        /// Gets the BlockState of the block at the given position.
        /// </summary>
        /// <param name="x">X coordinate of the block.</param>
        /// <param name="y">Y coordinate of the block.</param>
        /// <param name="z">Z coordinate of the block.</param>
        /// <returns>The block state of the block at the given position.</returns>
        public BlockStateWrapper GetBlock(int x, int y, int z) => BlockStatePalette[Blocks[x, y, z]];

        /// <summary>
        /// Encodes this BlockStorage into a sequence of bytes to be written into levelDB.
        /// </summary>
        /// <returns>A byte array representing this BlockStorage that can be written into levelDB.</returns>
        public byte[] GetBytes() {
            using (MemoryStream output = new MemoryStream()) {
                // number of bits for each BlockStatePalette index
                int bitsPerBlock = (int)Math.Ceiling(Math.Log(BlockStatePalette.Count, 2));

                // number of block indices stored per word
                int blocksPerWord = 32 / bitsPerBlock;

                // total number of integers used to store each BlockStatePalette index. the last word is not included, since it might have some extra padding
                int numFullWords = TotalBlockCount / blocksPerWord;

                // position of the block we are currently parsing. least significant 4 bits are y coordinate, next 4 bits are z coordinate, next 4 bits are x coordinate
                int blockPosition = 0;

                // write storage version
                output.WriteByte((byte)(bitsPerBlock << 1));

                // write all full words
                for (int i = 0; i < numFullWords; i++) {
                    // create the next word
                    uint nextWord = 0;
                    for (int j = 0; j < blocksPerWord; j++, blockPosition++) {
                        nextWord += (uint)(Blocks[blockPosition >> 8 & 15, blockPosition & 15, blockPosition >> 4 & 15] << (j * bitsPerBlock));
                    }

                    // write the next word
                    Span<byte> nextWordBytes = stackalloc byte[sizeof(int)];
                    BinaryPrimitives.WriteUInt32LittleEndian(nextWordBytes, nextWord);
                    output.Write(nextWordBytes);
                }

                // handle the final word if necessary
                if (blockPosition < TotalBlockCount) {
                    // create the final word
                    uint finalWord = 0;
                    for (int i = 0; blockPosition < TotalBlockCount; i++, blockPosition++) {
                        finalWord += (uint)(Blocks[blockPosition >> 8 & 15, blockPosition & 15, blockPosition >> 4 & 15] << (i * bitsPerBlock));
                    }

                    // write the final word
                    Span<byte> finalWordBytes = stackalloc byte[sizeof(int)];
                    BinaryPrimitives.WriteUInt32LittleEndian(finalWordBytes, finalWord);
                    output.Write(finalWordBytes);
                }

                // write the size of the palette
                Span<byte> paletteSizeBytes = stackalloc byte[sizeof(int)];
                BinaryPrimitives.WriteInt32LittleEndian(paletteSizeBytes, BlockStatePalette.Count);
                output.Write(paletteSizeBytes);

                // write the block states as NBT
                NbtByteWriter nbtWriter = new NbtByteWriter(output);
                foreach (BlockStateWrapper state in BlockStatePalette) {
                    nbtWriter.WriteNbtTag(state.RootNbtCompound);
                }

                return output.ToArray();
            }
        }
    }
}
