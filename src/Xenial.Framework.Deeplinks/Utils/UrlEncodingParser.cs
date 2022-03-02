using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Xenial.Framework.Deeplinks.Utils;

/// <summary>
/// A query string or UrlEncoded form parser and editor 
/// class that allows reading and writing of urlencoded
/// key value pairs used for query string and HTTP 
/// form data.
/// 
/// Useful for parsing and editing querystrings inside
/// of non-Web code that doesn't have easy access to
/// the HttpUtility class.                
/// </summary>
/// <remarks>
/// Supports multiple values per key
/// </remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1010:Generic interface should also be implemented")]
public sealed class UrlEncodingParser : NameValueCollection
{

    /// <summary>
    /// Holds the original Url that was assigned if any
    /// Url must contain // to be considered a url
    /// </summary>
    private string Url { get; set; }


    /// <summary>
    /// Always pass in a UrlEncoded data or a URL to parse from
    /// unless you are creating a new one from scratch.
    /// </summary>
    /// <param name="queryStringOrUrl">
    /// Pass a query string or raw Form data, or a full URL.
    /// If a URL is parsed the part prior to the ? is stripped
    /// but saved. Then when you write the original URL is 
    /// re-written with the new query string.
    /// </param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
    public UrlEncodingParser(string? queryStringOrUrl = null)
    {
        Url = string.Empty;

        if (!string.IsNullOrEmpty(queryStringOrUrl))
        {
            Parse(queryStringOrUrl!);
        }
    }


    /// <summary>
    /// Assigns multiple values to the same key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values"></param>
    public void SetValues(string key, IEnumerable<string> values!!)
    {
        foreach (var val in values)
        {
            Add(key, val);
        }
    }

    /// <summary>
    /// Parses the query string into the internal dictionary
    /// and optionally also returns this dictionary
    /// </summary>
    /// <param name="query">
    /// Query string key value pairs or a full URL. If URL is
    /// passed the URL is re-written in Write operation
    /// </param>
    /// <returns></returns>
    public NameValueCollection Parse(string query)
    {
        if (Uri.IsWellFormedUriString(query, UriKind.Absolute))
        {
            Url = query;
        }

        if (string.IsNullOrEmpty(query))
        {
            Clear();
        }
        else
        {
            var index = query.IndexOf('?');
            if (index > -1)
            {
                if (query.Length >= index + 1)
                {
                    query = query.Substring(index + 1);
                }
            }

            var pairs = query.Split('&');
            foreach (var pair in pairs)
            {
                var index2 = pair.IndexOf('=');
                if (index2 > 0)
                {
                    Add(pair.Substring(0, index2), pair.Substring(index2 + 1));
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Writes out the urlencoded data/query string or full URL based 
    /// on the internally set values.
    /// </summary>
    /// <returns>urlencoded data or url</returns>
    public override string ToString()
    {
        var query = string.Empty;
        foreach (string key in Keys)
        {
            var values = GetValues(key);
            foreach (var val in values)
            {
                query += key + "=" + Uri.EscapeUriString(val) + "&";
            }
        }
        query = query.Trim('&');

        if (!string.IsNullOrEmpty(Url))
        {
            if (Url.Contains("?"))
            {
                query = Url.Substring(0, Url.IndexOf('?') + 1) + query;
            }
            else
            {
                query = Url + "?" + query;
            }
        }

        return query;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetBoolean(string? key,
#if NET5_0_OR_GREATER
            [NotNullWhen(true)]
#endif
        out bool value
    )
    {
        foreach (string k in Keys)
        {
            if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                var val = this[k];
                if (bool.TryParse(val, out value))
                {
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetString(string? key,
#if NET5_0_OR_GREATER
            [NotNullWhen(true)]
#endif
        out string value
    )
    {
        foreach (string k in Keys)
        {
            if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = this[k];
            }
        }

        value = default!;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetInt(string? key,
#if NET5_0_OR_GREATER
            [NotNullWhen(true)]
#endif
        out int value
    )
    {
        foreach (string k in Keys)
        {
            if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                var val = this[k];
                if (int.TryParse(val, out value))
                {
                    return true;
                }
            }
        }

        value = default;
        return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetGuid(string? key,
#if NET5_0_OR_GREATER
            [NotNullWhen(true)]
#endif
        out Guid value
    )
    {
        foreach (string k in Keys)
        {
            if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                var val = this[k];
                if (Guid.TryParse(val, out value))
                {
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
}
