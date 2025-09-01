using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// User actions that can be performed on media
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<UserAction>))]
    public enum UserAction
    {
        /// <summary>
        /// User liked the content
        /// </summary>
        LIKE,

        /// <summary>
        /// User disliked the content
        /// </summary>
        DISLIKE,

        /// <summary>
        /// User followed the content/artist
        /// </summary>
        FOLLOW,

        /// <summary>
        /// User unfollowed the content/artist
        /// </summary>
        UNFOLLOW,

        /// <summary>
        /// User flagged the content
        /// </summary>
        FLAG,

        /// <summary>
        /// User skipped the content
        /// </summary>
        SKIP_AD
    }
}