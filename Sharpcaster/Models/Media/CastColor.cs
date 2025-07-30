using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents a color for Cast text track styling with support for multiple formats
    /// </summary>
    [JsonConverter(typeof(CastColorConverter))]
    public readonly struct CastColor : IEquatable<CastColor>
    {
        private readonly string _value;

        public CastColor(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Creates a color from RGBA values (0-255)
        /// </summary>
        /// <param name="red">Red component (0-255)</param>
        /// <param name="green">Green component (0-255)</param>
        /// <param name="blue">Blue component (0-255)</param>
        /// <param name="alpha">Alpha component (0-255), default is 255 (opaque)</param>
        /// <returns>A CastColor instance</returns>
        public static CastColor FromRgba(byte red, byte green, byte blue, byte alpha = 255)
        {
            return new CastColor($"#{red:X2}{green:X2}{blue:X2}{alpha:X2}");
        }

        /// <summary>
        /// Creates a color from RGB values (0-255) with full opacity
        /// </summary>
        /// <param name="red">Red component (0-255)</param>
        /// <param name="green">Green component (0-255)</param>
        /// <param name="blue">Blue component (0-255)</param>
        /// <returns>A CastColor instance</returns>
        public static CastColor FromRgb(byte red, byte green, byte blue)
        {
            return FromRgba(red, green, blue, 255);
        }

        /// <summary>
        /// Creates a color from a hex string (e.g., "#FF0000", "#FF0000FF", "FF0000")
        /// </summary>
        /// <param name="hex">Hex color string</param>
        /// <returns>A CastColor instance</returns>
        /// <exception cref="ArgumentException">Thrown when the hex format is invalid</exception>
        public static CastColor FromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("Hex color cannot be null or empty", nameof(hex));

            hex = hex.Trim();
            if (hex.StartsWith("#", StringComparison.InvariantCulture))
                hex = hex.Substring(1);

            if (hex.Length != 6 && hex.Length != 8)
                throw new ArgumentException("Hex color must be 6 or 8 characters long", nameof(hex));

            // Validate hex characters
            if (!System.Text.RegularExpressions.Regex.IsMatch(hex, "^[0-9A-Fa-f]+$"))
                throw new ArgumentException("Invalid hex color format", nameof(hex));

            // Add alpha if not provided
            if (hex.Length == 6)
                hex += "FF";

            return new CastColor($"#{hex.ToUpperInvariant()}");
        }

        /// <summary>
        /// Creates a color from HSL values
        /// </summary>
        /// <param name="hue">Hue (0-360)</param>
        /// <param name="saturation">Saturation (0-100)</param>
        /// <param name="lightness">Lightness (0-100)</param>
        /// <param name="alpha">Alpha (0-1), default is 1.0 (opaque)</param>
        /// <returns>A CastColor instance</returns>
        public static CastColor FromHsl(double hue, double saturation, double lightness, double alpha = 1.0)
        {
            // Normalize values
            hue = ((hue % 360) + 360) % 360; // Ensure 0-360
            saturation = Math.Max(0, Math.Min(100, saturation)) / 100.0;
            lightness = Math.Max(0, Math.Min(100, lightness)) / 100.0;
            alpha = Math.Max(0, Math.Min(1, alpha));

            // Convert HSL to RGB
            var (r, g, b) = HslToRgb(hue, saturation, lightness);
            
            return FromRgba(r, g, b, (byte)(alpha * 255));
        }

        private static (byte r, byte g, byte b) HslToRgb(double h, double s, double l)
        {
            double r, g, b;

            if (s == 0)
            {
                r = g = b = l; // achromatic
            }
            else
            {

                var q = l < 0.5 ? l * (1 + s) : l + s - (l * s);
                var p = (2 * l) - q;
                var hNorm = h / 360.0;
                
                r = Hue2rgb(p, q, hNorm + (1.0 / 3));
                g = Hue2rgb(p, q, hNorm);
                b = Hue2rgb(p, q, hNorm - (1.0 / 3));
            }

            return ((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        private static double Hue2rgb(double p, double q, double t)
        {
            if (t < 0) t++;
            if (t > 1) t--;
            if (t < 1.0 / 6) return p + ((q - p) * 6 * t);
            if (t < 1.0 / 2) return q;
            if (t < 2.0 / 3) return p + ((q - p) * ((2.0 / 3) - t) * 6);
            return p;
        }

        /// <summary>
        /// Returns the color as a hex string suitable for Cast API
        /// </summary>
        /// <returns>Hex color string</returns>
        public override string ToString()
        {
            return _value ?? "#000000FF";
        }

        /// <summary>
        /// Implicit conversion from string to CastColor
        /// </summary>
        /// <param name="hex">Hex color string</param>
        public static implicit operator CastColor(string hex)
        {
            return FromHex(hex);
        }

        /// <summary>
        /// Implicit conversion from CastColor to string
        /// </summary>
        /// <param name="color">CastColor instance</param>
        public static implicit operator string(CastColor color)
        {
            return color.ToString();
        }

        /// <summary>
        /// Determines whether the specified CastColor is equal to the current CastColor
        /// </summary>
        /// <param name="other">The CastColor to compare with the current CastColor</param>
        /// <returns>true if the specified CastColor is equal to the current CastColor; otherwise, false</returns>
        public bool Equals(CastColor other)
        {
            return string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current CastColor
        /// </summary>
        /// <param name="obj">The object to compare with the current CastColor</param>
        /// <returns>true if the specified object is equal to the current CastColor; otherwise, false</returns>
        public override bool Equals(object? obj)
        {
            return obj is CastColor other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this CastColor
        /// </summary>
        /// <returns>A 32-bit signed integer hash code</returns>
        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(_value ?? "#000000FF");
        }

        /// <summary>
        /// Determines whether two specified CastColor instances are equal
        /// </summary>
        /// <param name="left">The first CastColor to compare</param>
        /// <param name="right">The second CastColor to compare</param>
        /// <returns>true if the CastColor instances are equal; otherwise, false</returns>
        public static bool operator ==(CastColor left, CastColor right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified CastColor instances are not equal
        /// </summary>
        /// <param name="left">The first CastColor to compare</param>
        /// <param name="right">The second CastColor to compare</param>
        /// <returns>true if the CastColor instances are not equal; otherwise, false</returns>
        public static bool operator !=(CastColor left, CastColor right)
        {
            return !left.Equals(right);
        }
    }

    /// <summary>
    /// Predefined common colors
    /// </summary>
    public static class CastColors
    {
        public static readonly CastColor Transparent = new("#00000000");
        public static readonly CastColor Black = new("#000000FF");
        public static readonly CastColor White = new("#FFFFFFFF");
        public static readonly CastColor Red = new("#FF0000FF");
        public static readonly CastColor Green = new("#00FF00FF");
        public static readonly CastColor Blue = new("#0000FFFF");
        public static readonly CastColor Yellow = new("#FFFF00FF");
        public static readonly CastColor Cyan = new("#00FFFFFF");
        public static readonly CastColor Magenta = new("#FF00FFFF");
        public static readonly CastColor Gray = new("#808080FF");
        public static readonly CastColor DarkGray = new("#404040FF");
        public static readonly CastColor LightGray = new("#C0C0C0FF");
    }

    /// <summary>
    /// JSON converter for CastColor
    /// </summary>
    public class CastColorConverter : JsonConverter<CastColor>
    {
        public override CastColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return string.IsNullOrEmpty(value) ? CastColors.Transparent : CastColor.FromHex(value);
        }

        public override void Write(Utf8JsonWriter writer, CastColor value, JsonSerializerOptions options)
        {
            writer?.WriteStringValue(value.ToString());
        }
    }
}