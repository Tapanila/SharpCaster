using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents the styling information for text tracks
    /// </summary>
    public class TextTrackStyle
    {
        /// <summary>
        /// Gets or sets the background color
        /// </summary>
        [JsonPropertyName("backgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CastColor? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the edge color
        /// </summary>
        [JsonPropertyName("edgeColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CastColor? EdgeColor { get; set; }

        /// <summary>
        /// Gets or sets the edge type
        /// </summary>
        [JsonPropertyName("edgeType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextTrackEdgeType? EdgeType { get; set; }

        /// <summary>
        /// Gets or sets the font family
        /// </summary>
        [JsonPropertyName("fontFamily")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the font generic family
        /// </summary>
        [JsonPropertyName("fontGenericFamily")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextTrackFontGenericFamily? FontGenericFamily { get; set; }

        /// <summary>
        /// Gets or sets the font scale
        /// </summary>
        [JsonPropertyName("fontScale")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? FontScale { get; set; }

        /// <summary>
        /// Gets or sets the font style
        /// </summary>
        [JsonPropertyName("fontStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextTrackFontStyle? FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the foreground color
        /// </summary>
        [JsonPropertyName("foregroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CastColor? ForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the window color
        /// </summary>
        [JsonPropertyName("windowColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CastColor? WindowColor { get; set; }

        /// <summary>
        /// Gets or sets the window corner radius
        /// </summary>
        [JsonPropertyName("windowRoundedCornerRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? WindowRoundedCornerRadius { get; set; }

        /// <summary>
        /// Gets or sets the window type
        /// </summary>
        [JsonPropertyName("windowType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextTrackWindowType? WindowType { get; set; }
    }

    /// <summary>
    /// Text track edge types
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TextTrackEdgeType>))]
    public enum TextTrackEdgeType
    {
        NONE,
        OUTLINE,
        DROP_SHADOW,
        RAISED,
        DEPRESSED
    }

    /// <summary>
    /// Text track font generic families
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TextTrackFontGenericFamily>))]
    public enum TextTrackFontGenericFamily
    {
        SANS_SERIF,
        MONOSPACED_SANS_SERIF,
        SERIF,
        MONOSPACED_SERIF,
        CASUAL,
        CURSIVE,
        SMALL_CAPITALS
    }

    /// <summary>
    /// Text track font styles
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TextTrackFontStyle>))]
    public enum TextTrackFontStyle
    {
        NORMAL,
        BOLD,
        BOLD_ITALIC,
        ITALIC
    }

    /// <summary>
    /// Text track window types
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TextTrackWindowType>))]
    public enum TextTrackWindowType
    {
        NONE,
        NORMAL,
        ROUNDED_CORNERS
    }
}