using System;
using RLNET;
using RoguelikeGame.Core;
using RoguelikeGame.Systems;
using RogueSharp.Random;

namespace RogueSharpGame
{
    public static class Game
    {
        //The screen height and widht are in number of tiles
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static RLRootConsole _rootConsole;

        //The map console takes up most of the screen and is where the map will be draw
        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static RLConsole _mapConsole;

        //Below the map console is the message console whitch displays attack rolls and other information
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 11;
        private static RLConsole _messageConsole;

        //Stat console is to the right of the map console and display player and monster stats
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole _statConsole;

        //Above the map is the inventory console which show the player equepment, abilities and item
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole _inventoryConsole;
        //-----------------------------------------------------
        public static DungeonMap DungeonMap { get; private set; }

        public static MessageLog MessageLog { get; private set; }

        public static Player Player { get; set; }

        private static bool _renderRequered = true;
        public static CommandSystem CommandSystem { get; private set; }
        public static IRandom Random { get; private set; }

        public static SchedulingSystem SchedulingSystem { get; set; }
        private static int _mapLevel = 1;

        //private static int _step = 0;
        // Make sure that the setter for Player is not private
        //-----------------------------------------------------
        public static void Main()
        {
            // Establish the seed for the random number generator from the current time
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);
            SchedulingSystem = new SchedulingSystem();

            // The title will appear at the top of the console window 
            // also include the seed used to generate the level
            string consoleTitle = $"RougeSharp - Level {_mapLevel} [Seed: {seed}]";

            string fontFileName = "terminal8x8.png";

            // Create a new MessageLog and print the random seed used to generate the level
            MessageLog = new MessageLog();
            MessageLog.Add("The Rogue arrives on level 1");
            MessageLog.Add($"Level created with seed '{seed}'");

            // Initialize the sub consoles that we will Blit to the root console
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);
            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);

            CommandSystem = new CommandSystem();
                        
            // The next two lines already existed
            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, _mapLevel);
            DungeonMap = mapGenerator.CreateMap();
            // End of existing code
            DungeonMap.UpdatePlayerFieldOfView();

            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;
            _rootConsole.Run();
            
        }
             
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            //Set background color for each console
            _mapConsole.SetBackColor(0, 0, _mapWidth, _mapHeight, Colors.FloorBackground);
            _mapConsole.Print(1, 1, "Map", Colors.TextHeading);

            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Swatch.DbWood);
            _inventoryConsole.Print(1, 1, "Message", Colors.TextHeading);

            bool didPlayerAct = false;
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if (CommandSystem.IsPlayerTurn)
            {

                if (keyPress != null)
                {
                    if (keyPress.Key == RLKey.Up)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                    }
                    else if (keyPress.Key == RLKey.Down)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                    }
                    else if (keyPress.Key == RLKey.Left)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                    }
                    else if (keyPress.Key == RLKey.Right)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                    }
                    else if (keyPress.Key == RLKey.Escape)
                    {
                        _rootConsole.Close();
                    }
                    else if (keyPress.Key == RLKey.Period)
                    {
                        if (DungeonMap.CanMoveDownToNextLevel())
                        {
                            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, ++_mapLevel);
                            DungeonMap = mapGenerator.CreateMap();
                            MessageLog = new MessageLog();
                            CommandSystem = new CommandSystem();
                            _rootConsole.Title = $"RougeSharp - Level {_mapLevel}";
                            didPlayerAct = true;
                            Player.MaxHealth += 100;
                            Player.Health = Player.MaxHealth;
                        }
                    }
                }
            }
            if(didPlayerAct)
            {
                _renderRequered = true;
                CommandSystem.EndPlayerTurn();
            }
            else
            {
                CommandSystem.ActivateMonsters();
                _renderRequered = true;
            }
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (_renderRequered)
            {
                _mapConsole.Clear();
                _statConsole.Clear();
                _messageConsole.Clear();

                DungeonMap.Draw(_mapConsole, _statConsole);
                Player.Draw(_mapConsole, DungeonMap);
                MessageLog.Draw( _messageConsole );
                Player.DrawStats(_statConsole);


                // Blit the sub consoles to the root console in the correct locations
                RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _inventoryHeight);
                RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0);
                RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, _screenHeight - _messageHeight);
                RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole, 0, 0);

                 // Tell RLNET to draw the console that we set            

                _rootConsole.Draw();                        
       
                _renderRequered = false;
            }
            
        }
    }
}