using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Short.
    /// </summary>
    public class NbtShort : NbtTag {
        /// <summary>
        /// Value of this NbtShort.
        /// </summary>
        public short Value { get; set; }

        /// <summary>
        /// Creates a new NbtShort with the given name and value.
        /// </summary>
        /// <param name="name">Name of this NbtShort.</param>
        /// <param name="value">Value of this NbtShort.</param>
        public NbtShort(string name, short value) {
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Short('" + Name + "'): " + Value + '\n');
        }
    }
}
