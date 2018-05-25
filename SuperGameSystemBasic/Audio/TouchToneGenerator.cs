using System;
using System.Collections.Generic;
using System.IO;

namespace SuperGameSystemBasic.Audio
{
    /// <summary>
    ///     Generates tones from a touch-tone phone.
    /// </summary>
    public class TouchToneGenerator : IDisposable
    {
        # region Static Constructors

        static TouchToneGenerator()
        {
            ColumnTone['1'] = ColumnTones[0];
            ColumnTone['2'] = ColumnTones[1];
            ColumnTone['3'] = ColumnTones[2];
            ColumnTone['4'] = ColumnTones[0];
            ColumnTone['5'] = ColumnTones[1];
            ColumnTone['6'] = ColumnTones[2];
            ColumnTone['7'] = ColumnTones[0];
            ColumnTone['8'] = ColumnTones[1];
            ColumnTone['9'] = ColumnTones[2];
            ColumnTone['*'] = ColumnTones[0];
            ColumnTone['0'] = ColumnTones[1];
            ColumnTone['#'] = ColumnTones[2];

            RowTone['1'] = RowTones[0];
            RowTone['2'] = RowTones[0];
            RowTone['3'] = RowTones[0];
            RowTone['4'] = RowTones[1];
            RowTone['5'] = RowTones[1];
            RowTone['6'] = RowTones[1];
            RowTone['7'] = RowTones[2];
            RowTone['8'] = RowTones[2];
            RowTone['9'] = RowTones[2];
            RowTone['*'] = RowTones[3];
            RowTone['0'] = RowTones[3];
            RowTone['#'] = RowTones[3];

            LetterTranslations['1'] = '1';
            LetterTranslations['2'] = '2';
            LetterTranslations['3'] = '3';
            LetterTranslations['4'] = '4';
            LetterTranslations['5'] = '5';
            LetterTranslations['6'] = '6';
            LetterTranslations['7'] = '7';
            LetterTranslations['8'] = '8';
            LetterTranslations['9'] = '9';
            LetterTranslations['*'] = '*';
            LetterTranslations['0'] = '0';
            LetterTranslations['#'] = '#';

            LetterTranslations['A'] = '2';
            LetterTranslations['B'] = '2';
            LetterTranslations['C'] = '2';
            LetterTranslations['D'] = '3';
            LetterTranslations['E'] = '3';
            LetterTranslations['F'] = '3';
            LetterTranslations['G'] = '4';
            LetterTranslations['H'] = '4';
            LetterTranslations['I'] = '4';
            LetterTranslations['J'] = '5';
            LetterTranslations['K'] = '5';
            LetterTranslations['L'] = '5';
            LetterTranslations['M'] = '6';
            LetterTranslations['N'] = '6';
            LetterTranslations['O'] = '6';
            LetterTranslations['P'] = '7';
            LetterTranslations['R'] = '7';
            LetterTranslations['S'] = '7';
            LetterTranslations['T'] = '8';
            LetterTranslations['U'] = '8';
            LetterTranslations['V'] = '8';
            LetterTranslations['W'] = '9';
            LetterTranslations['X'] = '9';
            LetterTranslations['Y'] = '9';
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

        # region Static Fields

        private static readonly double[] ColumnTones = {1209, 1336, 1477};
        private static readonly double[] RowTones = {697, 770, 852, 941};
        private static readonly Dictionary<char, double> ColumnTone = new Dictionary<char, double>();
        private static readonly Dictionary<char, double> RowTone = new Dictionary<char, double>();
        private static readonly Dictionary<char, char> LetterTranslations = new Dictionary<char, char>();

        # endregion

        # region Private Fields

        private readonly WaveWriter8Bit _writer;

        private readonly int _samplesPerTone;

        private readonly int _samplesPerPause;

        # endregion

        # region Constructors

        /// <summary>
        ///     Instantiates a new TouchToneGenerator object.
        /// </summary>
        /// <param name="output">The Stream object to write the wave to.</param>
        /// <remarks>
        ///     When this object is disposed, it will close its underlying stream.  To change this default behavior,
        ///     you can call an overloaded constructor which takes a closeUnderlyingStream parameter.
        /// </remarks>
        public TouchToneGenerator(Stream output)
            : this(output, true)
        {
        }

        /// <summary>
        ///     Instantiates a new TouchToneGenerator object.
        /// </summary>
        /// <param name="output">The Stream object to write the wave to.</param>
        /// <param name="closeUnderlyingStream">
        ///     Determines whether to close the the stream that the TouchToneGenerator
        ///     is writing to when the TouchToneGenerator is closed.
        /// </param>
        public TouchToneGenerator(Stream output, bool closeUnderlyingStream)
        {
            _writer = new WaveWriter8Bit(output, 8000, false, closeUnderlyingStream);

            _samplesPerTone = _writer.SampleRate * 9 / 20;

            _samplesPerPause = _writer.SampleRate / 20;
        }

        # endregion

        # region Methods

        /// <summary>
        ///     Generates touchtone phone tones for a string of digits.
        /// </summary>
        /// <param name="phoneNumber">
        ///     A string of digits.
        ///     Valid digits are: 1234567890*#ABCDEFGHIJKLMNOPRSTUVWXY
        ///     Invalid digits will be silently ignored.
        /// </param>
        public void GenerateTones(string phoneNumber)
        {
            foreach (var digit in phoneNumber.ToUpper())
                if (LetterTranslations.ContainsKey(digit))
                    GenerateTone(LetterTranslations[digit]);
        }

        private void GenerateTone(char digit)
        {
            var columnFrequency = ColumnTone[digit];
            var rowFrequency = RowTone[digit];

            var samplesPerColumnCycle = _writer.SampleRate / columnFrequency;
            var samplesPerRowCycle = _writer.SampleRate / rowFrequency;


            var sample = new Sample8Bit(false);

            var startPoint = _writer.CurrentSample;

            // Write out the tone associated with the digit's column
            for (var currentSample = 0; currentSample < _samplesPerTone; currentSample++)
            {
                var sampleValue = Math.Sin(currentSample / samplesPerColumnCycle * 2 * Math.PI) * 60;

                sample.LeftChannel = (byte) (sampleValue + 128);

                _writer.Write(sample);
            }

            // go back to the starting point
            _writer.CurrentSample = startPoint;

            // Overlay the tone associated with the digit's row
            for (var currentSample = 0; currentSample < _samplesPerTone; currentSample++)
            {
                // Figure out what the sample was originally set to
                var originalSample = _writer.Read();
                _writer.CurrentSample--;

                var sampleValue = Math.Sin(currentSample / samplesPerRowCycle * 2 * Math.PI) * 60;

                sample.LeftChannel = (byte) (sampleValue + originalSample.LeftChannel);

                _writer.Write(sample);
            }

            // Insert a pause
            sample.LeftChannel = 128;

            for (var currentSample = 0; currentSample < _samplesPerPause; currentSample++)
                _writer.Write(sample);
        }

        /// <summary>
        ///     Closes the TouchToneGenerator and saves the underlying stream.
        /// </summary>
        public void Close()
        {
            _writer?.Close();
        }

        /// <summary>
        ///     Closes the TouchToneGenerator and the underlying stream.
        /// </summary>
        /// <param name="disposing">true if called by the Dispose() method; false if called by a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Close();
        }

        # endregion
    }
}