#region Copyright (C) 2008 Giacomo Stelluti Scala
//
// Copy Directory Tree: ArchiveInfo.cs
//
// Author:
//   Giacomo Stelluti Scala (giacomo.stelluti@gmail.com)
//
// Copyright (C) 2008 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System.Collections.Generic;

namespace tenoriTool;

public class ArchiveInfo
{
    private ulong _ArchiveSize;
    private uint _EntriesCount;
    private List<ArchiveEntryInfo> _Entries;


    public ulong ArchiveSize { get { return _ArchiveSize; } set { _ArchiveSize = value; } }
    public uint EntriesCount { get { return _EntriesCount; } set { _EntriesCount = value; } }
    public List<ArchiveEntryInfo> Entries { get { return _Entries; } set { _Entries = value; } }

    public ArchiveInfo()
    {
        _Entries = new List<ArchiveEntryInfo>();
    }
}

public class ArchiveEntryInfo
{
    string _EntryName;
    ulong _EntryOffset;
    uint _EntrySize;
    uint _EntryNameOffset;

    public string EntryName { get { return _EntryName; } set { _EntryName = value; } }
    public ulong EntryOffset { get { return _EntryOffset; } set { _EntryOffset = value; } }
    public uint EntrySize { get { return _EntrySize; } set { _EntrySize = value; } }
    public uint EntryNameOffset { get { return _EntryNameOffset; } set { _EntryNameOffset = value; } }
    //
    // Summary:
    //     Returns a System.String that contains entry info.
    //
    // Returns:
    //     A System.String that contains entry info.
    public override string ToString()
    {
        return string.Join(",", EntryName, "@" + EntryOffset.ToString("X8"), string.Format(new FileSizeFormatProvider(), "0x{0:X8} ({0:fs})", EntrySize));
    }
}