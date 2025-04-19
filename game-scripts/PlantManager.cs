/**
/ file:   PlantPlacer.cs
/ author: aditya.prakash
/ date:   April 16, 2025
/ Copyright (c) 2025 DigiPen (USA) Corporation. 
/ 
/ brief:  Example script for the NITELITE engine, uses several of its features
**/
using System;
using System.Collections.Generic;
using System.Numerics;
using NITELITE;

public class BananaPlant
{
  public Entity entity;
  public int health;
  public float shootCooldown;
  public float shootTimer;
}

public class BananaBullet
{
  public Entity entity;
  public float speed;
  public float damage;
  public bool active;
}

public class PlantData
{
  public Entity entity;
  public float health;
  public float shootCooldown;
  public float shootTimer;
  public BulletTypes type;
}
public enum BulletTypes
{
  Banana,
  Bread,
  Snickers,
  Snickers1,
  Snickers2,
  Apple,
  GreenApple,
  Watermelon
}

public class BulletData
{
  public Entity Entity;
  public float Speed;
  public float Damage;
  public bool Active;
  public BulletTypes type;
}


public class PlantManager : NL_Script
{
  // ——— Data containers —————————————————————————————————————————————
  //private float[] PlantCooldowns = new float[6] { 2.5f, 3.0f, 1.5f, 2.0f, 2.0f, 3.0f };

  // ——— Constants —————————————————————————————————————————————————————
  private const int MaxBullets = 1000;
  private const float OffscreenX = 1000f;
  private const int GridWidth = 9;  // X indices 0..8
  private const int GridHeight = 5;  // Y indices 0..4
  private const float InitialKeyDelay = 0.5f;
  private const float RepeatKeyDelay = 0.1f;
  private Vec3 cursScale;

  // ——— Plant definitions —————————————————————————————————————————————

  private static readonly string[] PlantTypes = new string[6]
  {
    "1 banana bread or 3 bananas",       // Banana
    "Sliced sourdough bread on board",   // Bread
    "Snickers bar",                      // Snickers
    "apple",                             // Apple
    "green apple",                       // Green Apple
    "materwelon",                        // Watermelon
  };

  private static readonly Vec3[] SelectedLocations = new Vec3[6]
  {
    new Vec3(-0.4f, 0.184f, -0.834f),
    new Vec3(-0.249f, 0.184f, -0.834f),
    new Vec3(-0.093f, 0.184f, -0.834f),
    new Vec3(0.06f, 0.184f, -0.834f),
    new Vec3(0.226f, 0.184f, -0.834f),
    new Vec3(0.387f, 0.184f, -0.834f),
  };

  // ——— Editor‐assigned references ————————————————————————————————————
  public Entity ent;
  public Entity highlight;
  public Entity preview;

  public Entity bananaCost;
  public Entity breadCost;
  public Entity snickersCost;
  public Entity appleCost;
  public Entity gAppleCost;
  public Entity watermelonCost;

  public Entity moneyText;

  // ——— Internal state —————————————————————————————————————————————
  private int bananaPrice;
  private int breadPrice;
  private int snickersPrice;
  private int applePrice;
  private int gApplePrice;
  private int watermelonPrice;
  private Transform entTransform;
  private float originalCursorY;
  private float timeElapsed;
  private int gridPosX;
  private int gridPosY;
  private float currentMoney = 0;
  private int currentPlant = 0;
  private float keyPressTimer = 0.0f;
  private readonly float[] emitTimers = new float[45];
  private readonly List<PlantData> plants = new List<PlantData>();
  private readonly List<BulletData> bullets = new List<BulletData>();
  private readonly Queue<int> freeBulletIndices = new Queue<int>();

  private List<BananaPlant> bananaPlants = new List<BananaPlant>();
  private List<BananaBullet> bananaBullets = new List<BananaBullet>();

