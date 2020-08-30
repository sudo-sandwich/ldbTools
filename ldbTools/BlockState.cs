using ldbTools.Nbt;
using System;
using System.Collections.Generic;
using System.Text;

namespace ldbTools {
    public class BlockState {
        public NbtString NbtName { get; set; }
        public string Name { 
            get => NbtName.Value;
            set => NbtName.Value = value;
        }

        public NbtCompound NbtStates { get; set; }
        public IList<NbtTag> States => NbtStates.Value;

        public NbtInt NbtVersion { get; set; }
        public int Version {
            get => NbtVersion.Value;
            set => NbtVersion.Value = value;
        }

        private NbtCompound _rootCompound { get; set; }

        public BlockState(string name) {
            NbtName = new NbtString("name", name);
            NbtStates = new NbtCompound("states");
            NbtVersion = new NbtInt("version", 17825808); // magic number. never seen this value be anything else.

            _rootCompound = new NbtCompound("", NbtName, NbtStates, NbtVersion);
        }

        public BlockState(NbtCompound source) {
            NbtName = source.Get<NbtString>("name");
            NbtStates = source.Get<NbtCompound>("states");
            NbtVersion = source.Get<NbtInt>("version");

            _rootCompound = source;
        }

        /// <summary>
        /// Returns a new NbtCompound wrapping the contents of this BlockState.
        /// </summary>
        /// <returns>A new NbtCompound wrapping the contents of this BlockState.</returns>
        public NbtCompound ToNbt() {
            return _rootCompound;
        }
    }
}
