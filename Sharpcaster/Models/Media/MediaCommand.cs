using Sharpcaster.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Media commands that can be supported by the media player
    /// Based on Google Cast SDK Command enum
    /// This is a flags enum where multiple commands can be combined using bitwise operations
    /// </summary>
    [Flags]
    [JsonConverter(typeof(MediaCommandEnumConverter))]
    public enum MediaCommand
    {
        /// <summary>
        /// Pause command
        /// </summary>
        PAUSE = 1,

        /// <summary>
        /// Seek command
        /// </summary>
        SEEK = 2,

        /// <summary>
        /// Stream volume command
        /// </summary>
        STREAM_VOLUME = 4,

        /// <summary>
        /// Stream mute command
        /// </summary>
        STREAM_MUTE = 8,

        /// <summary>
        /// Queue next command
        /// </summary>
        QUEUE_NEXT = 64,

        /// <summary>
        /// Queue previous command
        /// </summary>
        QUEUE_PREV = 128,

        /// <summary>
        /// Queue shuffle command
        /// </summary>
        QUEUE_SHUFFLE = 256,

        /// <summary>
        /// Skip ad command
        /// </summary>
        SKIP_AD = 512,

        /// <summary>
        /// Queue repeat all command
        /// </summary>
        QUEUE_REPEAT_ALL = 1024,

        /// <summary>
        /// Queue repeat one command
        /// </summary>
        QUEUE_REPEAT_ONE = 2048,

        /// <summary>
        /// Queue repeat command (combined repeat modes), 3072
        /// </summary>
        QUEUE_REPEAT = QUEUE_REPEAT_ALL | QUEUE_REPEAT_ONE,

        /// <summary>
        /// Edit tracks command
        /// </summary>
        EDIT_TRACKS = 4096,

        /// <summary>
        /// Playback rate command
        /// </summary>
        PLAYBACK_RATE = 8192,

        /// <summary>
        /// All basic media commands, 12303
        /// </summary>
        ALL_BASIC_MEDIA = PAUSE | SEEK | STREAM_VOLUME | STREAM_MUTE | EDIT_TRACKS | PLAYBACK_RATE,

        /// <summary>
        /// Like command
        /// </summary>
        LIKE = 16384,

        /// <summary>
        /// Dislike command
        /// </summary>
        DISLIKE = 32768,

        /// <summary>
        /// Follow command
        /// </summary>
        FOLLOW = 65536,

        /// <summary>
        /// Unfollow command
        /// </summary>
        UNFOLLOW = 131072,

        /// <summary>
        /// Stream transfer command
        /// </summary>
        STREAM_TRANSFER = 262144
    }

    /// <summary>
    /// Extension methods for MediaCommand flags enum
    /// </summary>
    public static class MediaCommandExtensions
    {
        /// <summary>
        /// All available individual media commands (excluding combined flags like ALL_BASIC_MEDIA and QUEUE_REPEAT)
        /// </summary>
        private static readonly MediaCommand[] AllIndividualCommands =
        {
            MediaCommand.PAUSE,
            MediaCommand.SEEK,
            MediaCommand.STREAM_VOLUME,
            MediaCommand.STREAM_MUTE,
            MediaCommand.QUEUE_NEXT,
            MediaCommand.QUEUE_PREV,
            MediaCommand.QUEUE_SHUFFLE,
            MediaCommand.SKIP_AD,
            MediaCommand.QUEUE_REPEAT_ALL,
            MediaCommand.QUEUE_REPEAT_ONE,
            MediaCommand.EDIT_TRACKS,
            MediaCommand.PLAYBACK_RATE,
            MediaCommand.LIKE,
            MediaCommand.DISLIKE,
            MediaCommand.FOLLOW,
            MediaCommand.UNFOLLOW,
            MediaCommand.STREAM_TRANSFER
        };

        /// <summary>
        /// Gets all individual commands that are set in the combined flags value
        /// </summary>
        /// <param name="combinedCommands">The combined MediaCommand flags value</param>
        /// <returns>An enumerable of individual MediaCommand values</returns>
        public static IEnumerable<MediaCommand> GetIndividualCommands(this MediaCommand combinedCommands)
        {
            return AllIndividualCommands.Where(command => combinedCommands.HasFlag(command));
        }

        /// <summary>
        /// Checks if a specific command is supported in the combined flags value
        /// </summary>
        /// <param name="combinedCommands">The combined MediaCommand flags value</param>
        /// <param name="command">The command to check for</param>
        /// <returns>True if the command is supported, false otherwise</returns>
        public static bool SupportsCommand(this MediaCommand combinedCommands, MediaCommand command)
        {
            return combinedCommands.HasFlag(command);
        }

        /// <summary>
        /// Gets a list of command names from the combined flags value
        /// </summary>
        /// <param name="combinedCommands">The combined MediaCommand flags value</param>
        /// <returns>A list of command names</returns>
        public static IReadOnlyCollection<string> GetCommandNames(this MediaCommand combinedCommands)
        {
            return GetIndividualCommands(combinedCommands)
                .Select(cmd => cmd.ToString())
                .ToList()
                .AsReadOnly();
        }
    }
}