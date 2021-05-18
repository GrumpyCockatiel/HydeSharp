using System;
using System.Text.RegularExpressions;

namespace Raydreams.HydeSharp
{
    public static class Extensions
    {
        /// <summary>Parse the filename of a post file</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Post TryParseFilename(this string path)
        {
            Regex regex = new Regex( @"(\d\d\d\d)-(\d\d)-(\d\d)-(.+)\.md$", RegexOptions.IgnoreCase );
            Match match = regex.Match( path );

            if ( !match.Success || match.Groups.Count < 5 )
                return null;

            if ( !Int32.TryParse( match.Groups[1].Value, out int year ) )
                return null;

            if ( !Int32.TryParse( match.Groups[2].Value, out int month ) )
                return null;

            if ( !Int32.TryParse( match.Groups[3].Value, out int day ) )
                return null;

            return new Post { Year = year, Month = month, Day = day, Name = match.Groups[4].Value };
        }

    }
}
