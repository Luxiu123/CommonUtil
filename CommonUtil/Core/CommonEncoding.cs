using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CommonUtil.Core;

public class CommonEncoding {
    private static readonly Regex UTF8Regex = new(@"&#x(\w{4});", RegexOptions.IgnoreCase);
    private static readonly Regex UnicodeRegex = new(@"\\u(\w{4})", RegexOptions.IgnoreCase);

    /// <summary>
    /// UTF8 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UTF8Encode(string s) {
        //&#x4F60;
        byte[] vs = Encoding.Unicode.GetBytes(s);
        var sb = new StringBuilder();
        for (int i = 0; i < vs.Length >> 1; i++) {
            byte n1 = vs[(i << 1) + 1];
            byte n2 = vs[i << 1];
            // ASCII 字符
            if (n1 == 0) {
                sb.Append(s[i]);
                continue;
            }
            string s1 = Convert.ToString(n1, 16);
            string s2 = Convert.ToString(n2, 16);
            // 补全 4 字符
            if (n1 < 10) {
                s1 = "0" + s1;
            }
            if (n2 < 10) {
                s2 = "0" + s2;
            }
            sb.Append($"&#x{s1}{s2};");
        }
        return sb.ToString();
    }

    /// <summary>
    /// UTF8 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UTF8Decode(string s) {
        return UTF8Regex.Replace(s, m => {
            var value = m.Groups[1].Value;
            var s1 = value[..2];
            var s2 = value[2..];
            byte[] bytes = new byte[] { Convert.ToByte(s2, 16), Convert.ToByte(s1, 16) };
            return Encoding.Unicode.GetString(bytes);
        });
    }

    /// <summary>
    /// Unicode 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UnicodeEncode(string s) {
        byte[] vs = Encoding.Unicode.GetBytes(s);
        var sb = new StringBuilder();
        for (int i = 0; i < vs.Length >> 1; i++) {
            byte n1 = vs[(i << 1) + 1];
            byte n2 = vs[i << 1];
            //// ASCII 字符
            //if (n1 == 0) {
            //    sb.Append(s[i]);
            //    continue;
            //}
            string s1 = Convert.ToString(n1, 16);
            string s2 = Convert.ToString(n2, 16);
            // 补全 4 字符
            if (n1 < 10) {
                s1 = "0" + s1;
            }
            if (n2 < 10) {
                s2 = "0" + s2;
            }
            sb.Append($"\\u{s1}{s2}");
        }
        return sb.ToString();
    }

    /// <summary>
    /// Unicode 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UnicodeDecode(string s) {
        return UnicodeRegex.Replace(s, m => {
            var value = m.Groups[1].Value;
            var s1 = value[..2];
            var s2 = value[2..];
            byte[] bytes = new byte[] { Convert.ToByte(s2, 16), Convert.ToByte(s1, 16) };
            return Encoding.Unicode.GetString(bytes);
        });
    }

    /// <summary>
    /// URL 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UrlEncode(string s) {
        return HttpUtility.UrlEncode(s);
    }

    /// <summary>
    /// URL 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UrlDecode(string s) {
        return HttpUtility.UrlDecode(s);
    }

    /// <summary>
    /// Hex 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string HexEncode(string s) {
        var sb = new StringBuilder();
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        foreach (var b in bytes) {
            sb.Append($"%{Convert.ToString(b, 16)}");
        }
        return sb.ToString();
    }

    /// <summary>
    /// Hex 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string HexDecode(string s) {
        return Encoding.UTF8.GetString(s.Trim()
                                        .Trim('%')
                                        .Split('%')
                                        .Select(n => Convert.ToByte(n, 16))
                                        .ToArray()
                                        );
    }

}
