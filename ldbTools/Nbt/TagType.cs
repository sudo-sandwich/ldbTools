using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ldbTools.Nbt {
    /// <summary>
    /// An enum backed by a byte value that can be translated to NBT tag IDs.
    /// </summary>
    public enum TagType : byte {
        End         = 0x00,
        Byte        = 0x01,
        Short       = 0x02,
        Int         = 0x03,
        Long        = 0x04,
        Float       = 0x05,
        Double      = 0x06,
        ByteArray   = 0x07,
        String      = 0x08,
        List        = 0x09,
        Compound    = 0x0A,
        IntArray    = 0x0B,
        LongArray   = 0x0C
    }
}
