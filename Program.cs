using System;
using Raylib_cs;

namespace HelloWorld {
  static class Program {
    private static bool IsDebug => false;
    private static bool MouseLiveMode { get; set; }
    private static bool Paused { get; set; }
    private static bool NightMode { get; set; }
    public static void Main () {
      int screenWidth = 800;
      int screenHeight = 480;
      int cellWidth = 8;
      int mouseX = 0;
      int mouseY = 0;
      var grid = new bool[screenHeight / cellWidth, screenWidth / cellWidth];
      int frameCounter = 0;
      bool mouseIsDragging = false;
      NightMode = true;

      grid[0, 10] = true;
      Raylib.InitWindow (screenWidth, screenHeight, "Game of life");
      Raylib.SetWindowMonitor (1);
      while (!Raylib.WindowShouldClose ()) {
        frameCounter += 1;
        Raylib.SetConfigFlags (ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_WINDOW_UNFOCUSED);
        mouseIsDragging = Raylib.IsMouseButtonDown(0);
        if (Raylib.IsWindowResized ()) {
          screenWidth = Raylib.GetScreenWidth ();
          screenHeight = Raylib.GetScreenHeight ();
          grid = new bool[screenHeight / cellWidth, screenWidth / cellWidth];
        }

        if (Raylib.IsMouseButtonPressed(0)) {
          MouseLiveMode = ClickCell (Raylib.GetMouseX (), Raylib.GetMouseY (), cellWidth, grid);
        }
        if (mouseIsDragging) {
          ClickCell (Raylib.GetMouseX (), Raylib.GetMouseY (), cellWidth, grid);
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) {
          Paused = !Paused;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER)) {
          grid = new bool[screenHeight / cellWidth, screenWidth / cellWidth];
        }

        Raylib.BeginDrawing ();
        if (IsDebug) {
          mouseX = Raylib.GetMouseX ();
          mouseY = Raylib.GetMouseY ();
          Raylib.DrawText ($"x:{mouseX},y:{mouseY}", Convert.ToInt32 (screenWidth * 0.9), 20, 20, Color.BLACK);
          int rows = grid.GetLength (0);
          int columns = grid.GetLength (1);

          int x = mouseX / cellWidth;
          int y = mouseY / cellWidth;
          if (x < columns && y < rows) {
            int liveNeighbours = GetAliveNeighbours(x, y, grid);
            Raylib.DrawText ($"Neighbours:{liveNeighbours}", Convert.ToInt32 (screenWidth * 0.9), 42, 20, Color.BLACK);
          }

        }
        Raylib.ClearBackground (NightMode ? Color.BLACK : Color.WHITE);
        DrawGrid (cellWidth, grid);
        if (((frameCounter / 12) % 2) == 1) {
          frameCounter = 0;
          if (!Paused){
            grid = DrawGrid(cellWidth, grid);
          }
        }
        Raylib.EndDrawing ();
      }

      Raylib.CloseWindow ();
    }
    private static bool ClickCell (int mouseX, int mouseY, int cellWidth, bool[, ] grid) {
      int rows = grid.GetLength (0);
      int columns = grid.GetLength (1);

      int x = mouseX / cellWidth;
      int y = mouseY / cellWidth;
      if (x < columns && y < rows) {
        grid[y, x] = true;
      }
      return grid[y, x];
    }
    private static int GetAliveNeighbours (int x, int y, bool[, ] grid) {
      int rows = grid.GetLength (0);
      int columns = grid.GetLength (1);

      int aliveNeighbours = 0;
      // left column
      if (x > 0) {
        // top left
        if (y > 0) {
          if (grid[y - 1, x - 1]) {
            aliveNeighbours += 1;
          }
        }
        // left
        if (grid[y, x - 1]) {
          aliveNeighbours += 1;
        }
        // bottom left
        if (y < rows - 1) {
          if (grid[y + 1, x - 1]) {
            aliveNeighbours += 1;
          }
        }
      }
      // top
      if (y > 0) {
        if (grid[y - 1, x]) {
          aliveNeighbours += 1;
        }
      }
      // bottom
      if (y < rows - 1) {
        if (grid[y + 1, x]) {
          aliveNeighbours += 1;
        }
      }
      // right column
      if (x < columns - 1) {
        // top right
        if (y > 0) {
          if (grid[y - 1, x + 1]) {
            aliveNeighbours += 1;
          }
        }
        // right
        if (grid[y, x + 1]) {
          aliveNeighbours += 1;
        }
        // bottom right
        if (y < rows - 1) {
          if (grid[y + 1, x + 1]) {
            aliveNeighbours += 1;
          }
        }
      }
      return aliveNeighbours;
    }
    private static bool[, ] DrawGrid (int cellWidth, bool[, ] grid) {
      int rows = grid.GetLength (0);
      int columns = grid.GetLength (1);

      var newGrid = new bool[rows, columns];
      for (int y = 0; y < rows; y++) {
        for (int x = 0; x < columns; x++) {
          int liveNeighbours = GetAliveNeighbours (x, y, grid);
          bool isAlive = grid[y, x];
          bool itWillLive = false;
          if (isAlive) {
            if (liveNeighbours == 2 || liveNeighbours == 3) {
              itWillLive = true;
            }
          } else {
            if (liveNeighbours == 3) {
              itWillLive = true;
            }
          }
          int cellX = cellWidth * x;
          int cellY = cellWidth * y;
          if (isAlive) {
            Raylib.DrawRectangle (cellX, cellY, cellWidth, cellWidth, NightMode ? Color.WHITE : Color.BLACK);
          } else {
            if (IsDebug) {
              Raylib.DrawRectangleLines (cellX, cellY, cellWidth, cellWidth, Color.BLACK);
            }
          }
          newGrid[y, x] = itWillLive;
        }
      }
      return newGrid;
    }
  }
}