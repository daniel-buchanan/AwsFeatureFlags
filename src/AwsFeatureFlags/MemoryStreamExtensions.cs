using System;
using System.IO;
using System.Text;

namespace AwsFeatureFlags;

internal static class MemoryStreamExtensions {
    /// <summary>
    /// Decode a memory stream to a string.
    /// </summary>
    /// <param name="content">The <see cref="MemoryStream"/> to decode.</param>
    /// <returns>The string representation of the stream.</returns>
    public static string DecodeToString(this MemoryStream content) {
        var uniEncoding = new UnicodeEncoding();

        using var memoryStream = content;
        memoryStream.Seek(0, SeekOrigin.Begin);
        var byteArray = new byte[memoryStream.Length];
        var count = memoryStream.Read(byteArray, 0, 20);

        while (count < memoryStream.Length) {
            byteArray[count++] = Convert.ToByte(memoryStream.ReadByte());
        }

        var charArray = new char[uniEncoding.GetCharCount(byteArray, 0, count)];
        uniEncoding.GetDecoder().GetChars(byteArray, 0, count, charArray, 0);
            
        var decodedBytes = Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(charArray)));
        var result = System.Text.Encoding.UTF8.GetString(decodedBytes);
        return result;
    }
}