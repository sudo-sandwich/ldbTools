using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Int_Array.
    /// </summary>
    public class NbtIntArray : NbtTag {
        /// <summary>
        /// Value of this NbtIntArray.
        /// </summary>
        public IList<int> Value { get; }

        /// <summary>
        /// Creates an empty NbtIntArray with the given name.
        /// </summary>
        /// <param name="name">Name of this NbtIntArray.</param>
        public NbtIntArray(string name) {
            Name = name;
            Value = new List<int>();
        }

        /// <summary>
        /// Creates a new NbtIntArray populated with <paramref name="value"/>.
        /// </summary>
        /// <param name="name">Name of this NbtIntArray.</param>
        /// <param name="value">Value of this NbtIntArray.</param>
        public NbtIntArray(string name, IEnumerable<int> value) {
            Name = name;
            Value = new List<int>(value);
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Int_Array('" + Name + "'): " + Value.Count + " entries\n");
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("{\n");

            foreach (int element in Value) {
                sb.Insert(sb.Length, indentString, currentIndentAmount + 1);
                sb.Append(element + ",\n");
            }

            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("}\n");
        }
    }
}
