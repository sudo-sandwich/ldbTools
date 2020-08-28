using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Int.
    /// </summary>
    public class NbtInt : NbtTag {
        /// <inheritdoc/>
        public override TagType Type => TagType.Int;

        /// <summary>
        /// Value of this NbtInt.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Creates a new NbtInt with the given name and value.
        /// </summary>
        /// <param name="name">Name of this NbtInt.</param>
        /// <param name="value">Value of this NbtInt.</param>
        public NbtInt(string name, int value) {
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Int('" + Name + "'): " + Value + '\n');
        }
    }
}
