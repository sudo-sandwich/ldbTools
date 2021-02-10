using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// Represents a TAG_List.
    /// </summary>
    /// <remarks>All NBT tags in a TAG_List are unnamed and not prefixed by their TagType. This class does not ensure that all its elements are of the same type. It also does not ensure that all tags are unnamed.</remarks>
    public class NbtList : NbtTag {
        /// <inheritdoc/>
        public override TagType Type => TagType.List;

        /// <summary>
        /// Value of this NbtList. This list is not type checked to ensure that all of the elements are of the same type.
        /// </summary>
        public IList<NbtTag> Value { get; }

        /// <summary>
        /// Type of the NbtTags in this NbtList.
        /// </summary>
        /// <remarks>This is more for convinience than functionality since the type of elements in this NbtList is not enforced.</remarks>
        public TagType ContentType { get; set; }

        /// <summary>
        /// Creates a new NbtList with the given name and type.
        /// </summary>
        /// <param name="name">Name of this NbtList</param>
        /// <param name="contentType">Type of all elements in this NbtList.</param>
        public NbtList(string name, TagType contentType) {
            Name = name;
            ContentType = contentType;
            Value = new List<NbtTag>();
        }

        /// <summary>
        /// Creates a new NbtList with the given name and type, populated with <paramref name="value"/>.
        /// </summary>
        /// <param name="name">Name of this NbtList</param>
        /// <param name="type">Type of all elements in this NbtList.</param>
        /// <param name="value">Value of this NbtList.</param>
        public NbtList(string name, TagType type, IEnumerable<NbtTag> value) {
            Name = name;
            ContentType = type;
            Value = new List<NbtTag>(value);
        }

        /// <inheritdoc/>
        public override void PrettyPrint(StringBuilder sb, string indentString, int currentIndentAmount) {
            sb.Insert(sb.Length, indentString, currentIndentAmount);
            sb.Append("TAG_List('" + Name + "'): " + Value.Count + " entries, TAG_" + ContentType + "\n");
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
