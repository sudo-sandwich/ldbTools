using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Double
    /// </summary>
    public class NbtDouble : NbtTag {
        /// <inheritdoc/>
        public override TagType Type => TagType.Double;

        /// <summary>
        /// Value of this NbtDouble.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Creates a new NbtDouble with the given name and value.
        /// </summary>
        /// <param name="name">Name of this NbtDouble.</param>
        /// <param name="value">Value of this NbtDouble.</param>
        public NbtDouble(string name, double value) {
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Double('" + Name + "'): " + Value + '\n');
        }
    }
}
