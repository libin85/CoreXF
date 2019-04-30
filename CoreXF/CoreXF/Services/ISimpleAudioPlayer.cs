using System;
using System.IO;
using System.Threading.Tasks;

namespace CoreXF
{
    public enum PlayerState
    {
        Playing,
        Paused,
        Stopped,
        Loading,
        Failed
    }

    public class MediaSource
    {
        public string Title { get; set; }

        public string Uri { get; set; }

        public static MediaSource FromFile(string path)
        {
            return new MediaSource { Uri = path };
        }
    }

    /// <summary>
    /// Interface for <see cref="ISimpleAudioPlayer"/>
    /// </summary>
    public interface ISimpleAudioPlayer : IDisposable
    {
        ///<Summary>
        /// Raised when audio player state is changed 
        ///</Summary>
        event EventHandler<PlayerState> StateChanged;

        ///<Summary>
        /// Raised when media item is changed 
        ///</Summary>
        event EventHandler<MediaSource> MediaItemChanged;

        /// <summary>
        /// Current player state
        /// </summary>
        PlayerState State { get; }

        /// <summary>
        /// Current media item (null if is not playing)
        /// </summary>
        MediaSource Current { get; }

        ///<Summary>
        /// Length of audio
        ///</Summary>
        TimeSpan Duration { get; }

        ///<Summary>
        /// Current position of audio playback
        ///</Summary>
        TimeSpan Position { get; }

        ///<Summary>
        /// Playback volume 0 to 1 where 0 is no-sound and 1 is full volume
        ///</Summary>
        float Volume { get; set; }

        ///<Summary>
        /// Load audio file from local storage
        ///</Summary>
        Task<bool> Load(string filePath);

        ///<Summary>
        /// Load audio file from local storage
        ///</Summary>
        Task<bool> Load(Stream stream);

        ///<Summary>
        /// Begin playback
        ///</Summary>
        Task Play();

        ///<Summary>
        /// Pause playback if playing (can resume)
        ///</Summary>
        Task Pause();

        ///<Summary>
        /// Stop and reset playack
        ///</Summary>
        Task Stop();

        ///<Summary>
        /// Set the current playback position (in seconds)
        ///</Summary>
        bool Seek(TimeSpan position);
    }

    public static class AudioPlayerExtensions
    {
        public static Task PlayOrPause(this ISimpleAudioPlayer player)
        {
            if (player.State == PlayerState.Playing)
            {
                player.Pause();
            }

            return player.Play();
        }
    }
}
