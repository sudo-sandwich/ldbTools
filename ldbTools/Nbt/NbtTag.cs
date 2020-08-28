using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Base class for all NBT tags.
    /// </summary>
    public abstract class NbtTag {
        /// <summary>
        /// Gets or sets the name of this NbtTag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the TagType associated with this NbtTag.
        /// </summary>
        public abstract TagType Type { get; }

        /// <summary>
        /// Appends the contents of this NbtTag to a StringBuilder, indented with <paramref name="indentString"/>.
        /// </summary>
        /// <param name="sb">StringBuilder to append the contents of this NbtTag to.</param>
        /// <param name="indentString">String used to indent nested tags.</param>
        /// <param name="currentIndentAmount">Current level of indent if this tag is nested in multiple NbtTags.</param>
        public abstract void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount);

        /// <summary>
        /// Returns a string that represents this NbtTag, indented with "  ".
        /// </summary>
        /// <returns>The string representation of this NbtTag.</returns>
        public override string ToString() {
            return ToString("  ");
        }

        /// <summary>
        /// Returns a string that represents this NbtTag, indented with <paramref name="indentString"/>.
        /// </summary>
        /// <returns>The string representation of this NbtTag.</returns>
        public string ToString(string indentString) {
            StringBuilder sb = new StringBuilder();
            PrettyPrint(sb, indentString, 0);
            return sb.ToString();
        }
    }
}
