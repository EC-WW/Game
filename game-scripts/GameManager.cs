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
}

public class GameManager : NL_Script
{
  private List<Zombie> zombies = new List<Zombie>();

  private ZombieLevel[] levels = new ZombieLevel[5];
  private int currentLevel = 0;
  private float levelTimer = 0f;
  private int nextWaveIndex = 0;
  private bool waitingForBigWave = false;

  private Entity ExplodeEntity;

  public override void Init()
  {
    levels[0] = new ZombieLevel
    {
      waves = new ZombieWave[]
      {
      new ZombieWave { spawnTime = 2f, count = 1, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 10f, count = 2, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 20f, count = 3, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 21f, count = 1, row = -1, type = "team_adi", speed = 0.7f, health = 80, bigWave = false },
      new ZombieWave { spawnTime = 30f, count = 4, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = true },
      new ZombieWave { spawnTime = 31f, count = 3, row = -1, type = "team_adi", speed = 0.7f, health = 80, bigWave = false },
      }
    };
    levels[1] = new ZombieLevel
    {
      waves = new ZombieWave[]
      {
      new ZombieWave { spawnTime = 2f, count = 5, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 3f, count = 6, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 4f, count = 7, row = -1, type = "team_travis", speed = 0.4f, health = 100, bigWave = false },
      new ZombieWave { spawnTime = 5f, count = 8, row = -1, type = "team_adi", speed = 0.7f, health = 80, bigWave = false },
      new ZombieWave { spawnTime = 7f, count = 40, row = -1, type = "team_adi", speed = 0.7f, health = 80, bigWave = true },
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
      if (zombie.health <= 0)
      {
        zombie.entity.GetComponent<Transform>().SetPosition(new Vec3(-100, 0, 0));
        zombies.RemoveAt(i);
        continue;
      }
      if (zombie.x < 0)
      {
        Vec3 zombPos = zombie.entity.GetComponent<Transform>().GetPosition();
        //zombie.entity.GetComponent<Transform>().SetPosition(new Vec3(-100, 0, 0));
        //zombies.RemoveAt(i);

        Entity[] mowers = Entity.GetEntitiesByName("mower");
        for (int j = 0; j < mowers.Length; ++j)
        {
          Vec3 mowerPos = mowers[j].GetComponent<Transform>().GetPosition();
          if (mowerPos.y < 1)
          if (zombPos.z > mowerPos.z - 0.5f && zombPos.z < mowerPos.z + 0.5f)
          {
            // Mower hit
            mowers[j].GetComponent<Transform>().SetPosition(new Vec3(mowerPos.x, mowerPos.y + 0.3f, mowerPos.z));
            break;
          }
        }
        if (zombie.x < -1)
        {

        }

        // Check Lawnmowers
        continue;
      }

      Transform tr = zombie.entity.GetComponent<Transform>();
      Vec3 pos = tr.GetPosition();
      zombie.x = pos.x - zombie.speed * dt + zombie.ax * dt;
      zombie.y = pos.z; // row
      tr.SetPosition(new Vec3(zombie.x, pos.y, pos.z));

      // lerp ax towards zero
      if (zombie.ax > 0)
      {
        zombie.ax = Lerp(zombie.ax, 0, dt);
      }
      else
      {
        zombie.ax = Math.Max(zombie.ax, 0);
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

        //Entity[] waveText = Entity.GetEntitiesByName("WaveText");
        //if (waveText.Length > 0)
        //{
        //  waveText[0].GetComponent<TextComponent>().Text = "Wave 1/" + levels[currentLevel].waves.Length;
        //}
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
        case "team_charles":
          ent.GetComponent<Transform>().SetRotation(new Vec3(0f, 4f, 0f));
          break;
        case "team_evan":
          ent.GetComponent<Transform>().SetRotation(new Vec3(0f, -1.57f, 0f));
          break;
        case "team_jack":
          ent.GetComponent<Transform>().SetRotation(new Vec3(0f, -1.57f, 0f));
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
      bool watermelonExplode = false;

      for (int j = 0; j < zombies.Count; ++j)
      {
        // check if colliding with zombie
        bool collided = false;
        Zombie zombie = zombies[j];

        Vec3 zombiePos = zombie.entity.GetComponent<Transform>().GetPosition();

        if (bulletPos.x > zombiePos.x - 0.5f && bulletPos.x < zombiePos.x + 0.5f &&
            bulletPos.z > zombiePos.z - 0.5f && bulletPos.z < zombiePos.z + 0.5f)
        {
          collided = true;
        }

        if (collided)
        {
          string path = bullet.GetComponent<ModelComponent>().modelPath;

          if (path != "green apple")
          {
            bullet.GetComponent<Transform>().SetPosition(new Vec3(-100, 0, 0));
          }
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
              Vec3 bulletScale = bullet.GetComponent<Transform>().GetScale() * 0.5f;
              bullet.GetComponent<Transform>().SetScale(bulletScale);
              if (bulletScale.x < 0.1f)
              {
                bullet.GetComponent<Transform>().SetPosition(new Vec3(-100, 0, 0));
              }
              zombie.health -= 10;
              zombie.ax = 4f;
              break;
            case "materwelon":
              //zombie.health -= 10;
              watermelonExplode = true;
              break;
          }

          zombie.health = Math.Max(zombie.health, 0);

          Vec3 oldScale = zombie.entity.GetComponent<Transform>().GetScale();
          float hp = (float)zombie.health / (float)zombie.originalHealth;
          float newSX = hp * zombie.originalScale.x;
          float newSY = hp * zombie.originalScale.y;
          float newSZ = hp * zombie.originalScale.z;
          zombie.entity.GetComponent<Transform>().SetScale(new Vec3(newSX, newSY, newSZ));
        }
      }

      if (watermelonExplode)
      {
        ExplodeEntity.GetComponent<Transform>().SetPosition(bulletPos);
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

          if (bulletPos.x > zombiePos.x - 1.5f && bulletPos.x < zombiePos.x + 1.5f &&
              bulletPos.z > zombiePos.z - 1.5f && bulletPos.z < zombiePos.z + 1.5f)
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
          Vec3 zombiePos = zombie.entity.GetComponent<Transform>().GetPosition();
          if (mowerPos.x > zombiePos.x - 0.5f && mowerPos.x < zombiePos.x + 0.5f &&
              mowerPos.z > zombiePos.z - 0.5f && mowerPos.z < zombiePos.z + 0.5f)
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