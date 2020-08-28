using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_String.
    /// </summary>
    public class NbtString : NbtTag {
        /// <summary>
        /// Value of this NbtString.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new NbtString with the given name and value.
        /// </summary>
        /// <param name="name">Name of this NbtString.</param>
        /// <param name="value">Value of this NbtString.</param>
        public NbtString(string name, string value) {
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_String('" + Name + "'): '" + Value + "'\n");
        }
    }
}