  private bool cursorOnPlant = false;
  private float moneyTimer = 0f;
  private GameStates gameState = GameStates.MenuControls;
  public override void Init()
  {
    entTransform = ent.GetComponent<Transform>();
    originalCursorY = entTransform.GetPosition().y;
    cursScale = entTransform.GetScale();

    ParticleEmitter emitter = ent.GetComponent<ParticleEmitter>();
    emitter.looping = false;
    emitter.emissionRate = 0.0f;

    for (int i = 0; i < emitTimers.Length; ++i)
    {
      emitTimers[i] = 0.0f;
    }

    Entity[] cubes = Entity.GetEntitiesByName("Cube");

    for (int i = 0; i < cubes.Length; ++i)
    {
      var cube = cubes[i];
      cube.GetComponent<ParticleEmitter>().emissionRate = 0.0f;
      cube.GetComponent<ParticleEmitter>().lifeTime = 0.0f;
      cube.GetComponent<ParticleEmitter>().ModelPath = "team_evan";
    }

    bananaPrice = int.Parse(bananaCost.GetComponent<TextComponent>().Text.Substring(1));
    breadPrice = int.Parse(breadCost.GetComponent<TextComponent>().Text.Substring(1));
    snickersPrice = int.Parse(snickersCost.GetComponent<TextComponent>().Text.Substring(1));
    applePrice = int.Parse(appleCost.GetComponent<TextComponent>().Text.Substring(1));
    gApplePrice = int.Parse(gAppleCost.GetComponent<TextComponent>().Text.Substring(1));
    watermelonPrice = int.Parse(watermelonCost.GetComponent<TextComponent>().Text.Substring(1));
  }

  public override void Update()
  {
    // Update GameState text
    Entity[] gameStateText = Entity.GetEntitiesByName("GameState");
    if (gameStateText.Length > 0)
    {
      string txt = gameStateText[0].GetComponent<TextComponent>().Text;
      gameState = (GameStates)Enum.Parse(typeof(GameStates), txt);
    }

    switch (gameState)
    {
      case GameStates.MenuControls:
        MenuControlsUpdate();
        break;
      case GameStates.Pause:
        break;
      case GameStates.Game:
        GameUpdate();
        break;
    }
  }

  private void MenuControlsUpdate()
  {

  }

