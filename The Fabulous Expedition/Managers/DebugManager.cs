using Raylib_cs;
using static Raylib_cs.Raylib;

public class DebugManager : OptionsFile
{
    private Rectangle debugFrame;
    private bool showDebug = false;

    public DebugManager() : base()
    {
        debugFrame = new Rectangle(0, 0, 300, GetScreenHeight());
    }

    public void ToggleDebug() => showDebug = !showDebug;

    public void Update()
    {
        debugFrame.Height = GetScreenHeight();
        if (IsKeyPressed(KeyboardKey.D))
        {
            ToggleDebug();
        }
    }

    public void Draw()
    {
        if (showDebug)
        {
            DrawRectangleRec(debugFrame, new Color(0, 0, 0, 150));

            int y = 20;
            foreach (var option in options)
            {
                DrawText(option.Key + " : " + option.Value, 20, y, 10, Color.White);
                y += 12;
            }
        }
    }
}