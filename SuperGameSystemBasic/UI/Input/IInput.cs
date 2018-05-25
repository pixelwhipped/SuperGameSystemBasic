using SuperGameSystemBasic.UI.Window;

namespace SuperGameSystemBasic.UI.Input
{
    public interface IInput : IDrawingContext
    {
        Window.Window ParentWindow { get; }
        string Id { get; }
        bool Selectable { get; set; }
        int PositionX { get; }
        int PositionY { get; }
        int PositionXOnScreen { get; }
        int PositionYOnScreen { get; }
        int Width { get; }
        int Height { get; }
        void Draw();
        void AddLetter(char letter);
        void BackSpace();
        void CursorMoveLeft();
        void CursorMoveUp();
        void CursorMoveDown();
        void CursorMoveRight();
        void CursorToStart();
        void CursorToEnd();

        void Delete();
        void Enter();
        void Escape();
        void PageUp();
        void PageDown();
        void Tab();
        void Select();
        void Unselect();
        void Click(int mx, int my);
    }
}