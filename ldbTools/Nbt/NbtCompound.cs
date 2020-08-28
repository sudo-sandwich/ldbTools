using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Compound.
    /// </summary>
    public class NbtCompound : NbtTag {
        /// <summary>
        /// Value of this NbtCompound.
        /// </summary>
        public IList<NbtTag> Value { get; }

        /// <summary>
        /// Creates an empty NbtCompound with the given name.
        /// </summary>
        /// <param name="name">Name of this NbtCompound.</param>
        public NbtCompound(string name) {
            Name = name;
            Value = new List<NbtTag>();
        }

        /// <summary>
        /// Creates a new NbtCompound populated with <paramref name="value"/>.
        /// </summary>
        /// <param name="name">Name of this NbtCompound.</param>
        /// <param name="value">Value of this NbtCompound.</param>
        public NbtCompound(string name, IEnumerable<NbtTag> value) {
            Name = name;
            Value = new List<NbtTag>(value);
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Compound('" + Name + "'): " + Value.Count + " entries\n");
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("{\n");

            foreach (NbtTag element in Value) {
                element.PrettyPrint(sb, indentString, currentIndentAmount + 1);
            }

            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("}\n");
        }
    }
}
