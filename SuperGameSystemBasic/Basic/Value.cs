using System;
using System.Globalization;
using System.Linq;

namespace SuperGameSystemBasic.Basic
{
    public enum ValueType

    {
        Real,
        String,
        Array
    }


    public struct Value
    {
        public static Value Zero => new Value(0);
        public static Value One => new Value(1);
        public ValueType Type;
        public double Real;
        public string String;
        public double[] Array;

        public Modifiers[] Modifier;

        public Value(double real) : this()
        {
            Type = ValueType.Real;
            Real = real;
        }

        public Value(double[] array) : this()
        {
            Type = ValueType.Array;
            Array = array;
        }

        public Value(string str)
            : this()
        {
            Type = ValueType.String;
            String = str;
        }

        public Value Convert(ValueType type)
        {
            switch (type)
            {
                case ValueType.Real:
                    switch (Type)
                    {
                        case ValueType.Real:
                            return new Value(Real);
                        case ValueType.String:
                            return double.TryParse(String, out var d) ? new Value(d) : new Value(double.NaN);
                        case ValueType.Array:
                            return new Value(Array[0]);
                        default: throw new TypeAccessException();
                    }

                case ValueType.String:
                    switch (Type)
                    {
                        case ValueType.Real:
                            return new Value(Real.ToString(CultureInfo.InvariantCulture));
                        case ValueType.String:
                            return new Value(String);
                        case ValueType.Array:
                            var str = Array.Aggregate(string.Empty, (current, t) => current + (char) ((int) t + 32));
                            return new Value(str);
                        default: throw new TypeAccessException();
                    }
                case ValueType.Array:
                {
                    switch (Type)
                    {
                        case ValueType.Real:
                            return new Value(new[] {Real});
                        case ValueType.String:
                            var sArray = String.ToCharArray();
                            var nArray = new double[sArray.Length];
                            for (var i = 0; i < sArray.Length; i++) nArray[i] = sArray[i] - 32;
                            return new Value(nArray);
                        case ValueType.Array:
                            var cArray = new double[Array.Length];
                            System.Array.Copy(Array, cArray, Array.Length);
                            return new Value(cArray);
                        default:
                            throw new TypeAccessException();
                    }
                }
                default: throw new TypeAccessException();
            }
        }


        public Value BinOp(Value b, Token tok)
        {
            var a = this;
            if (a.Type != b.Type)
                if (a.Type > b.Type)
                    b = b.Convert(a.Type);
                else
                    a = a.Convert(b.Type);
            switch (tok)
            {
                case Token.Plus:
                    if (a.Type == ValueType.Real)
                        return new Value(a.Real + b.Real) {Modifier = a.Modifier};
                    return new Value(a.String + b.String) {Modifier = a.Modifier};
                case Token.Equal:
                    if (a.Type == ValueType.Real)
                        return new Value(Math.Abs(a.Real - b.Real) < double.Epsilon ? 1 : 0) {Modifier = a.Modifier};
                    return new Value(a.String == b.String ? 1 : 0) {Modifier = a.Modifier};
                case Token.And:
                    if (a.Type == ValueType.Real)
                        return
                            new Value(
                                Math.Abs(a.Real - 1) < double.Epsilon && Math.Abs(b.Real - 1) < double.Epsilon ? 1 : 0)
                            {
                                Modifier = a.Modifier
                            };
                    throw new Exception("Cannot do binop on strings(except +).");
                case Token.Or:
                    if (a.Type == ValueType.Real)
                        return
                            new Value(
                                Math.Abs(a.Real - 1) < double.Epsilon || Math.Abs(b.Real - 1) < double.Epsilon ? 1 : 0)
                            {
                                Modifier = a.Modifier
                            };
                    throw new Exception("Cannot do binop on strings(except +).");
                case Token.NotEqual:
                    if (a.Type == ValueType.Real)
                        return new Value(Math.Abs(a.Real - b.Real) < double.Epsilon ? 0 : 1) {Modifier = a.Modifier};
                    return new Value(a.String == b.String ? 0 : 1) {Modifier = a.Modifier};
                default:
                    if (a.Type == ValueType.String)
                        throw new Exception("Cannot do binop on strings(except +).");
                    break;
            }
            switch (tok)
            {
                case Token.Minus:
                    return new Value(a.Real - b.Real) {Modifier = a.Modifier};
                case Token.Mod:
                    return new Value(a.Real % b.Real) {Modifier = a.Modifier};
                case Token.Asterisk:
                    return new Value(a.Real * b.Real) {Modifier = a.Modifier};
                case Token.Slash:
                    return new Value(a.Real / b.Real) {Modifier = a.Modifier};
                case Token.Caret:
                    return new Value(Math.Pow(a.Real, b.Real)) {Modifier = a.Modifier};
                case Token.Less:
                    return new Value(a.Real < b.Real ? 1 : 0) {Modifier = a.Modifier};
                case Token.More:
                    return new Value(a.Real > b.Real ? 1 : 0) {Modifier = a.Modifier};
                case Token.LessEqual:
                    return new Value(a.Real <= b.Real ? 1 : 0) {Modifier = a.Modifier};
                case Token.MoreEqual:
                    return new Value(a.Real >= b.Real ? 1 : 0) {Modifier = a.Modifier};
                default: throw new Exception("Unknown binop");
            }
        }

        public override string ToString()
        {
            return Convert(ValueType.String).String;
        }
    }
}