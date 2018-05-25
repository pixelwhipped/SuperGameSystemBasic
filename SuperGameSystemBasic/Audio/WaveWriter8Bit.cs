using System.IO;

namespace SuperGameSystemBasic.Audio
{
    /// <summary>
    ///     A class to write 8-bit wave files.
    /// </summary>
    public class WaveWriter8Bit : WaveWriter
    {
        # region Constructors

        /// <summary>
        ///     Instatiates a new WaveWriter8Bit object.
        /// </summary>
        /// <param name="output">A Stream object to write the wave to.</param>
        /// <param name="sampleRate">Specifies the sample rate (number of samples per second) of the wave file.</param>
        /// <param name="stereo">Specifies whether this is a mono or stereo wave file.</param>
        /// <remarks>
        ///     When this object is disposed, it will close its underlying stream.  To change this default behavior,
        ///     you can call an overloaded constructor which takes a closeUnderlyingStream parameter.
        /// </remarks>
        public WaveWriter8Bit(Stream output, int sampleRate, bool stereo)
            : this(output, sampleRate, stereo, true)
        {
        }

        /// <summary>
        ///     Instatiates a new WaveWriter8Bit object.
        /// </summary>
        /// <param name="output">A Stream object to write the wave to.</param>
        /// <param name="sampleRate">Specifies the sample rate (number of samples per second) of the wave file.</param>
        /// <param name="stereo">Specifies whether this is a mono or stereo wave file.</param>
        /// <param name="closeUnderlyingStream">
        ///     Determines whether to close the the stream that the WaveWriter is writing to when the WaveWriter is closed.
        /// </param>
        public WaveWriter8Bit(Stream output, int sampleRate, bool stereo, bool closeUnderlyingStream)
            : base(output, sampleRate, 8, stereo, closeUnderlyingStream)
        {
        }

        # endregion

        # region Methods

        /// <summary>
        ///     Reads a sample from the wave file.
        /// </summary>
        /// <returns>The data read from the wave file.</returns>
        public Sample8Bit Read()
        {
            ThrowIfDisposed();

            var initialPos = Reader.BaseStream.Position;

            try
            {
                if (Header.Stereo)
                    return new Sample8Bit(Reader.ReadByte(), Reader.ReadByte());
                return new Sample8Bit(Reader.ReadByte());
            }
            catch
            {
                Reader.BaseStream.Position = initialPos;

                throw;
            }
        }

        /// <summary>
        ///     Writes a sample to the wave file.
        /// </summary>
        /// <param name="sample">The sample information to write.</param>
        public void Write(Sample8Bit sample)
        {
            ThrowIfDisposed();

            var initialPos = Writer.BaseStream.Position;

            try
            {
                Writer.Write(sample.LeftChannel);

                if (Header.Stereo)
                    Writer.Write(sample.RightChannel);
            }
            catch
            {
                Writer.BaseStream.Position = initialPos;

                throw;
            }
            finally
            {
                if (CurrentSample > Header.NumberOfSamples)
                    Header.NumberOfSamples = CurrentSample;
            }
        }

        # endregion
    }
}