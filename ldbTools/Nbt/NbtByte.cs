using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Byte.
    /// </summary>
    public class NbtByte : NbtTag {
        /// <summary>
        /// Value of this NbtByte.
        /// </summary>
        public sbyte Value { get; set; }

        /// <summary>
        /// Creates a new NbtByte with the given name and value.
        /// </summary>
        /// <param name="name">Name of this NbtByte.</param>
        /// <param name="value">Value of this NbtByte.</param>
        public NbtByte(string name, sbyte value) {
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Byte('" + Name + "'): " + Value + '\n');
        }
    }
}
