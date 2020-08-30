using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_Compound.
    /// </summary>
    public class NbtCompound : NbtTag {
        /// <inheritdoc/>
        public override TagType Type => TagType.Compound;

        /// <summary>
        /// Value of this NbtCompound.
        /// </summary>
        public IList<NbtTag> Value { get; }

        /// <summary>
        /// Creates an empty NbtCompound with the given name.
        /// </summary>
        /// <param name="name">Name of this NbtCompound.</param>
        public NbtCompound(string name) {
            Name = name;
            Value = new List<NbtTag>();
        }

        /// <summary>
        /// Creates a new NbtCompound populated with <paramref name="value"/>.
        /// </summary>
        /// <param name="name">Name of this NbtCompound.</param>
        /// <param name="value">Value of this NbtCompound.</param>
        public NbtCompound(string name, IEnumerable<NbtTag> value) {
            Name = name;
            Value = new List<NbtTag>(value);
        }

        /// <summary>
        /// Creates a new NbtCompound populated with <paramref name="value"/>.
        /// </summary>
        /// <param name="name">Name of this NbtCompound.</param>
        /// <param name="value">Value of this NbtCompound.</param>
        public NbtCompound(string name, params NbtTag[] value) {
            Name = name;
            Value = new List<NbtTag>(value);
        }

        /// <summary>
        /// Gets the tag with the specified name. May return <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Type of tag to search for.</typeparam>
        /// <param name="tagName">Name of tag to search for.</param>
        /// <returns>Tag with the specified name. Returns null if a tag matching the name was not found.</returns>
        public T Get<T>(string tagName) where T : NbtTag {
            foreach (T element in Value.Where(item => item is T)) {
                if (element.Name == tagName) {
                    return element;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_Compound('" + Name + "'): " + Value.Count + " entries\n");
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("{\n");

            foreach (NbtTag element in Value) {
                element.PrettyPrint(sb, indentString, currentIndentAmount + 1);
            }

            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("}\n");
        }
    }
}
