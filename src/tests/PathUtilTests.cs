// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.IO;
using NUnit.Framework;

namespace NUnit.ProjectEditor.Tests
{
	[TestFixture]
	public class PathUtilTests : PathUtils
	{
        [TestCase(".", ExpectedResult="")]
        [TestCase("./.", ExpectedResult="")]
        [TestCase("././.", ExpectedResult="")]
        [TestCase("..", ExpectedResult = "..")]
        [TestCase("../..", ExpectedResult = "../..")]
        [TestCase("../../..", ExpectedResult = "../../..")]
        [TestCase("./..", ExpectedResult= "..")]
        [TestCase("../.", ExpectedResult= "..")]
	    [TestCase(@"C:\folder1\.\folder2\..\file.tmp", ExpectedResult="C:/folder1/file.tmp")]
		[TestCase(@"folder1\.\folder2\..\file.tmp", ExpectedResult="folder1/file.tmp")]
		[TestCase(@"folder1\folder2\.\..\file.tmp", ExpectedResult="folder1/file.tmp")]
		[TestCase(@"folder1\folder2\..\.\..\file.tmp", ExpectedResult="file.tmp")]
		[TestCase(@"folder1\folder2\..\..\..\file.tmp", ExpectedResult=@"../file.tmp")]
		[TestCase("/folder1/./folder2/../file.tmp", ExpectedResult="/folder1/file.tmp")]
		[TestCase("folder1/./folder2/../file.tmp", ExpectedResult="folder1/file.tmp")]
		[TestCase("folder1/folder2/./../file.tmp", ExpectedResult="folder1/file.tmp")]
		[TestCase("folder1/folder2/.././../file.tmp", ExpectedResult="file.tmp")]
		[TestCase("folder1/folder2/../../../file.tmp", ExpectedResult="../file.tmp")]
        public string CanonicalizeTest(string path)
        {
            return PathUtils.Canonicalize(path);
        }

        [TestCase("/folder1", "/folder1/folder2/folder3", ExpectedResult="folder2/folder3")]
        [TestCase("/folder1", "/folder2/folder3", ExpectedResult="../folder2/folder3")]
        [TestCase("/folder1", "bin/debug", ExpectedResult="bin/debug")]
        [TestCase("/folder", "/other/folder", ExpectedResult="../other/folder")]
        [TestCase("/a/b/c", "/a/d", ExpectedResult="../../d")]
        [TestCase("/a/b", "/a/b", ExpectedResult="")]
        [TestCase("/", "/", ExpectedResult="")]
        // First filePath consisting just of a root:
        [TestCase("/", "/folder1/folder2", ExpectedResult="folder1/folder2")]
        // Trailing directory separator in first filePath shall be ignored:
        [TestCase("/folder1/", "/folder1/folder2/folder3", ExpectedResult="folder2/folder3")]
        // Case-sensitive behaviour:
        [TestCase("/folder1", "/Folder1/Folder2/folder3", ExpectedResult="../Folder1/Folder2/folder3")]
        public string RelativePathTest(string path1, string path2)
        {
            return PathUtils.RelativePath(path1, path2);
        }

        [TestCase(@"c:\folder1", @"c:\folder1\folder2\folder3", ExpectedResult="folder2/folder3")]
        [TestCase(@"c:\folder1", @"c:\folder2\folder3", ExpectedResult="../folder2/folder3")]
        [TestCase(@"c:\folder1", @"bin\debug", ExpectedResult="bin/debug")]
        [TestCase(@"C:\folder", @"D:\folder", ExpectedResult=null)]
        [TestCase(@"C:\", @"D:\", ExpectedResult=null)]
        [TestCase(@"C:", @"D:", ExpectedResult=null)]
        [TestCase(@"C:\folder1", @"C:\folder1", ExpectedResult="")]
        [TestCase(@"C:\", @"C:\", ExpectedResult="")]
        // First filePath consisting just of a root:
        [TestCase(@"C:\", @"C:\folder1\folder2", ExpectedResult="folder1/folder2")]
        // Trailing directory separator in first filePath shall be ignored:
        [TestCase(@"c:\folder1\", @"c:\folder1\folder2\folder3", ExpectedResult="folder2/folder3")]
        // Case-insensitive behaviour, preserving 2nd filePath directories in result:
        [TestCase(@"C:\folder1", @"c:\folder1\Folder2\Folder3", ExpectedResult="Folder2/Folder3")]
        [TestCase(@"c:\folder1", @"C:\Folder2\folder3", ExpectedResult="../Folder2/folder3")]
        [Platform(Exclude = "Linux")]
        public string RelativePathTest_Windows(string path1, string path2)
        {
            return PathUtils.RelativePath(path1, path2);
        }

        [TestCase("/folder1/folder2/folder3", "/folder1/./folder2/junk/../folder3", ExpectedResult = true)]
        [TestCase("/folder1/folder2/", "/folder1/./folder2/junk/../folder3", ExpectedResult = true)]
        [TestCase("/folder1/folder2", "/folder1/./folder2/junk/../folder3", ExpectedResult = true)]
        [TestCase("/folder1/folder2", "/folder1/./Folder2/junk/../folder3", ExpectedResult = false)]
        [TestCase("/folder1/folder2", "/folder1/./folder22/junk/../folder3", ExpectedResult = false)]
        [TestCase("/", "/", ExpectedResult = true)]
        [TestCase("/", "/bin/debug", ExpectedResult = true)]
        [TestCase(@"C:\folder1\folder2\folder3", @"c:\folder1\.\folder2\junk\..\folder3", ExpectedResult = true)]
        [TestCase(@"C:\folder1\folder2\", @"c:\folder1\.\folder2\junk\..\folder3", ExpectedResult = true)]
        [TestCase(@"C:\folder1\folder2", @"c:\folder1\.\folder2\junk\..\folder3", ExpectedResult = true)]
        [TestCase(@"C:\folder1\folder2", @"c:\folder1\.\Folder2\junk\..\folder3", ExpectedResult = true)]
        [TestCase(@"C:\folder1\folder2", @"c:\folder1\.\folder22\junk\..\folder3", ExpectedResult = false)]
        [TestCase(@"C:\folder1\folder2ile.tmp", @"D:\folder1\.\folder2\folder3\file.tmp", ExpectedResult = false)]
        [TestCase(@"C:\", @"D:\", ExpectedResult = false)]
        [TestCase(@"C:\", @"c:\", ExpectedResult = true)]
        [TestCase(@"C:\", @"c:\bin\debug", ExpectedResult = true)]
        public bool SamePathOrUnderTest(string path1, string path2)
        {
            return PathUtils.SamePathOrUnder(path1, path2);
        }
    }
}
