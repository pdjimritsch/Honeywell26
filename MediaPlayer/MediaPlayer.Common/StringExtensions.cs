using System.Runtime.InteropServices;
using System.Security;

namespace MediaPlayer.Common;

/// <summary>
/// 
/// </summary>
public static partial class StringExtensions
{
    #region Extensions

    /// <summary>
    /// Checks whetther the provided content contains whitespaces or null characters (0x0).
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static bool IsContentEmpty(this string? content) =>
        string.IsNullOrEmpty(content ?? string.Empty) || string.IsNullOrWhiteSpace(content ?? string.Empty);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool IsContentEqualTo(this string? lhs, string? rhs, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
    {
        if (lhs.IsContentEmpty() ^ rhs.IsContentEmpty())
        {
            return false;
        }
        else if (!lhs.IsContentEmpty() && !rhs.IsContentEmpty())
        {
            if (!(lhs ?? string.Empty).Equals(rhs ?? string.Empty, comparison))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string? GetSecuredContent(this SecureString? content)
    {
        if (content != null)
        {
            IntPtr reference = IntPtr.Zero;
            try
            {
                reference = Marshal.SecureStringToGlobalAllocUnicode(content);

                if (reference != IntPtr.Zero)
                {
                    return Marshal.PtrToStringUni(reference);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(reference);
            }
        }
        return null;
    }

    #endregion

    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains(IEnumerable<string> collection, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            if (!(collection is object) || ((collection is object) && !collection.Any()))
            {
                return true;
            }

            return false;
        }

        if (collection is not null)
        {
            return collection.Any(x =>
                !string.IsNullOrEmpty(x) && x.Trim().Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="character"></param>
    /// <returns></returns>
    public static int GetPosition(this string? content, char character)
    {
        if (string.IsNullOrEmpty(content)) return -1;

        var len = content.Length;

        for (var pos = 0; pos < len; pos++)
        {
            var c = content[pos];

            if (c == character) return pos;
        }

        return -1;
    }

    /// <summary>
    /// Acquires the first position within the provided content that satifies the algorithm constraint.
    /// </summary>
    /// <param name="content">
    /// Textual content
    /// </param>
    /// <param name="algorithm">
    /// Algorithmic constraint.
    /// </param>
    /// <returns></returns>
    public static int GetFirstPosition(this string? content, Func<char, bool> algorithm)
    {
        if (string.IsNullOrEmpty(content)) return -1;

        var len = content.Length;

        for (var pos = 0; pos < len; pos++)
        {
            var c = content[pos];

            if (algorithm(c)) return pos;
        }

        return -1;

    }

    /// <summary>
    /// Generates a random string of a specfic length
    /// </summary>
    /// <param name="minimum"></param>
    /// <returns></returns>
    public static string Generate(int minimum)
    {
        if (minimum <= 0) return string.Empty;

        using var generator = RandomNumberGenerator.Create();

        var sequence = new byte[minimum];

        generator.GetNonZeroBytes(sequence);

        return Encoding.UTF8.GetString(sequence);
    }

    /// <summary>
    /// Compares the text based content for equality.
    /// </summary>
    /// <param name="lhs">
    /// Text based content.
    /// </param>
    /// <param name="rhs">
    /// Text based content.
    /// </param>
    /// <returns>
    /// True if the context insensitive content are identical,
    /// otherwise False.
    /// </returns>
    public static bool IsIdentical(this string lhs, string rhs)
    {
        if (ReferenceEquals(null, lhs) ^ ReferenceEquals(null, rhs))
        {
            return false;
        }
        else if (!ReferenceEquals(null, lhs) &&
                (!lhs?.Equals(rhs, StringComparison.InvariantCultureIgnoreCase) ?? false))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool IsContentEqualTo(this IEnumerable<string> lhs, IEnumerable<string> rhs)
    {
        if ((lhs is object) ^ (rhs is object)) return false;

        if ((lhs is object) && (rhs is object))
        {
            if (lhs.SequenceEqual(rhs)) return true;

            if (lhs.ToArray().Length != rhs.ToArray().Length) return false;

            foreach (var x in lhs)
            {
                if (!rhs.Any(y => 
                    !string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y) && 
                    y.Equals(x, StringComparison.OrdinalIgnoreCase))) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool IsContentEqualTo(this IReadOnlyDictionary<string, string> lhs, IReadOnlyDictionary<string, string> rhs)
    {
        if ((lhs is object) ^ (rhs is object)) return false;

        if ((lhs is object) && (rhs is object))
        {
            if (lhs.Count != rhs.Count) return false;

            if (lhs.Keys.SequenceEqual(rhs.Keys) && lhs.Values.SequenceEqual(rhs.Values)) return true;

            foreach (var key in lhs.Keys)
            {
                if (!rhs.Keys.Contains(key)) return false;
            }

            foreach (var key in lhs.Values)
            {
                if (!(rhs.Values.Contains(key))) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool IsContentEqualTo(this IReadOnlyDictionary<string, object> lhs, IReadOnlyDictionary<string, object> rhs)
    {
        if ((lhs is object) ^ (rhs is object)) return false;

        if ((lhs is object) && (rhs is object))
        {
            if (lhs.Count != rhs.Count) return false;

            if (lhs.Keys.SequenceEqual(rhs.Keys) && lhs.Values.SequenceEqual(rhs.Values)) return true;

            foreach (var key in lhs.Keys)
            {
                if (!rhs.Keys.Contains(key)) return false;
            }

            var confirmation = new List<bool>();

            foreach (var key in lhs.Values)
            {
                var confirmed = false;

                foreach (var value in rhs.Values)
                {
                    if (Object.Equals(key, value))
                    {
                        confirmed = true;
                    }

                    if (confirmed) break;
                }

                confirmation.Add(confirmed);
            }

            return confirmation.All(x => x);
        }

        return true;
    }

    /// <summary>
    /// Checks whether the provided content appears identical to the reversed conternt
    /// in appearance
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static bool IsPalindrome(this string? content)
    {
        if (content != null && content.Length > 0)
        {
            var characters = content.ToCharArray();

            if (characters != null)
            {
                var reversed = characters.Reverse();

                return reversed.SequenceEqual(characters);
            }

            return false;
        }
        return true;
    }

    #endregion
}