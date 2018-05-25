using System;
using System.IO;

namespace SuperGameSystemBasic.Audio
{
    /// <summary>
    ///     Abstract base class for writing wave files
    /// </summary>
    public abstract class WaveWriter : IDisposable
    {
        # region Constructors

        /// <summary>
        ///     Instantiates a WaveWriter object.
        /// </summary>
        /// <param name="output">A Stream object to write the wave to.</param>
        /// <param name="sampleRate">Specifies the sample rate (number of samples per second) of the wave file.</param>
        /// <param name="bitsPerSample">Specifies whether this is an 8-bit or 16-bit wave file.</param>
        /// <param name="stereo">Specifies whether this is a mono or stereo wave file.</param>
        /// <param name="closeUnderlyingStream">
        ///     Determines whether to close the the stream that the WaveWriter is writing to when the WaveWriter is closed.
        /// </param>
        protected WaveWriter(Stream output, int sampleRate, short bitsPerSample, bool stereo,
            bool closeUnderlyingStream)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output), "Output stream cannot be null.");

            _header = new WaveHeader(sampleRate, bitsPerSample, stereo);

            _reader = new BinaryReader(output);
            _writer = new BinaryWriter(output);
            _initialStreamPosition = output.Position;

            output.Position += WaveHeader.HeaderSize;

            _closeUnderlyingStream = closeUnderlyingStream;
        }

        # endregion

        #region IDisposable Members

        /// <summary>
        ///     Disposes this object and cleans up any resources used.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        # region Private Fields

        private readonly WaveHeader _header;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;
        private readonly long _initialStreamPosition;

        private readonly bool _closeUnderlyingStream;

        private bool _disposed;

        # endregion

        # region Properties

        /// <summary>
        ///     Gets the number of samples contained in the wave file.
        /// </summary>
        public long NumberOfSamples
        {
            get
            {
                ThrowIfDisposed();

                return _header.NumberOfSamples;
            }
        }

        /// <summary>
        ///     Gets the sample rate (number of samples per second) of the wave file.
        /// </summary>
        public int SampleRate
        {
            get
            {
                ThrowIfDisposed();

                return _header.SampleRate;
            }
        }

        /// <summary>
        ///     Gets the header information for the wave file.
        /// </summary>
        internal WaveHeader Header
        {
            get
            {
                ThrowIfDisposed();

                return _header;
            }
        }

        /// <summary>
        ///     Gets or sets the position within the wave file.
        /// </summary>
        public long CurrentSample
        {
            get
            {
                ThrowIfDisposed();

                return (_writer.BaseStream.Position - WaveHeader.HeaderSize - _initialStreamPosition) /
                       _header.BytesPerSample;
            }
            set
            {
                ThrowIfDisposed();

                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "Sample Number must be greater than or equal to zero.");

                _writer.BaseStream.Position = _initialStreamPosition + WaveHeader.HeaderSize +
                                              value * _header.BytesPerSample;
            }
        }

        /// <summary>
        ///     Gets a BinaryReader which allows direct read access to the underlying stream.
        /// </summary>
        protected BinaryReader Reader
        {
            get
            {
                ThrowIfDisposed();

                return _reader;
            }
        }

        /// <summary>
        ///     Gets a BinaryWriter which allows direct write access to the underlying stream.
        /// </summary>
        protected BinaryWriter Writer
        {
            get
            {
                ThrowIfDisposed();

                return _writer;
            }
        }

        # endregion

        # region Methods

        /// <summary>
        ///     Flushes all data and closes the WaveWriter.
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Writes the wave header information and closes the underlying stream.
        /// </summary>
        /// <param name="disposing">true if called by the Dispose() method; false if called by a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _writer.BaseStream.Position = _initialStreamPosition;

            Header.Write(_writer);

            if (disposing)
                if (_closeUnderlyingStream)
                {
                    _writer.Flush();
                    _writer.Dispose();
                    _reader.Dispose();
                }

            _disposed = true;
        }

        /// <summary>
        ///     Throws an ObjectDisposedException if the object has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("WaveWriter");
        }

        # endregion
    }
}