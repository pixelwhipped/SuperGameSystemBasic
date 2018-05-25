using System;

namespace SuperGameSystemBasic.Audio
{
    /// <summary>
    ///     Represents the data in a single sample (mono or stereo) in a 16-bit sound file.
    /// </summary>
    public struct Sample16Bit : IEquatable<Sample16Bit>
    {
        # region Private Fields

        private short _leftChannel;
        private short _rightChannel;

        # endregion

        # region Constructors

        /// <summary>
        ///     Creates a new single-channel 16-bit sample.
        /// </summary>
        /// <param name="monoChannel">The 16-bit value for the single channel in this sample.</param>
        public Sample16Bit(short monoChannel)
        {
            _leftChannel = monoChannel;
            _rightChannel = monoChannel;
            Stereo = false;
        }

        /// <summary>
        ///     Creates a new two-channel 16-bit sample.
        /// </summary>
        /// <param name="leftChannel">The 16-bit value for the left channel in this sample.</param>
        /// <param name="rightChannel">The 16-bit value for the right channel in this sample.</param>
        public Sample16Bit(short leftChannel, short rightChannel)
        {
            _leftChannel = leftChannel;
            _rightChannel = rightChannel;
            Stereo = true;
        }

        /// <summary>
        ///     Creates a new 16-bit sample with its value defaulted to 0.
        /// </summary>
        /// <param name="stereo">true for stereo; false for mono.</param>
        public Sample16Bit(bool stereo)
        {
            _leftChannel = 0;
            _rightChannel = 0;
            Stereo = stereo;
        }

        # endregion

        # region Properties

        /// <summary>
        ///     Gets or sets the left channel (for a stereo sample) and the single channel (for a mono sample).
        /// </summary>
        public short LeftChannel
        {
            get => _leftChannel;
            set
            {
                _leftChannel = value;

                if (!Stereo)
                    _rightChannel = value;
            }
        }

        /// <summary>
        ///     Gets or sets the right channel (for a stereo sample) and the single channel (for a mono sample).
        /// </summary>
        public short RightChannel
        {
            get => _rightChannel;
            set
            {
                _rightChannel = value;

                if (!Stereo)
                    _leftChannel = value;
            }
        }

        /// <summary>
        ///     Returns true if the sample is a stereo sample, or false if it's a mono sample.
        /// </summary>
        public bool Stereo { get; }

        # endregion

        # region Overridden Methods

        /// <summary>
        ///     Checks to see whether a specified object is equal to this object.
        /// </summary>
        /// <param name="obj">An object to compare to the this object.</param>
        /// <returns>True if the objects are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is Sample16Bit && Equals((Sample16Bit) obj);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>a 32-bit hash code.</returns>
        public override int GetHashCode()
        {
            int a = _leftChannel;
            int b = _rightChannel;

            return (a << 16) | b;
        }

        # endregion

        #region IEquatable<Sample16Bit> Members

        /// <summary>
        ///     Checks to see whether a specified Sample16Bit object is equal to this object.
        /// </summary>
        /// <param name="other">A Sample16Bit object to compare to the this object.</param>
        /// <returns>True if the objects are equal; false otherwise.</returns>
        public bool Equals(Sample16Bit other)
        {
            return _leftChannel == other._leftChannel && _rightChannel == other._rightChannel && Stereo == other.Stereo;
        }

        #endregion

        # region Operators

        /// <summary>
        ///     Checks to see whether two Sample16Bit instances are equal.
        /// </summary>
        /// <param name="a">a Sample16Bit instance</param>
        /// <param name="b">a Sample16Bit instance</param>
        /// <returns>true if the instances are equal; false otherwise.</returns>
        public static bool operator ==(Sample16Bit a, Sample16Bit b)
        {
            return a.Equals(b);
        }

        /// <summary>
        ///     Checks to see whether two Sample16Bit instances are unequal.
        /// </summary>
        /// <param name="a">a Sample16Bit instance</param>
        /// <param name="b">a Sample16Bit instance</param>
        /// <returns>true if the instances are unequal; false otherwise.</returns>
        public static bool operator !=(Sample16Bit a, Sample16Bit b)
        {
            return !a.Equals(b);
        }

        # endregion
    }
}