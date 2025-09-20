using Microsoft.Extensions.Configuration;
using SharpCaster.Console.Models;
using System.Text.Json;

namespace SharpCaster.Console.Services
{
    public class PlaylistService
    {
        public Playlist[] Playlists;

        public PlaylistService()
        {
            var playlists = File.ReadAllText("userSettings.json");
            if ( string.IsNullOrWhiteSpace(playlists))
            {
                Playlists = [];
                return;
            }

            var userSettings = JsonSerializer.Deserialize<UserSettingsModel>(playlists, ConsoleJsonContext.Default.UserSettingsModel);

            if (userSettings == null)
            {
                Playlists = [];
                return;
            }

            Playlists = userSettings.Playlists;
        }

        public bool IsPlaylistId(string arg)
        {
            return Playlists.First(p => p.Name == arg) != null;
        }

    }
}
