using System;
using System.IO;

namespace SuperGameSystemBasic.Audio
{
    /// <summary>
    ///     Generates musical tones at specified frequencies, and saves them as a 16-bit wave file.
    /// </summary>
    public class SongWriter : IDisposable
    {
        # region Private Fields

        private readonly WaveWriter16Bit _writer;

        private short _defaultVolume;

        # endregion

        # region Constructors

        /// <summary>
        ///     Instantiates a new SongWriter object.
        /// </summary>
        /// <param name="output">The Stream object to write the wave to.</param>
        /// <param name="tempo">The speed (in beats-per-minute) of playback.</param>
        /// <remarks>
        ///     When this object is disposed, it will close its underlying stream.  To change this default behavior,
        ///     you can call an overloaded constructor which takes a closeUnderlyingStream parameter.
        /// </remarks>
        public SongWriter(Stream output, double tempo) : this(output, tempo, true)
        {
        }

        /// <summary>
        ///     Instantiates a new SongWriter object.
        /// </summary>
        /// <param name="output">The Stream object to write the wave to.</param>
        /// <param name="tempo">The speed (in beats-per-minute) of playback.</param>
        /// <param name="closeUnderlyingStream">
        ///     Determines whether to close the the stream that the SongWriter is writing to when the SongWriter is closed.
        /// </param>
        public SongWriter(Stream output, double tempo, bool closeUnderlyingStream)
            : this(output, tempo, 8000, closeUnderlyingStream)
        {
        }

        /// <summary>
        ///     Instantiates a new SongWriter object.
        /// </summary>
        /// <param name="output">The Stream object to write the wave to.</param>
        /// <param name="tempo">The speed (in beats-per-minute) of playback.</param>
        /// <param name="defaultVolume">The default volume of the notes generated, ranging from 0 to short.MaxValue.</param>
        /// <remarks>
        ///     When this object is disposed, it will close its underlying stream.  To change this default behavior,
        ///     you can call an overloaded constructor which takes a closeUnderlyingStream parameter.
        /// </remarks>
        public SongWriter(Stream output, double tempo, short defaultVolume)
            : this(output, tempo, defaultVolume, true)
        {
        }

        /// <summary>
        ///     Instantiates a new SongWriter object.
        /// </summary>
        /// <param name="output">The Stream object to write the wave to.</param>
        /// <param name="tempo">The speed (in beats-per-minute) of playback.</param>
        /// <param name="defaultVolume">The default volume of the notes generated, ranging from 0 to short.MaxValue.</param>
        /// <param name="closeUnderlyingStream">
        ///     Determines whether to close the the stream that the SongWriter is writing to when the SongWriter is closed.
        /// </param>
        public SongWriter(Stream output, double tempo, short defaultVolume, bool closeUnderlyingStream)
        {
            Tempo = tempo;

            _writer = new WaveWriter16Bit(output, 44100, false, closeUnderlyingStream);

            _defaultVolume = defaultVolume;
        }

        # endregion

        # region Properties

        /// <summary>
        ///     Gets the tempo (in beats-per-minute) of the song.
        /// </summary>
        public double Tempo { get; }

        /// <summary>
        ///     Gets or sets the current beat of the song.
        /// </summary>
        public double CurrentBeat
        {
            get => _writer.CurrentSample * Tempo / 60 / _writer.SampleRate;
            set => _writer.CurrentSample = (long) (value * _writer.SampleRate * 60 / Tempo);
        }

        /// <summary>
        ///     Gets or sets the default volume of the song, ranging from 0 to short.MaxValue.
        /// </summary>
        public short DefaultVolume
        {
            get => _defaultVolume;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "Volume must be greater than or equal to zero.");

                _defaultVolume = value;
            }
        }

        # endregion

        # region Methods

        /// <summary>
        ///     Adds a note to the song.
        /// </summary>
        /// <param name="frequency">The frequency of the note to add.</param>
        /// <param name="length">The length, in beats, of the note.</param>
        /// <remarks>
        ///     The frequencies of all the notes on a piano keyboard have been defined in the Tones class.
        ///     You can use those constants for the frequency parameter if you want.
        /// </remarks>
        public void AddNote(float frequency, double length)
        {
            AddNote(frequency, length, _defaultVolume);
        }

        /// <summary>
        ///     Adds a note to the song.
        /// </summary>
        /// <param name="frequency">The frequency of the note to add.</param>
        /// <param name="length">The length, in beats, of the note.</param>
        /// <param name="volume">The volume of the note, ranging from 0 to short.MaxValue.</param>
        /// <remarks>
        ///     The frequencies of all the notes on a piano keyboard have been defined in the Tones class.
        ///     You can use those constants for the frequency parameter if you want.
        /// </remarks>
        public void AddNote(float frequency, double length, short volume, SignalType signalType = SignalType.Sine)
        {
            if (volume < 0)
                throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be greater than or equal to zero.");

            double samplesPerCycle = _writer.SampleRate / frequency;

            var samplesForNote = (int) (_writer.SampleRate * length * 60 / Tempo);


            var sample = new Sample16Bit();

            for (var currentSample = 0; currentSample < samplesForNote; currentSample++)
            {
                var endOfStream = true;

                if (_writer.CurrentSample < _writer.NumberOfSamples)
                {
                    sample = _writer.Read();
                    endOfStream = false;
                }
                else
                {
                    sample.LeftChannel = 0;
                }

                // If we're at the end of the note, fade out linearly.
                // This causes two back-to-back notes with the same frequency
                // to have a break between them, rather than being one
                // continuous tone.
                var distanceFromEnd = samplesForNote - currentSample;
                var finalVolume = distanceFromEnd < 1000 ? (short) (volume * distanceFromEnd / 1000) : volume;
                
                var t = currentSample / samplesPerCycle;
                switch (signalType)
                {
                    case SignalType.Sine:
                        t = SignalGenerators.Sine((float)t);
                        break;
                    case SignalType.Square:
                        t = SignalGenerators.Square((float)t);
                        break;
                    case SignalType.Sawtooth:
                        t = SignalGenerators.Sawtooth((float)t);
                        break;
                    case SignalType.Triangle:
                        t = SignalGenerators.Triangle((float)t);
                        break;
                    default:
                        t = SignalGenerators.Noise((float)t);
                        break;
                }
                var sampleValue = t * finalVolume + sample.LeftChannel;

                if (sampleValue > short.MaxValue)
                    sampleValue = short.MaxValue;
                else if (sampleValue < short.MinValue)
                    sampleValue = short.MinValue;

                sample.LeftChannel = (short) sampleValue;

                if (endOfStream == false)
                    _writer.CurrentSample--;

                _writer.Write(sample);
            }
        }

        /// <summary>
        ///     Adds a rest in the song, essentially advancing the CurrentBeat property
        ///     without adding any sound.
        /// </summary>
        /// <param name="length">The length, in beats, of the rest.</param>
        public void AddRest(double length)
        {
            CurrentBeat += length;
        }

        /// <summary>
        ///     Closes the SongWriter and saves the underlying stream.
        /// </summary>
        public void Close()
        {
            _writer?.Close();
        }

        # endregion

        #region IDisposable Members

        /// <summary>
        ///     Closes the SongWriter and the underlying stream.
        /// </summary>
        /// <param name="disposing">true if called by the Dispose() method; false if called by a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Close();
        }

        /// <summary>
        ///     Disposes this object and cleans up any resources used.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}