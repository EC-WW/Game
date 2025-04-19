/**
/ file:   ZombieManager.cs
/ author: aditya.prakash
/ date:   April 16, 2025
/ Copyright (c) 2025 DigiPen (USA) Corporation. 
/ 
/ brief:  Example script for the NITELITE engine, uses several of its features
**/
using System;
using System.Collections.Generic;
using NITELITE;

public struct ZombieWave
{
  public float spawnTime; // seconds from level start
  public int count;       // how many zombies to spawn
  public int row;         // which row to spawn in (0–4), or -1 for random
  public string type;     // zombie type
  public float speed;
  public int health;
  public bool bigWave;
}

public struct ZombieLevel
{
  public ZombieWave[] waves;
}

public class Zombie
{
  public Entity entity;
  public float x;
  public float y;
  public int health;
  public int originalHealth;
  public Vec3 originalScale;
  public int damage;
  public float speed;
  public string type;
  public float ax;
  public float ay;
  public float prevX;
  //public bool touchingPlant = false;
}
public enum GameStates
{
  MenuControls,
  Pause,
  Game,
  GameOver,
  Victory
}

public class GameManager : NL_Script
{
  public Entity MenuControlsEntity;
  public Entity PauseMenuEntity;
  public Entity GameOverEntity;
  public Entity VictoryMenuEntity;
  public Entity resumeTextEntity;
  public Entity exitTextEntity;

  private List<Zombie> zombies = new List<Zombie>();

  private ZombieLevel[] levels = new ZombieLevel[5];
  private int currentLevel = 0;
  private float levelTimer = 0f;
  private int nextWaveIndex = 0;
  private bool waitingForBigWave = false;

  private Entity ExplodeEntity;

  private GameStates gameState = GameStates.MenuControls;
  private float timeInMenu = 0f;
  private int pauseButton = 1;

