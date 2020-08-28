using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Long
    /// </summary>
    public class NbtLong : NbtTag {
        /// <summary>
        /// Value of this NbtLong.
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// Creates a new NbtLong with the given name and value.
        /// </summary>
        /// <param name="name">Name of this NbtLong.</param>
        /// <param name="value">Value of this NbtLong.</param>
        public NbtLong(string name, long value) {
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Long('" + Name + "'): " + Value + "L\n");
        }
    }
}
