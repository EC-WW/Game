using Microsoft.VisualBasic;
using NITELITE;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace MineSweeper
{
  class MineSweeperManager : NL_Script
  {
    public int gridSizeX;
    public int gridSizeY;
    public Entity Cursor;
    public Entity consoleEntity;

    Console consoleScript;

    Vec2 cursorPos;
    List<List<Entity>> gridCube = new List<List<Entity>>();
    List<List<Entity>> gridLabel = new List<List<Entity>>();

    bool[,] bombLocations = new bool[10, 10];
    int[,] bombCounts = new int[10, 10];
    bool[,] openStatis = new bool[10, 10];
    bool[,] flagStatis = new bool[10, 10];

    bool bombsGenerated = false;

    public override void Init()
    {
      //Spawn grid of cubes
      for (int i = 0; i < gridSizeX; ++i)
      {
        List<Entity> cubeRow = new List<Entity>();
        List<Entity> labelRow = new List<Entity>();
        for (int j = 0; j < gridSizeY; ++j)
        {
          Entity newCube = Scene.LoadPrefab("assets/prefabs/Cube.nlprefab");
          newCube.name = "Cube[" + i + "," + j + "]";
          newCube.GetComponent<Transform>().SetPosition(new Vec3((-gridSizeX / 2.0f) * 1.3f + (i * 1.4444444444f), -6, 25 + (j * 1.3f)));
          newCube.GetComponent<ModelComponent>().tint = new Vec3(0.0f, 1.0f, 0.0f);
          cubeRow.Add(newCube);

          Entity newLabel = Scene.LoadPrefab("assets/prefabs/Label.nlprefab");
          newLabel.name = "Label[" + i + "," + j + "]";
          newLabel.GetComponent<Transform>().SetPosition(new Vec3((-gridSizeX / 2.0f) * 1.3f + (i * 1.4444444444f), -6 + 0.502f, 25 + (j * 1.3f)));
          newLabel.GetComponent<TextComponent>().Text = "";
          labelRow.Add(newLabel);
        }
        gridCube.Add(cubeRow);
        gridLabel.Add(labelRow);
      }

      consoleScript = consoleEntity.GetComponent<Console>();

      Vec3 startPos = gridCube[(int)cursorPos.x][(int)cursorPos.y].GetComponent<Transform>().GetPosition();
      startPos.y += 1.5f;
      Cursor.GetComponent<Transform>().SetPosition(startPos);
    }

    public override void Update()
    {
      if (!consoleScript.MinigameActive) return;

      if (NITELITE.Input.GetKeyTriggered(Keys.W))
      {
        cursorPos.y += 1;
        if (cursorPos.y >= gridSizeY) cursorPos.y = gridSizeY - 1;
      }

      if (NITELITE.Input.GetKeyTriggered(Keys.A))
      {
        cursorPos.x -= 1;
        if(cursorPos.x <= -1) cursorPos.x = 0;
      }

      if (NITELITE.Input.GetKeyTriggered(Keys.S))
      {
        cursorPos.y -= 1;
        if (cursorPos.y <= -1) cursorPos.y = 0;
      }

      if (NITELITE.Input.GetKeyTriggered(Keys.D))
      {
        cursorPos.x += 1;
        if (cursorPos.x >= gridSizeX) cursorPos.x = gridSizeX - 1;
      }

      if (NITELITE.Input.GetKeyTriggered(Keys.SPACE))
      {
        if(!bombsGenerated) generateBombs((int) cursorPos.x, (int)cursorPos.y);

        if(!OpenCell((int)cursorPos.x, (int)cursorPos.y)) bombsGenerated = true;
      }

      if (NITELITE.Input.GetKeyTriggered(Keys.LEFT_SHIFT) && !openStatis[(int)cursorPos.x, (int)cursorPos.y])
      { 
        flagStatis[(int)cursorPos.x, (int)cursorPos.y] = !flagStatis[(int)cursorPos.x, (int)cursorPos.y];

        if (flagStatis[(int)cursorPos.x, (int)cursorPos.y])
        {
          gridCube[(int)cursorPos.x][(int)cursorPos.y].GetComponent<ModelComponent>().tint = new Vec3(0.0f, 0.0f, 1.0f);
        }
        else
        {
          gridCube[(int)cursorPos.x][(int)cursorPos.y].GetComponent<ModelComponent>().tint = new Vec3(0.0f, 1.0f, 0.0f);
        }
      }

      Vec3 startPos = gridCube[(int)cursorPos.x][(int)cursorPos.y].GetComponent<Transform>().GetPosition();
      startPos.y += 1.5f;
      Cursor.GetComponent<Transform>().SetPosition(startPos);

      //Check text based on nearby bombs
      
    }

    public override void Exit()
    {
    }

    void generateBombs(int startX, int startY)
    {
      Random r = new Random();
      for (int i = 0; i < 35; ++i)
      {
        int x = r.Next(0, gridSizeX - 1);
        int y = r.Next(0, gridSizeY - 1);

        //Check if the bomb is in the same row or column as the starting point
        if (x == startX && y == startY)
        {
          i--;
          continue;
        }

        //Check if in 3x3 area around starting point
        if (x >= startX - 1 && x <= startX + 1 && y >= startY - 1 && y <= startY + 1)
        {
          i--;
          continue;
        }

        bombLocations[x, y] = true;

        //gridCube[x][y].GetComponent<ModelComponent>().tint = new Vec3(1.0f, 0.0f, 0.0f);
      }

      //Get counts
      for (int i = 0; i < gridSizeX; ++i)
      {
        for (int j = 0; j < gridSizeY; ++j)
        {
          int bombCount = 0;
          for (int k = -1; k <= 1; ++k)
          {
            for (int m = -1; m <= 1; ++m)
            {
              if (i + k < 0 || i + k >= gridSizeX || j + m < 0 || j + m >= gridSizeY) continue;
              if (bombLocations[(int)(i + k), (int)(j + m)])
              {
                bombCount++;
              }
            }
          }

          bombCounts[i, j] = bombCount;
        }
      }
    }

    bool OpenCell(int x, int y)
    {
      if (flagStatis[x,y]) return false;

      //Check if bomb
      if (bombLocations[x, y])
      {
        restart();
        return true;
      }

      gridCube[x][y].GetComponent<ModelComponent>().tint = new Vec3(1.0f, 1.0f, 1.0f);
      updateLabel(x, y);
      openStatis[x, y] = true;

      for (int k = -1; k <= 1; ++k)
      {
        for (int m = -1; m <= 1; ++m)
        {

          if (x + k < 0 || x + k >= gridSizeX || y + m < 0 || y + m >= gridSizeY) continue;
          //If you are zero or target cell 0  

          if (bombCounts[x, y] == 0 || bombCounts[x + k, y + m] == 0)
          {
            if (openStatis[x + k, y + m] == false)
            {
              OpenCell(x + k, y + m);
            }
          }
        }
      }

      return false;
    }

    void updateLabel(int x, int y)
    {
      gridLabel[x][y].GetComponent<TextComponent>().Text = bombCounts[x, y].ToString();
    }

    void restart()
    {
      for (int x = 0; x < gridSizeX; ++x)
      {
        for (int y = 0; y < gridSizeY; ++y)
        {
          gridLabel[x][y].GetComponent<TextComponent>().Text = "";
          gridCube[x][y].GetComponent<ModelComponent>().tint = new Vec3(0.0f, 1.0f, 0.0f);
          bombLocations[x, y] = false;
          bombCounts[x, y] = 0;
          openStatis[x, y] = false;
          flagStatis[x, y] = false;
        }
      }

      bombsGenerated = false;
    }
  }
}
