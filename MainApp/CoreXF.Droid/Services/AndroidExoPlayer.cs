using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Audio;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;

namespace CoreXF.Droid.Services
{
    /// <summary>
    /// Реализация <see cref="IAudioPlayer" /> на Android с бэкэндом на ExoPlayer
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class AndroidExoPlayer : Java.Lang.Object, ISimpleAudioPlayer, IPlayerEventListener
    {
        public event EventHandler<PlayerState> StateChanged;
        public event EventHandler<MediaSource> MediaItemChanged;

        private SimpleExoPlayer player;
        private TimeSpan savedPosition;

        public PlayerState State { get; private set; }
        public MediaSource Current { get; private set; }

        public TimeSpan Duration
        {
            get
            {
                long duration = player.Duration;

                if (duration > 0)
                    return TimeSpan.FromMilliseconds(duration);

                return TimeSpan.Zero;
            }
        }

        public TimeSpan Position
        {
            get
            {
                long position = player.CurrentPosition;

                if (position > 0)
                    return TimeSpan.FromMilliseconds(position);

                return TimeSpan.Zero;
            }
        }

        public float Volume
        {
            get => player.Volume;
            set => player.Volume = value;
        }

        public AndroidExoPlayer()
        {
            var audioAttributes = new AudioAttributes.Builder()
                .SetUsage(C.UsageMedia)
                .SetContentType(C.ContentTypeMusic)
                .Build();

            player = ExoPlayerFactory.NewSimpleInstance(Application.Context, new DefaultTrackSelector());

            player.AudioAttributes = audioAttributes;
            player.AddListener(this);

            State = PlayerState.Stopped;
        }

        public Task<bool> Load(string filePath)
        {
            if (filePath.NotNullAndEmpty())
            {
                try
                {
                    var mediaUri = Android.Net.Uri.Parse(filePath);

                    var mediaSource = new ExtractorMediaSource(mediaUri,
                        new FileDataSourceFactory(),
                        new DefaultExtractorsFactory(), null, null);

                    player.Prepare(mediaSource, true, true);

                    Current = MediaSource.FromFile(filePath);

                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load player {ex}");
                }
            }

            return Task.FromResult(false);
        }

        public async Task<bool> Load(Stream stream)
        {
            var tempFolder = Path.GetTempPath();
            var tempFile = Guid.NewGuid().ToString("N");

            var path = Path.Combine(tempFolder, tempFile);

            using (var fileStream = new FileStream(path, FileMode.CreateNew))
            {
                await stream.CopyToAsync(fileStream);
            }

            return await Load(path);
        }

        public Task Play()
        {
            if (savedPosition > TimeSpan.Zero)
            {
                if (Seek(savedPosition))
                {
                    savedPosition = TimeSpan.Zero;
                }
            }

            if (State == PlayerState.Stopped)
            {
                Seek(TimeSpan.Zero);
            }

            player.PlayWhenReady = true;

            return Task.CompletedTask;
        }

        public Task Pause()
        {
            savedPosition = Position;

            //player.Stop();
            player.PlayWhenReady = false;

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            player.Stop(true);
            player.PlayWhenReady = false;

            return Task.CompletedTask;
        }

        public bool Seek(TimeSpan position)
        {
            if (!player.IsCurrentWindowSeekable)
                return false;

            Debug.WriteLine($"Player seek to {position}");

            player.SeekTo(position.Milliseconds);

            return true;
        }

        public new void Dispose()
        {
            player.RemoveListener(this);
            player.Release();
            player = null;

            base.Dispose();
        }

        #region IPlayerEventListener

        public void OnLoadingChanged(bool p0)
        {
        }

        public void OnPlaybackParametersChanged(PlaybackParameters p0)
        {
        }

        public void OnPlayerError(ExoPlaybackException exception)
        {
            Debug.WriteLine($"EXOPLAYER ERROR: {exception}");
        }

        public void OnPlayerStateChanged(bool playWhenReady, int stateCode)
        {
            Debug.WriteLine($"Player state changed to '{stateCode}'");

            switch (stateCode)
            {
                case Player.StateBuffering:
                    State = PlayerState.Loading;
                    break;
                case Player.StateEnded:
                    State = PlayerState.Stopped;
                    break;
                case Player.StateIdle:
                    State = PlayerState.Paused;
                    break;
                case Player.StateReady:
                    State = PlayerState.Playing;
                    break;
                default:
                    State = PlayerState.Failed;
                    break;
            }

            if (State != PlayerState.Paused)
            {
                savedPosition = TimeSpan.Zero;
            }

            StateChanged?.Invoke(this, State);
        }

        public void OnPositionDiscontinuity(int p0)
        {
        }

        public void OnRepeatModeChanged(int p0)
        {
        }

        public void OnSeekProcessed()
        {
        }

        public void OnShuffleModeEnabledChanged(bool p0)
        {
        }

        public void OnTimelineChanged(Timeline p0, Java.Lang.Object p1, int p2)
        {
        }

        public void OnTracksChanged(TrackGroupArray p0, TrackSelectionArray p1)
        {
            MediaItemChanged?.Invoke(this, Current);
        }

        #endregion
    }
}