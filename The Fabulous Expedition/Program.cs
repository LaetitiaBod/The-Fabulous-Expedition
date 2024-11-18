using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.ConfigFlags;
using System.Numerics;
using Font = Raylib_cs.Font;

public static class Program
{
    public static int Main()
    {
        const int WindowWidth = 1920;
        const int WindowHeight = 1080;

        InitWindow(WindowWidth, WindowHeight, "The Fabulous Expedition");
#if DEBUG
        SetWindowState(ResizableWindow);
#endif
        SetConfigFlags(VSyncHint);
        SetExitKey(KeyboardKey.Null);
        InitAudioDevice();

        GameManager gameManager = new GameManager();
        ServiceLocator.AddService(gameManager);

        RenderTexture2D target = LoadRenderTexture(WindowWidth, WindowHeight);
        SetTextureFilter(target.Texture, TextureFilter.Bilinear);
        SetTargetFPS(60);

        gameManager.SetVirtualGameResolution(WindowWidth, WindowHeight);
        gameManager.exitWindow = false;
        
        Hud hud = new Hud();
        ServiceLocator.AddService(hud);

        gameManager.ChangeScene("logo");
		
		while (!gameManager.exitWindow)
        {
            //--- UPDATE ------------------------------------------------------------------------------
            if(WindowShouldClose()) gameManager.exitWindow = true;

            float dt = GetFrameTime();

            gameManager.scale = Math.Min((float)GetScreenWidth() / WindowWidth, (float)GetScreenHeight() / WindowHeight);

			gameManager.UpdateScene(dt);
#if DEBUG
            gameManager.debugManager.Update();
            if (IsWindowState(UnfocusedWindow)) Console.WriteLine("Pause");
#endif

            //-----------------------------------------------------------------------------------------

            //--- DRAW --------------------------------------------------------------------------------
            #region draw in native resolution
            BeginDrawing();
            ClearBackground(Color.Black);

			#region draw with the scale
			BeginTextureMode(target);
			ClearBackground(Color.White);

			if (gameManager.currentScene?.name == "gameplay")
            {
				#region draw with the camera
				BeginMode2D(gameManager.camera);

				gameManager.DrawScene();

				EndMode2D();

				#endregion
			}
			else
            {
				gameManager.DrawScene();
			}
			if (gameManager.currentScene?.name == "gameplay")
				hud.DrawHud();

            EndTextureMode();
            #endregion

            DrawTexturePro(
                target.Texture,
                new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height),
                new Rectangle(0, 0, WindowWidth * gameManager.scale, WindowHeight * gameManager.scale),
                new Vector2(0,0), 0, Color.White
            );
#if DEBUG
			gameManager.debugManager.Draw();
#endif
            EndDrawing();
			#endregion
			//-----------------------------------------------------------------------------------------
		}

		// --- DE-INITIALIZATION ---------------------------------------------------------------
		DeInitialize();
        //--------------------------------------------------------------------------------------

        return 0;
    }

    public static void DeInitialize()
    {
        ServiceLocator.GetService<GameManager>().Close();

        //UnloadRenderTexture(target);
        CloseAudioDevice();
        CloseWindow();
    }
}