  private void GameUpdate()
  {
    timeElapsed += dt;
    moneyTimer += dt;
    entTransform = ent.GetComponent<Transform>();
    Vec3 entPos = entTransform.GetPosition();
    entPos = new Vec3(entPos.x, originalCursorY + Sin(timeElapsed) * 0.05f, entPos.z);
    //Vec3 entRot = entTransform.GetRotation();
    //entTransform.SetRotation(new Vec3(entRot.x, entRot.y + dt * 0.3f, entRot.z));
    entTransform.SetPosition(entPos);

    for (int i = 0; i < emitTimers.Length; ++i)
    {
      emitTimers[i] -= dt;
    }
    keyPressTimer -= dt;

    // Select cell with a 0.5s initial delay, then 0.2s repeats
    if (NITELITE.Input.GetKeyTriggered(Keys.D))
    {
      ++gridPosX;
      keyPressTimer = InitialKeyDelay;      // 0.5f
    }
    else if (NITELITE.Input.GetKeyPressed(Keys.D))
    {
      if (keyPressTimer <= 0f)
      {
        ++gridPosX;
        keyPressTimer = RepeatKeyDelay;
      }
    }
    else if (NITELITE.Input.GetKeyTriggered(Keys.A))
    {
      --gridPosX;
      keyPressTimer = InitialKeyDelay;
    }
    else if (NITELITE.Input.GetKeyPressed(Keys.A))
    {
      if (keyPressTimer <= 0f)
      {
        --gridPosX;
        keyPressTimer = RepeatKeyDelay;
      }
    }
    else if (NITELITE.Input.GetKeyTriggered(Keys.W))
    {
      ++gridPosY;
      keyPressTimer = InitialKeyDelay;
    }
    else if (NITELITE.Input.GetKeyPressed(Keys.W))
    {
      if (keyPressTimer <= 0f)
      {
        ++gridPosY;
        keyPressTimer = RepeatKeyDelay;
      }
    }
    else if (NITELITE.Input.GetKeyTriggered(Keys.S))
    {
      --gridPosY;
      keyPressTimer = InitialKeyDelay;
    }
    else if (NITELITE.Input.GetKeyPressed(Keys.S))
    {
      if (keyPressTimer <= 0f)
      {
        --gridPosY;
        keyPressTimer = RepeatKeyDelay;
      }
    }
    else
    {
      // no arrow key held or just released → reset timer so next press is “initial”
      keyPressTimer = 0f;
    }

    if (moneyTimer > 1f)
    {
      currentMoney += 5;
      moneyTimer -= 1f;
    }
    moneyText.GetComponent<TextComponent>().Text = "Money: $" + currentMoney.ToString();

    if (NITELITE.Input.GetKeyTriggered(Keys.RIGHT))
    {
      currentPlant = (currentPlant + 1) % PlantTypes.Length;
      highlight.GetComponent<Transform>().SetPosition(SelectedLocations[currentPlant]);
    }
    if (NITELITE.Input.GetKeyTriggered(Keys.LEFT))
    {
      currentPlant = (currentPlant - 1 + PlantTypes.Length) % PlantTypes.Length;
      highlight.GetComponent<Transform>().SetPosition(SelectedLocations[currentPlant]);
    }

    //NL_INFO("Grid Pos: " + gridPosX + ", " + gridPosY);

    // 5x9 grid
    gridPosX = Math.Max(0, Math.Min(gridPosX, 8));
    gridPosY = Math.Max(0, Math.Min(gridPosY, 4));

    Entity[] cubes = Entity.GetEntitiesByName("Cube");

    for (int i = 0; i < cubes.Length; ++i)
    {
      var cube = cubes[i];
      var tf = cube.GetComponent<Transform>();

      var tfX = tf.GetPosition().x;
      var tfZ = tf.GetPosition().z;

      if (tfX != gridPosX || tfZ != gridPosY) continue;

      entTransform.SetPosition(new Vec3(tfX, entPos.y, tfZ));

      if (NITELITE.Input.GetKeyTriggered(Keys.SPACE) && !cursorOnPlant)
      {
        bool canPlace = true;
        switch (PlantTypes[currentPlant])
        {
          case "1 banana bread or 3 bananas":
            if (currentMoney >= bananaPrice) currentMoney -= bananaPrice;
            else canPlace = false;
            break;
          case "Sliced sourdough bread on board":
            if (currentMoney >= breadPrice) currentMoney -= breadPrice;
            else canPlace = false;
            break;
          case "Snickers bar":
            if (currentMoney >= snickersPrice) currentMoney -= snickersPrice;
            else canPlace = false;
            break;
          case "apple":
            if (currentMoney >= applePrice) currentMoney -= applePrice;
            else canPlace = false;
            break;
          case "green apple":
            if (currentMoney >= gApplePrice) currentMoney -= gApplePrice;
            else canPlace = false;
            break;
          case "materwelon":
            if (currentMoney >= watermelonPrice) currentMoney -= watermelonPrice;
            else canPlace = false;
            break;
        }

        if (canPlace)
        {

          tf.SetScale(new Vec3(0.95f, 0.95f, 0.95f));
          emitTimers[i] = 0.5f;

          Entity ent = Entity.Create();
          ent.AddComponent<Transform>();
          ent.AddComponent<ModelComponent>();

          Transform entTf = ent.GetComponent<Transform>();
          if (PlantTypes[currentPlant] == "Snickers bar")
          {
            entTf.SetPosition(new Vec3(tfX + 0.1f, 0.7f, tfZ - 0.2f));
          }
          else if (PlantTypes[currentPlant] == "Sliced sourdough bread on board")
          {
            entTf.SetPosition(new Vec3(tfX + 0.1f, 0.75f, tfZ - 0.1f));
          }
          else if (PlantTypes[currentPlant] == "materwelon")
          {
            entTf.SetPosition(new Vec3(tfX + 0.05f, 0.8f, tfZ - 0.1f));
          }
          else if (PlantTypes[currentPlant] == "1 banana bread or 3 bananas")
          {
            entTf.SetPosition(new Vec3(tfX + 0.1f, 0.86f, tfZ - 0.1f));
            entTf.SetRotation(new Vec3(0f, -1.57f, 0f));
          }
          else
          {
            entTf.SetPosition(new Vec3(tfX, 0.86f, tfZ));
          }

          ModelComponent entModel = ent.GetComponent<ModelComponent>();
          entModel.modelPath = PlantTypes[currentPlant];

          switch (PlantTypes[currentPlant])
          {
            case "1 banana bread or 3 bananas":
              ent.GetComponent<Transform>().SetScale(new Vec3(0.7f, 0.7f, 0.7f));
              ent.name = "plant";
              plants.Add(new PlantData
              {
                entity = ent,
                health = 100,
                shootCooldown = 2.5f,
                shootTimer = 1f,
                type = BulletTypes.Banana
              });
              break;
            case "Sliced sourdough bread on board":
              ent.GetComponent<Transform>().SetScale(new Vec3(0.7f, 0.7f, 0.7f));
              ent.name = "plant";
              plants.Add(new PlantData
              {
                entity = ent,
                health = 100,
                shootCooldown = 0.75f,
                shootTimer = 1f,
                type = BulletTypes.Bread
              });
              break;
            case "Snickers bar":
              ent.GetComponent<Transform>().SetScale(new Vec3(0.7f, 0.7f, 0.7f));
              ent.name = "plant";
              plants.Add(new PlantData
              {
                entity = ent,
                health = 100,
                shootCooldown = 1.5f,
                shootTimer = 1f,
                type = BulletTypes.Snickers
              });
              break;
            case "apple":
              ent.GetComponent<Transform>().SetScale(new Vec3(0.7f, 0.7f, 0.7f));
              ent.name = "plant";
              plants.Add(new PlantData
              {
                entity = ent,
                health = 100,
                shootCooldown = 1f,
                shootTimer = 0f,
                type = BulletTypes.Apple
              });
              break;
            case "green apple":
              ent.GetComponent<Transform>().SetScale(new Vec3(0.7f, 0.7f, 0.7f));
              ent.name = "plant";
              plants.Add(new PlantData
              {
                entity = ent,
                health = 1000,
                shootCooldown = 1f,
                shootTimer = 0f,
                type = BulletTypes.GreenApple
              });
              break;
            case "materwelon":
              ent.GetComponent<Transform>().SetScale(new Vec3(0.7f, 0.7f, 0.7f));
              ent.name = "plant";
              plants.Add(new PlantData
              {
                entity = ent,
                health = 25,
                shootCooldown = 1f,
                shootTimer = 0f,
                type = BulletTypes.Watermelon
              });
              break;
          }
        }
      }
    }

    for (int i = 0; i < emitTimers.Length; ++i)
    {
      if (emitTimers[i] > 0)
      {
        ParticleEmitter emitter = cubes[i].GetComponent<ParticleEmitter>();
        emitter.ModelPath = "Rei chikita";
        emitter.emissionRate = 0f;
        emitter.duration = 1.0f;
        emitter.emissionRate = 15;
        emitter.lifeTime = 1f;
        emitter.sizeBegin = 0.2f;
        emitter.sizeEnd = 0.1f;
        emitter.sizeVariation = 0f;
        emitter.velocity = new Vec3(0, 1, 0);
        emitter.velocityVariation = new Vec3(1, 1, 1);
        emitter.acceleration = new Vec3(0, -2, 0);
        emitter.accelerationVariation = new Vec3(1, 0, 1);
        emitter.looping = true;
      }
      else
      {
        ParticleEmitter emitter = cubes[i].GetComponent<ParticleEmitter>();
        emitter.duration = 1.0f;
        emitter.emissionRate = 0.0f;
        emitter.looping = false;
      }
    }

    UpdatePreview();
    UpdatePlants();
    UpdateBullets();
  }

