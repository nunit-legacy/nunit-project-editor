// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NUnit.ProjectEditor
{
	/// <summary>
	/// Static methods for manipulating project paths, including both directories
	/// and files. Some synonyms for System.Path methods are included as well.
	/// </summary> 
	public class PathUtils
	{
		public const uint FILE_ATTRIBUTE_DIRECTORY  = 0x00000010;  
		public const uint FILE_ATTRIBUTE_NORMAL     = 0x00000080;  
		public const int MAX_PATH = 256;
        public static readonly char[] SEPARATORS;

        // We use a slash unless it's not valid - currently, it is valid on all known platforms
        protected static char PreferredDirectorySeparatorChar =
            Path.DirectorySeparatorChar == '/' || Path.AltDirectorySeparatorChar == '/'
                ? '/' : Path.DirectorySeparatorChar;

        static PathUtils()
        {
            SEPARATORS = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        }

		#region Public methods

		public static bool IsAssemblyFileType( string path )
		{
			string extension = Path.GetExtension( path ).ToLower();
			return extension == ".dll" || extension == ".exe";
		}

		/// <summary>
		/// Returns the relative path from a base directory to another
		/// directory or file.
		/// </summary>
		public static string RelativePath( string from, string to )
		{
			if (from == null)
				throw new ArgumentNullException (from);
			if (to == null)
				throw new ArgumentNullException (to);

            from = Canonicalize(from);
            to = Canonicalize(to);

            string toPathRoot = Path.GetPathRoot(to);
            if (toPathRoot == null || toPathRoot == string.Empty)
                return to;
            string fromPathRoot = Path.GetPathRoot(from);

            if (!PathsEqual(toPathRoot, fromPathRoot))
                return null;

            string fromNoRoot = from.Substring(fromPathRoot.Length);
            string toNoRoot = to.Substring(toPathRoot.Length);

            string[] _from = SplitPath(fromNoRoot);
            string[] _to = SplitPath(toNoRoot);

			StringBuilder sb = new StringBuilder (Math.Max (from.Length, to.Length));

			int last_common, min = Math.Min (_from.Length, _to.Length);
			for (last_common = 0; last_common < min;  ++last_common) 
			{
                if (!PathsEqual(_from[last_common], _to[last_common]))
                    break;
            }

			if (last_common < _from.Length)
				sb.Append ("..");
			for (int i = last_common + 1; i < _from.Length; ++i) 
			{
				sb.Append (PathUtils.PreferredDirectorySeparatorChar).Append ("..");
			}

			if (sb.Length > 0)
				sb.Append (PathUtils.PreferredDirectorySeparatorChar);
			if (last_common < _to.Length)
				sb.Append (_to [last_common]);
			for (int i = last_common + 1; i < _to.Length; ++i) 
			{
				sb.Append (PathUtils.PreferredDirectorySeparatorChar).Append (_to [i]);
			}

			return sb.ToString ();
		}

		/// <summary>
		/// Return the canonical form of a path, using '/' as the directory separator
		/// </summary>
		public static string Canonicalize( string path )
		{
			List<string> parts = new List<string>( path.Split( SEPARATORS ) );

			for( int index = 0; index < parts.Count; )
			{
				string part = (string)parts[index];
		
				switch( part )
				{
					case ".":
                        parts.RemoveAt(index);
						break;
				
					case "..":
                        if (index > 0 && parts[index-1] != "..")
                        {
                            parts.RemoveAt(index);
                            parts.RemoveAt(--index);
                        }
                        else
                            index++;
						break;
					default:
						index++;
						break;
				}
			}
	
			return String.Join( PreferredDirectorySeparatorChar.ToString(), parts.ToArray() );
		}

		/// <summary>
		/// True if the two paths are the same. However, two paths
		/// to the same file or directory using different network
		/// shares or drive letters are not treated as equal.
		/// </summary>
		public static bool SamePath( string path1, string path2, bool ignoreCase = false )
		{
			return string.Compare( Canonicalize(path1), Canonicalize(path2), ignoreCase ) == 0;
		}

		/// <summary>
		/// True if the two paths are the same or if the second is
		/// directly or indirectly under the first. Note that paths 
		/// using different network shares or drive letters are 
		/// considered unrelated, even if they end up referencing
		/// the same subtrees in the file system.
		/// </summary>
		public static bool SamePathOrUnder( string path1, string path2, bool ignoreCase = false )
		{
			path1 = Canonicalize( path1 );
			path2 = Canonicalize( path2 );

			int length1 = path1.Length;
			int length2 = path2.Length;

			// if path1 is longer, then path2 can't be under it
			if ( length1 > length2 )
				return false;

			// if lengths are the same, check for equality
			if ( length1 == length2 )
				return PathsEqual( path1, path2 );

			// path 2 is longer than path 1: see if initial parts match
			if ( !PathsEqual( path1, path2.Substring( 0, length1 ) ) )
				return false;
			
			// must match through or up to a directory separator boundary
			return	path2[length1-1] == PreferredDirectorySeparatorChar ||
				path2[length1] == PreferredDirectorySeparatorChar;
		}

		#endregion

		#region Helper Methods

        // Split a path, removing any empty entries
        private static string[] SplitPath(string path)
        {
            return path.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
        }

        // Compare two paths or path segments for equality. If both paths
        // start with X:, we ignore case for the whole comparison
        private static bool PathsEqual(string path1, string path2)
        {
            if (path1.Length >= 2 && path1[1] == ':' && path2.Length >= 2 && path2[1] == ':')
                return path1.Equals(path2, StringComparison.InvariantCultureIgnoreCase);
            else
                return path1.Equals(path2, StringComparison.InvariantCulture);
        }

        #endregion
	}
}