  public override void Init()
  {
    levels[0] = new ZombieLevel
    {
      waves = new ZombieWave[]
      {
      new ZombieWave { spawnTime = 2f, count = 1, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 12f, count = 2, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 25f, count = 2, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 30f, count = 1, row = -1, type = "team_adi", speed = 0.7f, health = 80, bigWave = true },
      new ZombieWave { spawnTime = 31f, count = 4, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      }
    };
    levels[1] = new ZombieLevel
    {
      waves = new ZombieWave[]
      {
      new ZombieWave { spawnTime = 2f, count = 5, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 15f, count = 3, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 30f, count = 5, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 31f, count = 5, row = -1, type = "team_adi", speed = 0.7f, health = 80, bigWave = false },
      new ZombieWave { spawnTime = 40f, count = 4, row = -1, type = "charles_flexing", speed = 0.3f, health = 400, bigWave = true },
      }
    };
    levels[2] = new ZombieLevel
    {
      waves = new ZombieWave[]
      {
      new ZombieWave { spawnTime = 2f, count = 1, row = -1, type = "charles_flexing", speed = 0.3f, health = 400, bigWave = false },
      new ZombieWave { spawnTime = 10f, count = 3, row = -1, type = "team_evan", speed = 2f, health = 20, bigWave = false },
      new ZombieWave { spawnTime = 25f, count = 5, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 31f, count = 5, row = -1, type = "team_adi", speed = 0.7f, health = 80, bigWave = false },
      new ZombieWave { spawnTime = 40f, count = 4, row = -1, type = "charles_flexing", speed = 0.3f, health = 400, bigWave = true },
      new ZombieWave { spawnTime = 41f, count = 4, row = -1, type = "team_evan", speed = 2f, health = 20, bigWave = false },
      }
    };
    levels[3] = new ZombieLevel
    {
      waves = new ZombieWave[]
      {
      new ZombieWave { spawnTime = 2f, count = 3, row = -1, type = "team_evan", speed = 2f, health = 20, bigWave = false },
      new ZombieWave { spawnTime = 3f, count = 2, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 10f, count = 5, row = -1, type = "jack_samba", speed = 0.4f, health = 150, bigWave = false },
      new ZombieWave { spawnTime = 25f, count = 5, row = -1, type = "team_adi", speed = 0.7f, health = 80, bigWave = false },
      new ZombieWave { spawnTime = 38f, count = 3, row = -1, type = "charles_flexing", speed = 0.3f, health = 400, bigWave = false},
      new ZombieWave { spawnTime = 41f, count = 8, row = -1, type = "jack_samba", speed = 0.4f, health = 150, bigWave = true },
      }
    };
    levels[4] = new ZombieLevel
    {
      waves = new ZombieWave[]
      {
      new ZombieWave { spawnTime = 2f, count = 5, row = -1, type = "Taylor", speed = 1.2f, health = 5, bigWave = false },
      new ZombieWave { spawnTime = 3f, count = 10, row = -1, type = "Taylor", speed = 1.2f, health = 5, bigWave = false },
      new ZombieWave { spawnTime = 4f, count = 15, row = -1, type = "Taylor", speed = 1.2f, health = 5, bigWave = false },
      new ZombieWave { spawnTime = 25f, count = 5, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 26f, count = 5, row = -1, type = "jack_samba", speed = 0.4f, health = 150, bigWave = false },
      new ZombieWave { spawnTime = 27f, count = 5, row = -1, type = "team_adi", speed = 0.7f, health = 80, bigWave = false },
      new ZombieWave { spawnTime = 38f, count = 8, row = -1, type = "charles_flexing", speed = 0.3f, health = 400, bigWave = true},
      new ZombieWave { spawnTime = 41f, count = 8, row = -1, type = "jack_samba", speed = 0.4f, health = 150, bigWave = false },
      new ZombieWave { spawnTime = 43f, count = 5, row = -1, type = "team_evan", speed = 2f, health = 20, bigWave = false },
      }
    };


    ExplodeEntity = Entity.Create();
    ExplodeEntity.AddComponent<Transform>();
    ExplodeEntity.GetComponent<Transform>().SetScale(new Vec3(0f, 0f, 0f));

    ExplodeEntity.AddComponent<ModelComponent>();

    ExplodeEntity.AddComponent<ParticleEmitter>();
    ExplodeEntity.GetComponent<ParticleEmitter>().emissionRate = 0f;
    ExplodeEntity.GetComponent<ParticleEmitter>().looping = false;
  }
  private float Lerp(float a, float b, float t)
  {
    return a + (b - a) * t;
  }

  public override void Update()
  {
    switch (gameState)
    {
      case GameStates.MenuControls:
        MenuControlsUpdate();
        break;
      case GameStates.Pause:
        PauseMenuUpdate();
        break;
      case GameStates.Game:
        GameUpdate();
        break;
    }

    LerpMenusBack();

    // Update GameState text
    Entity[] gameStateText = Entity.GetEntitiesByName("GameState");
    if (gameStateText.Length > 0)
    {
      gameStateText[0].GetComponent<TextComponent>().Text = gameState.ToString();
    }
  }

  private void LerpMenusBack()
  {
    switch (gameState)
    {
      case GameStates.MenuControls:
        break;
      case GameStates.Pause:
        break;
      case GameStates.Game:
        // Lerp menu and pause menu back to original position
        Transform menuTr = MenuControlsEntity.GetComponent<Transform>();
        Vec3 menuPos = menuTr.GetPosition();
        float newY = Lerp(menuPos.y, 5f, 3f * dt);
        menuTr.SetPosition(new Vec3(menuPos.x, newY, menuPos.z));

        Transform pauseTr = PauseMenuEntity.GetComponent<Transform>();
        Vec3 pausePos = pauseTr.GetPosition();
        newY = Lerp(pausePos.y, 5f, 3f * dt);
        pauseTr.SetPosition(new Vec3(pausePos.x, newY, pausePos.z));

        break;
      case GameStates.Victory:
      {
        // Lerp menu and pause menu back to original position
        menuTr = MenuControlsEntity.GetComponent<Transform>();
        menuPos = menuTr.GetPosition();
        newY = Lerp(menuPos.y, 5f, 3f * dt);
        menuTr.SetPosition(new Vec3(menuPos.x, newY, menuPos.z));

        pauseTr = PauseMenuEntity.GetComponent<Transform>();
        pausePos = pauseTr.GetPosition();
        newY = Lerp(pausePos.y, 5f, 3f * dt);
        pauseTr.SetPosition(new Vec3(pausePos.x, newY, pausePos.z));

        Transform victoryTr = VictoryMenuEntity.GetComponent<Transform>();
        Vec3 victoryPos = victoryTr.GetPosition();
        float newYVictory = Lerp(victoryPos.y, 0f, 3f * dt);
        victoryTr.SetPosition(new Vec3(victoryPos.x, newYVictory, victoryPos.z));

        if (NITELITE.Input.GetKeyPressed(Keys.SPACE))
        {
            // Back to lobby
        }

        break;
      }
      case GameStates.GameOver:
      {
        // Lerp menu and pause menu back to original position
        menuTr = MenuControlsEntity.GetComponent<Transform>();
        menuPos = menuTr.GetPosition();
        newY = Lerp(menuPos.y, 5f, 3f * dt);
        menuTr.SetPosition(new Vec3(menuPos.x, newY, menuPos.z));

        pauseTr = PauseMenuEntity.GetComponent<Transform>();
        pausePos = pauseTr.GetPosition();
        newY = Lerp(pausePos.y, 5f, 3f * dt);
        pauseTr.SetPosition(new Vec3(pausePos.x, newY, pausePos.z));

        Transform victoryTr = GameOverEntity.GetComponent<Transform>();
        Vec3 victoryPos = victoryTr.GetPosition();
        float newYVictory = Lerp(victoryPos.y, 0f, 3f * dt);
        victoryTr.SetPosition(new Vec3(victoryPos.x, newYVictory, victoryPos.z));

        if (NITELITE.Input.GetKeyPressed(Keys.SPACE))
        {
          // Back to lobby
        }

        break;
      }
    }
  }

  private void MenuControlsUpdate()
  {
    timeInMenu += dt;

    Transform menuTr = MenuControlsEntity.GetComponent<Transform>();
    Vec3 menuPos = menuTr.GetPosition();

    if (NITELITE.Input.GetKeyPressed(Keys.SPACE) && timeInMenu > 2f)
    {
      gameState = GameStates.Game;
    }
    else
    {
      float newY = Lerp(menuPos.y, 0f, 3f * dt);
      menuTr.SetPosition(new Vec3(menuPos.x, newY, menuPos.z));
    }
  }

  private void PauseMenuUpdate()
  {
    Transform menuTr = PauseMenuEntity.GetComponent<Transform>();
    Vec3 menuPos = menuTr.GetPosition();

    if (NITELITE.Input.GetKeyTriggered(Keys.RIGHT))
    {
      pauseButton++;
    }
    if (NITELITE.Input.GetKeyTriggered(Keys.LEFT))
    {
      pauseButton--;
    }

    pauseButton = Math.Max(0, Math.Min(pauseButton, 1));

    if (NITELITE.Input.GetKeyPressed(Keys.SPACE))
    {
      if (pauseButton == 0)
      {
        gameState = GameStates.Game;
      }
      if (pauseButton == 1)
      {
        // Back to lobby
      }
    }
    else
    {
      float newY = Lerp(menuPos.y, 0f, 6f * dt);
      menuTr.SetPosition(new Vec3(menuPos.x, newY, menuPos.z));
    }

    ModelComponent resumeModel = resumeTextEntity.GetComponent<ModelComponent>();
    ModelComponent exitModel = exitTextEntity.GetComponent<ModelComponent>();

    if (pauseButton == 0)
    {
      resumeModel.tint = new Vec3(0.85f, 0.67f, 0.2f);
      exitModel.tint = new Vec3(1f, 1f, 1f);
    }
    if (pauseButton == 1)
    {
      resumeModel.tint = new Vec3(1f, 1f, 1f);
      exitModel.tint = new Vec3(0.85f, 0.67f, 0.2f);
    }
  }

  private void GameUpdate()
  {
    if (NITELITE.Input.GetKeyPressed(Keys.P))
    {
      gameState = GameStates.Pause;
      return;
    }

    if (!waitingForBigWave)
    {
      levelTimer += dt;
    }

    if (currentLevel >= levels.Length)
      return;

    ZombieLevel level = levels[currentLevel];

    // Spawn waves when it's time
    while (nextWaveIndex < level.waves.Length && levelTimer >= level.waves[nextWaveIndex].spawnTime)
    {
      if (zombies.Count != 0 && level.waves[nextWaveIndex].bigWave)
      {
        // Wait for all zombies to die before spawning the next wave
        waitingForBigWave = true;
        break;
      }

      waitingForBigWave = false;
      SpawnWave(level.waves[nextWaveIndex]);
      nextWaveIndex++;

      NL_INFO("Spawning next wave! " + nextWaveIndex);

      //Entity[] waveText = Entity.GetEntitiesByName("WaveText");
      //if (waveText.Length > 0)
      //{
      //  waveText[0].GetComponent<TextComponent>().Text = "Wave " + (nextWaveIndex) + "/" + level.waves.Length;
      //}
    }

    // Update all active zombies
    for (int i = zombies.Count - 1; i >= 0; --i)
    {
      Zombie zombie = zombies[i];
      
      if (zombie.x < 0)
      {
        Vec3 zombPos = zombie.entity.GetComponent<Transform>().GetPosition();

        Entity[] mowers = Entity.GetEntitiesByName("mower");
        bool hitMower = false;
        for (int j = 0; j < mowers.Length; ++j)
        {
          Vec3 mowerPos = mowers[j].GetComponent<Transform>().GetPosition();
          if (mowerPos.y < 1)
          {
            float realZombieY = zombie.y;
            if (zombie.type == "charles_flexing" || zombie.type == "jack_samba")
            {
              realZombieY = (zombie.y + 0.5f) / 0.9f;
            }
            if (realZombieY > mowerPos.z - 0.5f && realZombieY < mowerPos.z + 0.5f)
            {
              // Mower hit
              mowers[j].GetComponent<Transform>().SetPosition(new Vec3(mowerPos.x, mowerPos.y + 0.3f, mowerPos.z));
              zombie.health = 0;
              hitMower = true;
              break;
            }
          }
        }

        if (!hitMower)
        {
          gameState = GameStates.GameOver;
        }
      }

      if (zombie.health <= 0)
      {
        zombie.entity.GetComponent<Transform>().SetPosition(new Vec3(-100, 0, 0));
        zombies.RemoveAt(i);
        continue;
      }

      Transform tr = zombie.entity.GetComponent<Transform>();
      Vec3 pos = tr.GetPosition();
      zombie.prevX = pos.x;
      zombie.x = pos.x - zombie.speed * dt + zombie.ax * dt;
      zombie.y = pos.z + zombie.ay * dt * 60f; // row
      tr.SetPosition(new Vec3(zombie.x, pos.y, zombie.y));

      // lerp ax towards zero
      if (zombie.ax > 0)
      {
        zombie.ax = Lerp(zombie.ax, 0, dt);
      }
      else
      {
        zombie.ax = Math.Max(zombie.ax, 0);
      }

      if (zombie.type == "jack_samba")
      {
        // lerp ay randomly towards -1 or 1
        Random rand = new Random();
        int randomY = rand.Next(0, 2);
        if (randomY == 0)
        {
          zombie.ay = Lerp(zombie.ay, -1, dt);
        }
        else
        {
          zombie.ay = Lerp(zombie.ay, 1, dt);
        }
        
        float oldY = (zombie.y + 0.5f) / 0.9f;
        if (oldY < 0)
        {
          zombie.ay = -zombie.ay;
          zombie.y = -0.5f;
          tr.SetPosition(new Vec3(zombie.x, pos.y, zombie.y));
        }
        if (oldY > 4)
        {
          zombie.ay = -zombie.ay;
          zombie.y = 3.1f;
          tr.SetPosition(new Vec3(zombie.x, pos.y, zombie.y));
        }
      }
      else
      {
        zombie.ay = 0;
      }
    }

    // Next level if all waves for this level are complete and no zombies are alive
    if (nextWaveIndex >= level.waves.Length && zombies.Count == 0)
    {
      levelTimer = 0f;
      nextWaveIndex = 0;

      if (currentLevel < levels.Length - 1)
      {
        currentLevel++;

        Entity[] levelText = Entity.GetEntitiesByName("LevelText");
        if (levelText.Length > 0)
        {
          levelText[0].GetComponent<TextComponent>().Text = "Level " + (currentLevel + 1) + "/" + levels.Length;
        }
      }
      else
      {
        gameState = GameStates.Victory;
      }
    }

    ExplodeEntity.GetComponent<ParticleEmitter>().looping = false;

    PlantUpdate();
    PlantZombieInteractions();
    MowerUpdate();
  }

  private void SpawnWave(ZombieWave wave)
  {
    Random rand = new Random();

    for (int i = 0; i < wave.count; ++i)
    {
      Entity ent = Entity.Create();

      ent.AddComponent<Transform>();
      ent.AddComponent<ModelComponent>();     

      float randomX = (float)(rand.NextDouble() * 2.0f + 3.0f);
      int randomY = rand.Next(0, 5);

      float spawnX = 8 + randomX;
      int row = wave.row;
      if (row == -1) row = randomY;

      ent.GetComponent<Transform>().SetPosition(new Vec3(spawnX, 1, row));
      ent.GetComponent<ModelComponent>().modelPath = wave.type;
      ent.name = "zombie";// wave.type;

      zombies.Add(new Zombie
      {
        entity = ent,
        x = spawnX,
        y = row,
        speed = wave.speed,
        health = wave.health,
        originalHealth = wave.health,
        originalScale = ent.GetComponent<Transform>().GetScale(),
        damage = 10,
        type = wave.type
      });

      switch (wave.type)
      {
        case "team_travis":
          ent.GetComponent<Transform>().SetRotation(new Vec3(0f, -1.57f, 0f));
          break;
        case "charles_flexing":
          ent.GetComponent<Transform>().SetRotation(new Vec3(4.7f, 4.8f, 0f));
          ent.GetComponent<Transform>().SetPosition(new Vec3(spawnX, 1, -0.5f + 0.9f * randomY));
          zombies[zombies.Count - 1].y = -0.5f + 0.9f * randomY;
          break;
        case "team_evan":
          ent.GetComponent<Transform>().SetRotation(new Vec3(0f, -1.57f, 0f));
          break;
        case "jack_samba":
          ent.GetComponent<Transform>().SetRotation(new Vec3(4.7f, 4.8f, 0f));
          ent.GetComponent<Transform>().SetPosition(new Vec3(spawnX, 1, -0.5f + 0.9f * randomY));
          zombies[zombies.Count - 1].y = -0.5f + 0.9f * randomY;
          break;
        case "team_adi":
          ent.GetComponent<Transform>().SetRotation(new Vec3(0f, -1.570f, 0f));
          break;
        case "Taylor":
          ent.GetComponent<Transform>().SetRotation(new Vec3(0f, -0.3f, 0f));
          break;
        case "Will":
          ent.GetComponent<Transform>().SetRotation(new Vec3(0f, 1.3f, 0f));
          break;
      }
    }
  }

  private void PlantUpdate()
  {
    Entity[] bananas = Entity.GetEntitiesByName("banana");
    for (int i = 0; i < bananas.Length; ++i)
    {
      
    }
  }

  private void PlantZombieInteractions()
  {
    Entity[] bullets = Entity.GetEntitiesByName("bullet");

    for (int i = 0; i < bullets.Length; ++i)
    {
      Entity bullet = bullets[i];
      Vec3 bulletPos = bullet.GetComponent<Transform>().GetPosition();

      for (int j = 0; j < zombies.Count; ++j)
      {
        // check if colliding with zombie
        bool collided = false;
        Zombie zombie = zombies[j];

        Vec3 zombiePos = zombie.entity.GetComponent<Transform>().GetPosition();

        if (zombie.type == "charles_flexing" || zombie.type == "jack_samba")
        {
          float oldY = (zombiePos.z + 0.5f) / 0.9f;
          if (bulletPos.x > zombiePos.x - 0.5f && bulletPos.x < zombiePos.x + 0.5f &&
              bulletPos.z > oldY - 0.5f && bulletPos.z < oldY + 0.5f)
          {
            collided = true;
          }
        }
        else
        {
          if (bulletPos.x > zombiePos.x - 0.5f && bulletPos.x < zombiePos.x + 0.5f &&
              bulletPos.z > zombiePos.z - 0.5f && bulletPos.z < zombiePos.z + 0.5f)
          {
            collided = true;
          }
        }

        if (collided)
        {
          string path = bullet.GetComponent<ModelComponent>().modelPath;
          bullet.GetComponent<Transform>().SetPosition(new Vec3(-100, 0, 0));

          switch (path)
          {
            case "1 banana bread or 3 bananas":
              zombie.health -= 34;
              break;
            case "Sliced sourdough bread on board":
              zombie.health -= 10;
              break;
            case "Snickers bar":
              zombie.health -= 15;
              break;
            case "apple":
              zombie.health -= 250;
              break;
            case "green apple":
              break;
            case "materwelon":
              break;
          }

          zombie.health = Math.Max(zombie.health, 0);

          Vec3 oldScale = zombie.entity.GetComponent<Transform>().GetScale();
          float hp = (float)zombie.health / (float)zombie.originalHealth;
          float newSX = 0.2f + 0.8f * hp * zombie.originalScale.x;
          float newSY = 0.2f + 0.8f * hp * zombie.originalScale.y;
          float newSZ = 0.2f + 0.8f * hp * zombie.originalScale.z;
          zombie.entity.GetComponent<Transform>().SetScale(new Vec3(newSX, newSY, newSZ));
        }
      }
    }

    Entity[] plants = Entity.GetEntitiesByName("plant");

    for (int i = 0; i < plants.Length; ++i)
    {
      Entity plant = plants[i];
      Vec3 plantPos = plant.GetComponent<Transform>().GetPosition();
      bool watermelonExplode = false;
      for (int j = 0; j < zombies.Count; ++j)
      {
        Zombie zombie = zombies[j];
        Vec3 zombiePos = zombie.entity.GetComponent<Transform>().GetPosition();

        bool collided = false;

        if (zombie.type == "charles_flexing" || zombie.type == "jack_samba")
        {
          float oldY = (zombiePos.z + 0.5f) / 0.9f;
          if (plantPos.x > zombiePos.x - 0.5f && plantPos.x < zombiePos.x + 0.5f &&
              plantPos.z > oldY - 0.5f && plantPos.z < oldY + 0.5f)
          {
            collided = true;
          }
        }
        else
        {
          if (plantPos.x > zombiePos.x - 0.5f && plantPos.x < zombiePos.x + 0.5f &&
              plantPos.z > zombiePos.z - 0.5f && plantPos.z < zombiePos.z + 0.5f)
          {
            collided = true;
          }
        }

        if (collided)
        {
          // Stop zombie
          zombie.entity.GetComponent<Transform>().SetPosition(new Vec3(zombie.prevX, zombiePos.y, zombiePos.z));
          
          Transform plantTransform = plant.GetComponent<Transform>();
          string mn = plant.GetComponent<ModelComponent>().modelPath;
          if (mn == "materwelon")
          {
            watermelonExplode = true;
          }
          else if (mn == "green apple")
          {
            Vec3 appleScale = plant.GetComponent<Transform>().GetScale() * 0.5f;
            plantTransform.SetScale(appleScale);
            if (appleScale.x < 0.1f)
            {
              plantTransform.SetPosition(new Vec3(-100, 0, 0));
            }
            zombie.health -= 10;
            zombie.ax = 4f;
          }
          else
          {
            float shrinkRate = 0.5f; // shrink per second (i.e., 50% per second)

            Vec3 scale = plantTransform.GetScale();
            scale *= (1.0f - shrinkRate * dt);
            plantTransform.SetScale(scale);

            if (scale.x < 0.2f)
            {
              plantTransform.SetPosition(new Vec3(-100, 0, 0));
              plantTransform.SetScale(new Vec3(0, 0, 0));
            }
          }
        }
      }

      if (watermelonExplode)
      {
        ExplodeEntity.GetComponent<Transform>().SetPosition(plantPos);
        ParticleEmitter explodeEmitter = ExplodeEntity.GetComponent<ParticleEmitter>();
        explodeEmitter.duration = 0.5f;
        explodeEmitter.velocity = new Vec3(0f, 0f, 0f);
        explodeEmitter.velocityVariation = new Vec3(6f, 6f, 6f);
        explodeEmitter.acceleration = new Vec3(0f, -5f, 0f);
        explodeEmitter.accelerationVariation = new Vec3(2f, 2f, 2f);
        explodeEmitter.ModelPath = "materwelon";
        explodeEmitter.looping = true;
        explodeEmitter.emissionRate = 500;
        explodeEmitter.sizeBegin = 0.3f;
        explodeEmitter.sizeEnd = 0.1f;
        explodeEmitter.sizeVariation = 0f;
        explodeEmitter.lifeTime = 0.2f;

        // get all zombies with a distance < 1.5f
        for (int j = 0; j < zombies.Count; ++j)
        {
          Zombie zombie = zombies[j];
          Vec3 zombiePos = zombie.entity.GetComponent<Transform>().GetPosition();

          if (plantPos.x > zombiePos.x - 1.5f && plantPos.x < zombiePos.x + 1.5f &&
              plantPos.z > zombiePos.z - 1.5f && plantPos.z < zombiePos.z + 1.5f)
          {
            zombie.health -= 250;
            zombie.health = Math.Max(zombie.health, 0);

            Vec3 oldScale = zombie.entity.GetComponent<Transform>().GetScale();
            float hp = (float)zombie.health / (float)zombie.originalHealth;
            float newSX = hp * zombie.originalScale.x;
            float newSY = hp * zombie.originalScale.y;
            float newSZ = hp * zombie.originalScale.z;
            zombie.entity.GetComponent<Transform>().SetScale(new Vec3(newSX, newSY, newSZ));
          }
        }
      }
    }
  }

  private void MowerUpdate()
  {
    Entity[] mowers = Entity.GetEntitiesByName("mower");
    for (int i = 0; i < mowers.Length; ++i)
    {
      Vec3 mowerPos = mowers[i].GetComponent<Transform>().GetPosition();
      if (mowerPos.y > 1)
      {
        mowerPos += new Vec3(2.5f, 0, 0) * dt;
        if (mowerPos.x > 15f) mowerPos.x = 100f;
        mowerPos.x = Math.Min(mowerPos.x, 100f);
        mowers[i].GetComponent<Transform>().SetPosition(mowerPos);

        // check collision with zombies
        for (int j = 0; j < zombies.Count; ++j)
        {
          Zombie zombie = zombies[j];

          Transform tr = zombie.entity.GetComponent<Transform>();
          Vec3 pos = tr.GetPosition();
          zombie.y = pos.z;
          bool collided = false;

          if (zombie.type == "charles_flexing" || zombie.type == "jack_samba")
          {
            float realZombieY = (zombie.y + 0.5f) / 0.9f;

            // check x and z
            if (mowerPos.x > pos.x - 0.5f && mowerPos.x < pos.x + 0.5f &&
                mowerPos.z > realZombieY - 0.5f && mowerPos.z < realZombieY + 0.5f)
            {
              collided = true;
            }
          }
          else
          {
            // cxheck x and z
            if (mowerPos.x > pos.x - 0.5f && mowerPos.x < pos.x + 0.5f &&
                mowerPos.z > zombie.y - 0.5f && mowerPos.z < zombie.y + 0.5f)
            {
              collided = true;
            }
          }

          Vec3 zombiePos = zombie.entity.GetComponent<Transform>().GetPosition();
          if (collided)
          {
            // mower hit
            zombie.health = 0;
            zombie.entity.GetComponent<Transform>().SetScale(new Vec3(0f, 0f, 0f));
          }
        }
      }
    }
  }

  public override void Exit()
  {
  }
}