  public override void Exit()
  {
    // nothing to clean up
  }
  private float Sin(float x) => (float)Math.Sin(x);

  private void UpdatePreview()
  {
    Transform previewTransform = preview.GetComponent<Transform>();
    Vec3 rot = previewTransform.GetRotation();
    previewTransform.SetRotation(new Vec3(rot.x, rot.y + dt * 0.3f, rot.z));

    ModelComponent previewModel = preview.GetComponent<ModelComponent>();
    previewModel.modelPath = PlantTypes[currentPlant];

    // check if it's overlapping with an existing plant. If so, hide the preview.
    bool doesOverlap = false;
    for (int i = 0; i < plants.Count; ++i)
    {
      var plant = plants[i];
      var plantPos = plant.entity.GetComponent<Transform>().GetPosition();
      var previewPos = entTransform.GetPosition();
      float dist = MathF.Sqrt(MathF.Pow(plantPos.x - previewPos.x, 2) + MathF.Pow(plantPos.z - previewPos.z, 2));

      if (dist < 0.5f)
      {
        doesOverlap = true;
        previewModel.modelPath = "wire_line";
        break;
      }
    }

    cursorOnPlant = doesOverlap;
    if (doesOverlap)
    {
      // lerp cursScale x and z to 1f
      cursScale.x = Lerp(cursScale.x, 1f, dt * 5f);
      cursScale.z = Lerp(cursScale.z, 1f, dt * 5f);
    }
    else
    {
      // lerp to 0.8f
      cursScale.x = Lerp(cursScale.x, 0.8f, dt * 5f);
      cursScale.z = Lerp(cursScale.z, 0.8f, dt * 5f);
    }
    entTransform.SetScale(cursScale);
  }
  private float Lerp(float a, float b, float t)
  {
    return a + (b - a) * t;
  }

