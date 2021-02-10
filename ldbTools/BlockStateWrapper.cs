using ldbTools.Nbt;
using System;
using System.Collections.Generic;
using System.Text;

namespace ldbTools {
    /// <summary>
    /// Wraps block state NBT data.
    /// </summary>
    /// <remarks>Changes to a BlockState object will affect the underlying NBT and vice versa.</remarks>
    public class BlockStateWrapper {
        /// <summary>
        /// Gets or sets the underlying NBT for the namespaced ID of the block.
        /// </summary>
        public NbtString NbtName { get; set; }

        /// <summary>
        /// Gets or sets the namespaced ID of the block.
        /// </summary>
        public string Name { 
            get => NbtName.Value;
            set => NbtName.Value = value;
        }

        /// <summary>
        /// Gets or sets the underlying NBT for the states of the block.
        /// </summary>
        public NbtCompound NbtStates { get; set; }

        /// <summary>
        /// Gets or sets the states of the block.
        /// </summary>
        public IList<NbtTag> States => NbtStates.Value;

        /// <summary>
        /// Gets or sets the underlying NBT for the block state version.
        /// </summary>
        /// <remarks>I have never seen the version number be anything different from <c>17825808</c>.</remarks>
        public NbtInt NbtVersion { get; set; }
        public int Version {
            get => NbtVersion.Value;
            set => NbtVersion.Value = value;
        }

        /// <summary>
        /// Gets or sets the underlying NBT for the root compound. Changing this will effectively change the entire block.
        /// </summary>
        public NbtCompound RootNbtCompound { get; set; }

        /// <summary>
        /// Creates a new, empty BlockState.
        /// </summary>
        /// <param name="name">Namespaced ID of the block this BlockState will represent.</param>
        public BlockStateWrapper(string name) {
            NbtName = new NbtString("name", name);
            NbtStates = new NbtCompound("states");
            NbtVersion = new NbtInt("version", 17825808); // magic number. never seen this value be anything else.

            RootNbtCompound = new NbtCompound("", NbtName, NbtStates, NbtVersion);
        }

        /// <summary>
        /// Wraps an existing NbtCompound as a new BlockState. Changes to this BlockState will affect the underlying NBT and vice versa.
        /// </summary>
        /// <param name="source">The NbtCompound to wrap.</param>
        public BlockStateWrapper(NbtCompound source) {
            NbtName = source.Get<NbtString>("name");
            NbtStates = source.Get<NbtCompound>("states");
            NbtVersion = source.Get<NbtInt>("version");

            RootNbtCompound = source;
        }
    }
}
