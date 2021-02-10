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
        /// <inheritdoc/>
        public override TagType Type => TagType.Byte;

        /// <summary>
        /// Value of this NbtByte.
        /// </summary>
        public sbyte Value { get; set; }

        /// <summary>
        /// Creates a new NbtByte with the given name and value.
        /// </summary>
        /// <param name="name">Name of this NbtByte.</param>
        /// <param name="value">Value of this NbtByte. false = 0, true = 1.</param>
        public NbtByte(string name, sbyte value) {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Creates a new NbtByte with the given name and based on a boolean value.
        /// </summary>
        /// <param name="name">Name of this NbtByte.</param>
        /// <param name="value">Value of this NbtByte.</param>
        public NbtByte(string name, bool value) {
            Name = name;
            Value = Convert.ToSByte(value);
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Byte('" + Name + "'): " + Value + '\n');
        }
    }
}