  private void UpdatePlants()
  {
    for (int i = 0; i < plants.Count; ++i)
    {
      PlantData plant = plants[i];
      if (plant.health <= 0)
      {
        plants.RemoveAt(i);
        i--;
        continue;
      }

      // Shooting:
      plant.shootTimer -= dt;
      if (plant.shootTimer > 0f) continue;

      if (plant.type == BulletTypes.Snickers)
      {
        SpawnBullet(plant, 0);
        SpawnBullet(plant, 1);
        SpawnBullet(plant, 2);
      }
      else if (plant.type == BulletTypes.Apple)
      {
        SpawnBullet(plant, 3);
        plant.health = 0;
        plants.RemoveAt(i);
        plant.entity.GetComponent<Transform>().SetPosition(new Vec3(-100, 0, 0));
        i--;
        continue;
      }
      else if (plant.type == BulletTypes.Watermelon)
      {
        //SpawnBullet(plant, 3);
        //plant.health = 0;
        //plants.RemoveAt(i);
        //plant.entity.GetComponent<Transform>().SetPosition(new Vec3(-100, 0, 0));
        //i--;
        continue;
      }
      else if (plant.type == BulletTypes.GreenApple)
      {
        //SpawnBullet(plant, 3);
        //plant.health = 0;
        //plants.RemoveAt(i);
        //plant.entity.GetComponent<Transform>().SetPosition(new Vec3(-100, 0, 0));
        //i--;
        continue;
      }
      else
      {
        SpawnBullet(plant, 0);
      }

      plant.shootTimer = plant.shootCooldown;
    }

  } // UpdatePlants

  private void SpawnBullet(PlantData plant, int special)
  {
    Vec3 plantPos = plant.entity.GetComponent<Transform>().GetPosition();
    bool hasValidTarget = special == 3;

    Entity[] zombies = Entity.GetEntitiesByName("zombie");
    for (int i = 0; i < zombies.Length; ++i)
    {
      Vec3 pos = zombies[i].GetComponent<Transform>().GetPosition();
      
      if (pos.x > plantPos.x && pos.x - 10f < plantPos.x)
      {
        if (plant.type != BulletTypes.Snickers)
        {
          string modelName = zombies[i].GetComponent<ModelComponent>().modelPath;
          if (modelName == "jack_samba" || modelName == "charles_flexing")
          {
            float newY = (pos.z + 0.5f) / 0.9f;
            if (newY > plantPos.z - 0.5f && newY < plantPos.z + 0.5f)
            {
              hasValidTarget = true;
              break;
            }
          }
          else if (pos.z > plantPos.z - 0.5f && pos.z < plantPos.z + 0.5f)
          {
            hasValidTarget = true;
            break;
          }
        }
        else
        {
          Vec3 toZombie = pos - plantPos;
          float length = MathF.Sqrt(toZombie.x * toZombie.x + toZombie.z * toZombie.z);

          if (length > 0f && toZombie.x > -0.25f) // only in front
          {
            NL_INFO("Shoot!");
            float dot = (toZombie.x / length); // plant's forward is (1, 0, 0), so dot = cos(θ)
            float coneCos = MathF.Cos(0.291f); // ≈ 0.957

            if (dot >= coneCos)
            {
              hasValidTarget = true;
              break;
            }
          }
        }
      }
    }
    if (!hasValidTarget) return;

    int idx;
    if (freeBulletIndices.Count > 0)
    {
      idx = freeBulletIndices.Dequeue();
    }
    else if (bullets.Count < MaxBullets)
    {
      bullets.Add(CreateBulletEntity());
      idx = bullets.Count - 1;
    }
    else
    {
      // reached maximum bullets
      return;
    }

    var b = bullets[idx];
    b.Active = true;
    b.Speed = 5f;
    b.Damage = 10f;
    b.Entity.name = "bullet";

    // position & appearance
    var tf = b.Entity.GetComponent<Transform>();
    tf.SetPosition(plantPos);
    tf.SetScale(new Vec3(0.4f, 0.4f, 0.4f));

    var mc = b.Entity.GetComponent<ModelComponent>();
    mc.modelPath = GetModelName(plant.type);

    var pe = b.Entity.GetComponent<ParticleEmitter>();
    pe.ModelPath = GetModelName(plant.type);

    b.type = GetBulletTypes(mc.modelPath);
    if (b.type == BulletTypes.Snickers)
    {
      if (special == 1)
      {
        b.type = BulletTypes.Snickers1;
      }
      else if (special == 2)
      {
        b.type = BulletTypes.Snickers2;
      }
    }
  }

