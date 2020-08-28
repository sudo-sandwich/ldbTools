using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Long_Array.
    /// </summary>
    public class NbtLongArray : NbtTag {
        /// <inheritdoc/>
        public override TagType Type => TagType.LongArray;

        /// <summary>
        /// Value of this NbtLongArray.
        /// </summary>
        public IList<long> Value { get; }

        /// <summary>
        /// Creates an empty NbtLongArray with the given name.
        /// </summary>
        /// <param name="name">Name of this NbtLongArray.</param>
        public NbtLongArray(string name) {
            Name = name;
            Value = new List<long>();
        }

        /// <summary>
        /// Creates a new NbtLongArray populated with <paramref name="value"/>.
        /// </summary>
        /// <param name="name">Name of this NbtLongArray.</param>
        /// <param name="value">Value of this NbtLongArray.</param>
        public NbtLongArray(string name, IEnumerable<long> value) {
            Name = name;
            Value = new List<long>(value);
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Long_Array('" + Name + "'): " + Value.Count + " entries\n");
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("{\n");

            foreach (long element in Value) {
                sb.Insert(sb.Length, indentString, currentIndentAmount + 1);
                sb.Append(element + ",\n");
            }

            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("}\n");
        }
    }
}
