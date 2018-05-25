using System;
using System.IO;

namespace SuperGameSystemBasic.Audio
{
    /// <summary>
    ///     Represents the Header information in a PCM wave file.
    /// </summary>
    internal class WaveHeader
    {
        # region Constants

        /// <summary>
        ///     The size of the wave header.
        /// </summary>
        public const int HeaderSize = 44;

        # endregion

        # region Private Fields

        private long _numberOfSamples;

        # endregion

        # region Constructors

        /// <summary>
        ///     Instantiates a new wave header.
        /// </summary>
        /// <param name="sampleRate">Specifies the sample rate (number of samples per second) of the wave file.</param>
        /// <param name="bitsPerSample">Specifies whether this is an 8-bit or 16-bit wave file.</param>
        /// <param name="stereo">Specifies whether this is a mono or stereo wave file.</param>
        public WaveHeader(int sampleRate, short bitsPerSample, bool stereo)
        {
            if (sampleRate <= 0)
                throw new ArgumentOutOfRangeException(nameof(sampleRate), "Sample Rate must be greater than zero.");

            if (bitsPerSample != 8 && bitsPerSample != 16)
                throw new ArgumentException(
                    "Invalid Bits Per Sample specified.  Currently supported values are [8, 16].",
                    nameof(bitsPerSample));

            SampleRate = sampleRate;
            BitsPerSample = bitsPerSample;
            Stereo = stereo;

            BytesPerSample = BitsPerSample / 8;
            if (Stereo)
                BytesPerSample *= 2;
        }

        # endregion

        # region Methods

        /// <summary>
        ///     Writes the wave header to a BinaryWriter object.
        /// </summary>
        /// <param name="writer">A BinaryWriter object to write the header to.</param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(0x46464952); // "RIFF" in ASCII
            writer.Write((int) (HeaderSize + _numberOfSamples * BitsPerSample * (Stereo ? 2 : 1) / 8) - 8);
            writer.Write(0x45564157); // "WAVE" in ASCII

            writer.Write(0x20746d66); // "fmt " in ASCII
            writer.Write(16);
            writer.Write((short) 1);
            writer.Write((short) (Stereo ? 2 : 1));
            writer.Write(SampleRate);
            writer.Write(SampleRate * (Stereo ? 2 : 1) * BitsPerSample / 8);
            writer.Write((short) ((Stereo ? 2 : 1) * BitsPerSample / 8));
            writer.Write(BitsPerSample);

            writer.Write(0x61746164); // "data" in ASCII
            writer.Write((int) (_numberOfSamples * BitsPerSample * (Stereo ? 2 : 1) / 8));
        }

        # endregion

        # region Properties

        /// <summary>
        ///     Gets the sample rate (number of samples per second) of the wave file.
        /// </summary>
        public int SampleRate { get; }

        /// <summary>
        ///     Gets the size in bits for a single channel of a sample.
        ///     Supported values are 8 and 16.
        /// </summary>
        public short BitsPerSample { get; }

        /// <summary>
        ///     Gets a boolean value which is true if the wave file is stereo, or false if it's mono.
        /// </summary>
        public bool Stereo { get; }

        /// <summary>
        ///     Gets the number of bytes needed to store all channels of a single sample.
        /// </summary>
        public int BytesPerSample { get; }

        /// <summary>
        ///     Gets or sets the number of samples that the wave file contains.
        /// </summary>
        public long NumberOfSamples
        {
            get => _numberOfSamples;
            set
            {
                if (value < 0 || value * BytesPerSample > uint.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "NumberOfSamples cannot be less than zero or greater than int.MaxValue * bytesPerSample.");

                _numberOfSamples = value;
            }
        }

        # endregion
    }
}