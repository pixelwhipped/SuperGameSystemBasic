namespace SuperGameSystemBasic.UI.Window
{
    public interface IWindow : IDrawingContext
    {
        bool IsDialog { get; }
        int PostionX { get; }
        int PostionY { get; }
        int PositionXOnScreen { get; }
        int PositionYOnScreen { get; }
        int Width { get; }
        int Height { get; }
        void Draw();

        void Select();
        void Unselect();
    }
}