using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeNet;

namespace LiteLoaderFixerR.Utils
{
    internal class FunctionEngine
    {
        public record FunctionPattern(string ExactName, byte[] Signature);

        private readonly PeFile pe;

        public FunctionEngine(PeFile pe_) => pe = pe_;

        private PeNet.Header.Pe.ImageSectionHeader? FindSectionByRva(ulong function_rva)
        {
            if (pe.ImageSectionHeaders == null)
            {
                return null;
            }

            var sections = pe.ImageSectionHeaders;

            foreach (var section in sections)
            {
                var va = section.VirtualAddress;
                var size = section.VirtualSize;

                if (va <= function_rva && function_rva < va + size)
                {
                    return section;
                }
            }

            return null;
        }

        private PeNet.Header.Pe.ImageSectionHeader? FindSectionByOffset(ulong function_offset)
        {
            if (pe.ImageSectionHeaders == null)
            {
                return null;
            }

            var sections = pe.ImageSectionHeaders;

            foreach (var section in sections)
            {
                var offset = section.PointerToRawData;
                var size = section.SizeOfRawData;

                if (offset <= function_offset && function_offset < offset + size)
                {
                    return section;
                }
            }

            return null;
        }

        public List<FunctionPattern>? GetFunctionSignature(string containing_name, uint size)
        {
            if (pe.ExportedFunctions == null || pe.ImageNtHeaders == null)
            {
                return null;
            }

            var ret = new List<FunctionPattern>();
            var exports = pe.ExportedFunctions;

            foreach (var export in exports)
            {
                if (export.Name == null)
                {
                    continue;
                }

                var name = export.Name;

                if (name.Contains(containing_name))
                {
                    var function_rva = export.Address;
                    var section = FindSectionByRva(function_rva)!;
                    var function_offset = function_rva - section.VirtualAddress;
                    var section_size = section.SizeOfRawData;

                    if (section_size - function_offset < size)
                    {
                        size = section_size - function_offset;
                    }

                    var pattern = new FunctionPattern(name, pe.RawFile.AsSpan(
                         section.PointerToRawData + function_offset,
                         size).ToArray());

                    ret.Add(pattern);
                }
            }

            return ret;
        }

        private static List<ulong>? SearchSignature(byte[] data, byte[] signature)
        {
            if (data.LongLength < signature.LongLength)
            {
                return null;
            }

            var ret = new List<ulong>();

            for (ulong i = 0; i < (ulong)data.LongLength - (ulong)signature.LongLength; i++)
            {
                bool is_same = true;

                for (ulong j = 0; j < (ulong)signature.LongLength; j++)
                {
                    if (data[i + j] != signature[j])
                    {
                        is_same = false;
                        break;
                    }
                }

                if (is_same)
                {
                    ret.Add(i);
                }
            }

            return ret;
        }

        public List<ulong>? GetFunctionRva(byte[] signature)
        {
            var data = pe.RawFile.ToArray();
            var offset_list = SearchSignature(data, signature);

            if (offset_list == null || pe.ImageSectionHeaders == null)
            {
                return null;
            }

            var ret = new List<ulong>();

            foreach (var offset in offset_list)
            {
                var section = FindSectionByOffset(offset);

                if (section == null)
                {
                    continue;
                }

                ret.Add(offset - section.PointerToRawData + section.VirtualAddress);
            }

            return ret;
        }
    }
}