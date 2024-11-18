using Raylib_cs;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Program;

public class Animator
{
    public Texture2D texture { get; private set; }
    public int timerFrame;
    public int currentFrame;
    public int speedFrame { get; private set; }
    public int widthFrame { get; private set; }
    public Rectangle rectFrame;

	public Animator(Texture2D _texPlayer, int _speedFrame, int _widthFrame, Rectangle _rectFrame)
    {
        texture = _texPlayer;
        timerFrame = 0;
        currentFrame = 0;
        speedFrame = _speedFrame;
        widthFrame = _widthFrame;
        rectFrame = _rectFrame;
    }

    public void Update()
    {
		timerFrame++;
		if (timerFrame >= speedFrame)
		{
			timerFrame = 0;
			currentFrame++;
			if (currentFrame > 7) currentFrame = 0;
		}
		rectFrame.X = currentFrame * widthFrame;
	}
}
