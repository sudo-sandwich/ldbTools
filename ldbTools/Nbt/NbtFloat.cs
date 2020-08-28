using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Float.
    /// </summary>
    public class NbtFloat : NbtTag {
        /// <summary>
        /// Value of this NbtFloat.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Creates a new NbtFloat with the given name and value.
        /// </summary>
        /// <param name="name">Name of this NbtFloat.</param>
        /// <param name="value">Value of this NbtFloat.</param>
        public NbtFloat(string name, float value) {
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Float('" + Name + "'): " + Value + '\n');
        }
    }
}
