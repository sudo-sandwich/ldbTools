using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Byte_Array.
    /// </summary>
    public class NbtByteArray : NbtTag {
        /// <summary>
        /// Value of this NbtByteArray.
        /// </summary>
        public IList<sbyte> Value { get; }

        /// <summary>
        /// Creates an empty NbtByteArray with the given name.
        /// </summary>
        /// <param name="name">Name of this NbtByteArray.</param>
        public NbtByteArray(string name) {
            Name = name;
            Value = new List<sbyte>();
        }

        /// <summary>
        /// Creates a new NbtByteArray populated with <paramref name="value"/>.
        /// </summary>
        /// <param name="name">Name of this NbtByteArray.</param>
        /// <param name="value">Value of this NbtByteArray.</param>
        public NbtByteArray(string name, IEnumerable<sbyte> value) {
            Name = name;
            Value = new List<sbyte>(value);
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Byte_Array('" + Name + "'): " + Value.Count + " entries\n");
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("{\n");

            foreach (sbyte element in Value) {
                sb.Insert(sb.Length, indentString, currentIndentAmount + 1);
                sb.Append(element + ",\n");
            }

            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("}\n");
        }
    }
}
