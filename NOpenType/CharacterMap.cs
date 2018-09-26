using System;
using System.Collections.Generic;

namespace NRasterizer
{
    class CharacterMap
    {
        private readonly int _segCount;
        private readonly ushort[] _startCode;
        private readonly ushort[] _endCode;
        private readonly ushort[] _idDelta;
        private readonly ushort[] _idRangeOffset;
        private readonly ushort[] _glyphIdArray;

        internal CharacterMap(int segCount, ushort[] startCode, ushort[] endCode, ushort[] idDelta, ushort[] idRangeOffset, ushort[] glyphIdArray)
        {
            _segCount = segCount;
            _startCode = startCode;
            _endCode = endCode;
            _idDelta = idDelta;
            _idRangeOffset = idRangeOffset;
            _glyphIdArray = glyphIdArray;
        }

        public bool IsCharacterInMap(UInt32 character)
        {
            for (int i = 0; i < _segCount; i++)
            {
                if (_startCode[i] <= character && character <= _endCode[i])
                {
                    if (_idRangeOffset == null || _idDelta == null)
                    {
                        // 1:1 mapping
                        return _glyphIdArray[character] != 0;
                    }

                    if (_idRangeOffset[i] == 0)
                    {
                        return true;
                    }
                    else
                    {
                        var offset = _idRangeOffset[i] / 2 + (character - _startCode[i]);

                        if (_glyphIdArray[offset - _idRangeOffset.Length + i] == 0) return false;
                        return true;
                    }
                }
            }
            return false;
        }

        public int CharacterToGlyphIndex(UInt32 character)
        {
            return (int)RawCharacterToGlyphIndex(character);
        }

        public uint RawCharacterToGlyphIndex(UInt32 character)
        {
            // TODO: Fast fegment lookup using bit operations?
            for (int i = 0; i < _segCount; i++)
            {
                if (_startCode[i] <= character && character <= _endCode[i])
                {
                    if (_idRangeOffset == null || _idDelta == null)
                    {
                        // 1:1 mapping
                        return _glyphIdArray[character];
                    }

                    if (_idRangeOffset[i] == 0)
                    {
                        return (uint)((int)character + _idDelta[i]) & 65535;
                    }
                    else
                    {
                        var offset = _idRangeOffset[i] / 2 + (character - _startCode[i]);

                        // I want to thank Microsoft for this clever pointer trick
                        // TODO: What if the value fetched is inside the _idRangeOffset table?
                        // TODO: e.g. (offset - _idRangeOffset.Length + i < 0)
                        return _glyphIdArray[offset - _idRangeOffset.Length + i];
                    }
                }
            }
            return 0;
        }
    }
}
