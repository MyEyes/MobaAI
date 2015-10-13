using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MobaLib;

namespace MobaTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ManualCamera2D cam;
        MobaLib.MobaGame moba;
        DrawHelper helper;
        float distance = 0;
        TestMinion m;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cam = new ManualCamera2D(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, GraphicsDevice);
            helper = new DrawHelper(GraphicsDevice);
            cam.CenterHard(new Vector2(500, 500));
            cam.SetZoom(1);
            StartNewGame();
            // TODO: use this.Content to load your game content here
        }

        public void StartNewGame()
        {
            moba = new MobaLib.MobaGame("Testmap.mm");
            Champion c = new Champion(moba.Map, moba.Teams[0], new CharacterInfo("Champion.ci"), moba.Map.Lanes[0].Waypoints[0]);
            c.SetController(AIController.Instantiate("ChampionAIs.dll", "ChampionAIs.TestController", moba.Map, c));
            moba.Map.Add(c);
            Champion c2 = new Champion(moba.Map, moba.Teams[1], new CharacterInfo("Champion.ci"), moba.Map.Lanes[1].Waypoints[0]);
            c2.SetController(AIController.Instantiate("ChampionAIs.dll", "ChampionAIs.TestController", moba.Map, c2));
            moba.Map.Add(c2);

            //m = new TestMinion(moba.Map, moba.Map.Lanes[0], moba.Teams[0], new CharacterInfo("Minion.ci"), new MobaLib.Vector3(500,0, 500));
            //m.SetTarget(new MobaLib.Vector3(500, 0, 500));
            //moba.Map.Add(m);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                StartNewGame();
            if (Keyboard.GetState().IsKeyDown(Keys.L))
                distance += 0.1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.K))
                distance -= 0.1f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mouse = Mouse.GetState();
            /*
            if (mouse.LeftButton == ButtonState.Pressed)
                m.SetTarget(new MobaLib.Vector3(mouse.X, 0, mouse.Y));
             */


            moba.Update((float)gameTime.ElapsedGameTime.TotalSeconds*5f);
            //moba.Update(0.016f);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            List<MobaLib.Polygon> polygons = new List<MobaLib.Polygon>();
            for (int x = 0; x < moba.Map.Bushes.Length; x++)
                polygons.Add(moba.Map.Bushes[x].Bounds);
            helper.DrawFilledPolys(polygons.ToArray(), cam.Transformation, Color.Green);

            polygons.Clear();
            for (int x = 0; x < moba.Map.Collisions.Length; x++)
                polygons.Add(moba.Map.Collisions[x].Bounds);
            helper.DrawFilledPolys(polygons.ToArray(), cam.Transformation, Color.Black);

            for (int x = 0; x < moba.Teams.Length; x++)
                DrawTeamThings(moba.Teams[x], moba.Teams[1-x]);

            polygons.Clear();
            for (int x = 0; x < moba.Map.Attacks.Count; x++)
            {
                MobaLib.Vector3 position = moba.Map.Attacks[x].GetPosition();
                polygons.Add(new Polygon(new MobaLib.Vector3[] { 
                    position+new MobaLib.Vector3(-1,0,-1),
                    position+new MobaLib.Vector3(1,0,-1),
                    position+new MobaLib.Vector3(1,0,1),
                    position+new MobaLib.Vector3(-1,0,1),
                }
                    ));
            }
            
            helper.DrawFilledPolys(polygons.ToArray(), cam.Transformation, Color.White);


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        void DrawTeamThings(Team team, Team enemyTeam)
        {
            Color color = new Color(team.R, team.G, team.B);

            List<MobaLib.Polygon> polygons = new List<MobaLib.Polygon>();
            for (int x = 0; x < moba.Map.Characters.Count; x++)
            {
                if (moba.Map.Characters[x].GetTeam() != team || !moba.Map.Characters[x].IsTargetable(enemyTeam))
                    continue;
                MobaLib.Vector3 position = moba.Map.Characters[x].GetPosition();

                polygons.Add(new Polygon(new MobaLib.Vector3[] { 
                    position+new MobaLib.Vector3(-1,0,-1)*moba.Map.Characters[x].Size/2.0f,
                    position+new MobaLib.Vector3(1,0,-1)*moba.Map.Characters[x].Size/2.0f,
                    position+new MobaLib.Vector3(1,0,1)*moba.Map.Characters[x].Size/2.0f,
                    position+new MobaLib.Vector3(-1,0,1)*moba.Map.Characters[x].Size/2.0f,
                }));
                polygons.Add(new Polygon(new MobaLib.Vector3[]{
                    position-new MobaLib.Vector3(moba.Map.Characters[x].Size/2.0f,0, moba.Map.Characters[x].Size/2.0f+2),
                    position+new MobaLib.Vector3(-1+2*moba.Map.Characters[x].Health/moba.Map.Characters[x].GetInfo().maxHealth,0, -1-4/moba.Map.Characters[x].Size)*moba.Map.Characters[x].Size/2.0f,
                    position+new MobaLib.Vector3(-1+2*moba.Map.Characters[x].Health/moba.Map.Characters[x].GetInfo().maxHealth,0, -1-2/moba.Map.Characters[x].Size)*moba.Map.Characters[x].Size/2.0f,
                    position-new MobaLib.Vector3(moba.Map.Characters[x].Size/2.0f,0, moba.Map.Characters[x].Size/2.0f+1),
                }));
            }

            for (int x = 0; x < moba.Map.Structures.Length; x++)
                if (moba.Map.Structures[x].GetTeam() == team && !moba.Map.Structures[x].IsDead())
                    polygons.Add(moba.Map.Structures[x].Bounds);

            helper.DrawFilledPolys(polygons.ToArray(), cam.Transformation, color);
        }
    }
}
