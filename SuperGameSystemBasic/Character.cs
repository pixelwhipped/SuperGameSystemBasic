using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperGameSystemBasic
{
    public struct Character : IEquatable<Character>
    {
        /// <summary>
        ///     Gets the <see cref="Glyph" /> represented by the given ASCII character.
        /// </summary>
        /// <param name="ascii"></param>
        /// <returns></returns>
        public static Glyph ToGlyph(char ascii)
        {
            return (Glyph) MathHelper.Clamp(ascii - 32, 0, 191);
        }

        public static Glyph ParseGlyph(string text)
        {
            if (text.Length == 1)
                return ToGlyph(text[0]);
            // multiple characters are the glyph enum names
            return (Glyph) Enum.Parse(typeof(Glyph), text, true);
        }

        /// <summary>
        ///     Gets the <see cref="Glyph" /> used to draw this Character.
        /// </summary>
        public Glyph Glyph { get; }

        /// <summary>
        ///     Gets the foreground <see cref="Color" /> of this Character.
        /// </summary>
        public int ForeColor { get; }

        /// <summary>
        ///     Gets the background <see cref="Color" /> of this Character.
        /// </summary>
        public int BackColor { get; }

        public SpriteEffects Effect { get; }

        /// <summary>
        ///     Returns true if the <see cref="Glyph" /> for this Character is a non-visible
        ///     whitespace Glyph.
        /// </summary>
        public bool IsWhitespace => Glyph == Glyph.Space;


        /// <summary>
        ///     Initializes a new Character.
        /// </summary>
        /// <param name="glyph">Glyph used to draw the Character.</param>
        /// <param name="foreColor">Foreground <see cref="Color" /> of the Character.</param>
        /// <param name="backColor">Background <see cref="Color" /> of the Character.</param>
        /// <param name="effect">The effect</param>
        public Character(Glyph glyph, int foreColor, int backColor, SpriteEffects effect)
        {
            Glyph = glyph;
            Effect = effect;
            BackColor = MathHelper.Clamp(backColor, 0, 15);
            ForeColor = MathHelper.Clamp(foreColor, 0, 15);
        }

        /// <summary>
        ///     Initializes a new Character.
        /// </summary>
        /// <param name="ascii">
        ///     ASCII representation of the <see cref="Glyph" /> used
        ///     to draw the Character.
        /// </param>
        /// <param name="foreColor">Foreground <see cref="Color" /> of the Character.</param>
        /// <param name="backColor">Background <see cref="Color" /> of the Character.</param>
        /// <param name="effect">The effect</param>
        public Character(char ascii, int foreColor, int backColor, SpriteEffects effect)
            : this(ToGlyph(ascii), foreColor, backColor, effect)
        {
        }

        /// <summary>
        ///     Gets a string representation of this Character.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Glyph.ToString();
        }

        /// <summary>
        ///     Determines whether the specified object equals this <see cref="Character" />.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns><c>true</c> if <c>obj</c> is a Character equivalent to this Character; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Character character) return Equals(character);
            return base.Equals(obj);
        }

        /// <summary>
        ///     Returns a hash code for this <see cref="Character" />.
        /// </summary>
        /// <returns>An integer value that specifies the hash code for this Character.</returns>
        public override int GetHashCode()
        {
            return Glyph.GetHashCode();
        }

        #region IEquatable<Character> Members

        /// <summary>
        ///     Determines whether the specified <see cref="Character" /> equals this one.
        /// </summary>
        /// <param name="other">The <see cref="Character" /> to test.</param>
        /// <returns><c>true</c> if <c>other</c> is equivalent to this Character; otherwise, <c>false</c>.</returns>
        public bool Equals(Character other)
        {
            return Glyph == other.Glyph;
        }

        #endregion
    }
}