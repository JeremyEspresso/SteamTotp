using System.Buffers.Binary;
using System.Diagnostics;
using System.Security.Cryptography;

namespace JeremyEspresso.SteamTotp;

public static class SteamTotp
{
    /// <summary>
    /// Generate a Steam TOTP authentication code.
    /// </summary>
    /// <param name="sharedSecret">Your TOTP shared_secret as a base64 string.</param>
    /// <returns>5 character TOTP code.</returns>
    public static string GetAuthCode(string sharedSecret)
    {
        Span<byte> secret = stackalloc byte[20];
        Convert.TryFromBase64Chars(sharedSecret.AsSpan(), secret, out _);

        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Span<byte> hmacResult = stackalloc byte[20];
        Span<byte> buffer = stackalloc byte[8];

        BinaryPrimitives.WriteUInt32BigEndian(buffer[4..], (uint)time / 30);
        var bytesWritten = HMACSHA1.HashData(secret, buffer, hmacResult);

        Debug.Assert(bytesWritten == 20);

        var start = hmacResult[19] & 0x0f;
        var truncatedHash = hmacResult.Slice(start, 4);
        truncatedHash.Reverse();

        var fullCode = BitConverter.ToUInt32(truncatedHash) & 0x7FFFFFFF;

        return string.Create(5, fullCode, (span, state) =>
        {
            for (var i = 0; i < 5; i++)
            {
                span[i] = TotpChars[(int)state % TotpChars.Length];
                state /= (uint)TotpChars.Length;
            }
        });
    }

    private const string TotpChars = "23456789BCDFGHJKMNPQRTVWXY";
}