  private BulletData CreateBulletEntity()
  {
    var e = Entity.Create();
    e.name = "bullet";
    e.AddComponent<Transform>();
    e.AddComponent<ModelComponent>();
    e.AddComponent<ParticleEmitter>();

    var tf = e.GetComponent<Transform>();
    tf.SetPosition(new Vec3(OffscreenX, 0, 0));

    // var mc = e.GetComponent<ModelComponent>();
    // modelPath set at spawn

    var pe = e.GetComponent<ParticleEmitter>();
    pe.duration = 1f;
    pe.emissionRate = 5f;
    pe.lifeTime = 0.5f;
    pe.sizeBegin = 0.1f;
    pe.sizeEnd = 0.1f;
    pe.looping = true;
    pe.velocityVariation = new Vec3(0.1f, 0.1f, 0.1f);
    pe.accelerationVariation = new Vec3(0.1f, 0.1f, 0.1f);

    return new BulletData
    {
      Entity = e,
      Speed = 0f,
      Damage = 0f,
      Active = false
    };
  }

  private void UpdateBullets()
  {
    for (int i = 0; i < bullets.Count; i++)
    {
      var b = bullets[i];
      if (!b.Active) continue;

      var tf = b.Entity.GetComponent<Transform>();
      var pos = tf.GetPosition();
      var nextX = pos.x;
      var nextY = pos.z;

      switch (b.type)
      {
        case BulletTypes.Banana:
          nextX += b.Speed * dt;
          break;
        case BulletTypes.Bread:
          nextX += b.Speed * dt;
          break;
        case BulletTypes.Snickers:
          nextX += b.Speed * dt;
          break;
        case BulletTypes.Snickers1:
          nextX += b.Speed * dt;
          nextY += 1.5f * dt;
          break;
        case BulletTypes.Snickers2:
          nextX += b.Speed * dt;
          nextY -= 1.5f * dt;
          break;
        case BulletTypes.Apple:
          nextX += b.Speed * dt;
          break;
        case BulletTypes.GreenApple:
          //nextX += b.Speed * dt;
          break;
        case BulletTypes.Watermelon:
          //nextX += b.Speed * dt;
          break;
      }

      tf.SetPosition(new Vec3(nextX, pos.y, nextY));

      if (nextX < -10f || nextX > 15f)
      {
        b.Active = false;
        tf.SetPosition(new Vec3(OffscreenX, 0, 0));
        freeBulletIndices.Enqueue(i);
      }
    }
  }
  public static BulletTypes GetBulletTypes(string description)
  {
    switch (description)
    {
      case "1 banana bread or 3 bananas":
        return BulletTypes.Banana;
      case "Sliced sourdough bread on board":
        return BulletTypes.Bread;
      case "Snickers bar":
        return BulletTypes.Snickers;
      case "apple":
        return BulletTypes.Apple;
      case "green apple":
        return BulletTypes.GreenApple;
      case "materwelon":
        return BulletTypes.Watermelon;
      default:
        return BulletTypes.Watermelon;
    }
  }

  public static string GetModelName(BulletTypes type)
  {
    switch (type)
    {
      case BulletTypes.Banana: return "1 banana bread or 3 bananas";
      case BulletTypes.Bread: return "Sliced sourdough bread on board";
      case BulletTypes.Snickers: return "Snickers bar";
      case BulletTypes.Snickers1: return "Snickers bar";
      case BulletTypes.Snickers2: return "Snickers bar";
      case BulletTypes.Apple: return "apple";
      case BulletTypes.GreenApple: return "green apple";
      case BulletTypes.Watermelon: return "materwelon";
    };
    return "1 banana bread or 3 bananas";
  }

} // PlantManager
