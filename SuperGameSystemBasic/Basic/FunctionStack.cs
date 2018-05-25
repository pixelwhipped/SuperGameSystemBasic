using System.Collections.Generic;

namespace SuperGameSystemBasic.Basic
{
    public class FunctionStack
    {
        public Marker Marker;
        public Stack<Marker> Stack = new Stack<Marker>();
        public readonly string Name;
        //public Interpreter Interpreter;
        //public Value Value => Interpreter.GetVar(Name);

        public FunctionStack(string var, Marker lineMarker)//, Interpreter interpreter)
        {
            Name = var;
            Marker = lineMarker;
            //Interpreter = interpreter;
            //Interpreter.SetVar(Name, Value.Zero);
        }
    }
}