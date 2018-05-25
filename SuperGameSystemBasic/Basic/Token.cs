namespace SuperGameSystemBasic.Basic
{
    public enum Token

    {
        Unknown,
        Identifer,
        Value,

        //Keywords

        Horizontal, //1
        Vertical, //2
        None, //0       


        Blit,
        Print,
        If,
        EndIf,
        Then,
        Else,
        ElseIf,
        For,
        To,
        Step,
        Next,
        Input,
        Let,
        Var,
        Dim,
        While,
        Wend,
        Gosub,
        Return,
        Rem,
        End,
        NewLine,
        Colon,
        Semicolon,
        Comma,
        Plus,
        Minus,
        Mod,
        Slash,
        Asterisk,
        Caret,
        Equal,
        Less,
        More,
        NotEqual,
        LessEqual,
        MoreEqual,
        Or,
        And,
        Not,
        LParen,
        RParen,
        LBrace,
        RBrace,
        LBracket,
        RBracket,
        Echo,
        ReadTo,
        ReadLineTo,
        Obj,
        Array,
        Eval,
        Function,
        EndFunction,
        // ReSharper disable once InconsistentNaming
        EOF = -1 //End Of File
    }
}