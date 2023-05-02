using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CodeBlock2D;
using System;
using System.Runtime.CompilerServices;
using System.Data.Common;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace CodeBlock2D;
public class CodeBlock2D : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int BlockSize = 32;
    private const int WindowWidth = 42 * BlockSize; // 36 blocs
    private const int WindowHeight = 24 * BlockSize; // 22 blocs
    private const int _nbLine = WindowHeight / BlockSize;
    private const int _nbCol = WindowWidth / BlockSize;
    private const int gapBetweenInventoryBlock = 5;
    private const int startInventoryX = WindowWidth - (BlockSize + gapBetweenInventoryBlock) * _inventorySize;
    private const int _inventorySize = 6;

    private static readonly Random _floorLvl = new Random();

    private SpriteFont _font;

    private Texture2D _background;
    private Texture2D _inventoryBlock;
    private Texture2D _inventoryBlockSelected;

    private Texture2D _dirtTexture;
    private Texture2D _stoneTexture;
    private Texture2D _grassTexture;
    private Texture2D _playerTexture;

    private int[,] map;

    private Texture2D _heartFull;
    private Texture2D _heartEmpty;
<<<<<<< HEAD
    private int health_Bar = 100;
    private bool Attacked = false;
    private bool Healed = false;




    private int _health_Bar = 100;
    private bool _attacked = false;
    private bool _healed = false;

    private float xPlayer = WindowWidth / 2 - BlockSize;
    private float yPlayer = WindowHeight / 2 - BlockSize;
    private float yVelPlayer = 0;
    private float _speedPlayer = 0.3f;
    private float _playerRange = 3 * BlockSize;

    /// <summary>
    /// blockIndex => [blockType, Quantity]
    /// </summary>
    private List<int[]> inventory;
    private int _selectedInventoryBlock = 0;

    public CodeBlock2D()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        _graphics.PreferredBackBufferWidth = WindowWidth;
        _graphics.PreferredBackBufferHeight = WindowHeight;
        _graphics.ApplyChanges();

        map = CreateMap();
        inventory = new();
        // init list with invalid values
        for (int i = 0; i < _inventorySize; i++)
        {
            inventory.Add(new int[] { 0, 0 });
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _font = Content.Load<SpriteFont>("font");
        _background = Content.Load<Texture2D>("background");

        _inventoryBlock = Content.Load<Texture2D>("inventoryBlock");
        _inventoryBlockSelected = Content.Load<Texture2D>("inventoryBlockSelected");

        _dirtTexture = Content.Load<Texture2D>("Blocks/dirt");
        _stoneTexture = Content.Load<Texture2D>("Blocks/stone");
        _grassTexture = Content.Load<Texture2D>("Blocks/grass");
        _playerTexture = Content.Load<Texture2D>("Player/player");

        _heartFull = Content.Load<Texture2D>("heartFull");
        _heartEmpty = Content.Load<Texture2D>("heartEmpty");


        if (_attacked)
        {
            _health_Bar -= 10;
            _attacked = false;
        }

        if (_healed)
        {
            _health_Bar += 10;
            _healed = false;
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        PlayerPhysics();

        int newXplayer;
        int ellapsedMs = gameTime.ElapsedGameTime.Milliseconds;
        Keys[] keysDown = Keyboard.GetState().GetPressedKeys();

        foreach (Keys key in keysDown)
        {
            switch (key)
            {
                case Keys.Q or Keys.Left:
                    newXplayer = ((int)(xPlayer - ellapsedMs * _speedPlayer) / BlockSize);

                    if (map[(int)yPlayer / BlockSize, newXplayer] == 0 && map[((int)yPlayer / BlockSize) + 1, newXplayer] == 0)
                    {
                        xPlayer -= ellapsedMs * _speedPlayer;
                    }
                    break;

                case Keys.D or Keys.Right:
                    newXplayer = ((int)(xPlayer + ellapsedMs * _speedPlayer) / BlockSize) + 1;

                    if (map[(int)yPlayer / BlockSize, newXplayer] == 0 && map[((int)yPlayer / BlockSize) + 1, newXplayer] == 0)
                    {
                        xPlayer += ellapsedMs * _speedPlayer;
                    }
                    break;

                case Keys.Space or Keys.Up:
                    Jump();
                    break;

                case Keys.L:
                    if (health_Bar == 0 )
                    {
                        health_Bar = 100;
                    }
                    else
                    {
                        health_Bar -= 20;
                    }
                    
                    break;

                default:
                    break;
            }
        }

         MouseState mouseState = Mouse.GetState();

        // mouse left button pressed
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            // if mouse click is inside the range of the player
            if (IsPointWithinPlayerRange(mouseState.Position))
            {
                int blockX = mouseState.Position.X / BlockSize;
                int blockY = mouseState.Position.Y / BlockSize;

                int blockTarget = map[blockY, blockX];

                // if clicked block is not air
                if(blockTarget !=  0)
                {
                    // remove block from map
                    map[blockY, blockX] = 0;

                    AddBlockToInventory(blockTarget);
                }
            }
        }

        // mouse right button pressed and their is a block in the selected inventory slot
        if (mouseState.RightButton == ButtonState.Pressed && inventory[_selectedInventoryBlock][0] != 0)
        {
            // if mouse click is inside the range of the player
            if (IsPointWithinPlayerRange(mouseState.Position) && !IsPointOnPlayer(mouseState.Position))
            {
                int blockX = mouseState.Position.X / BlockSize;
                int blockY = mouseState.Position.Y / BlockSize;

                int blockTarget = map[blockY, blockX];

                // if clicked block is air
                if (blockTarget == 0)
                {
                    // remove block from map
                    map[blockY, blockX] = inventory[_selectedInventoryBlock][0];

                    RemoveBlockFromInventory();
                }
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        int idBlock;

        GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();

        _spriteBatch.Draw(_background, new Rectangle(0, 0, WindowWidth, WindowHeight), Color.White);

        for (int line = 0; line < _nbLine; line++)
        {
            for (int column = 0; column < _nbCol; column++)
            {
                idBlock = map[line, column];

                if (idBlock != 0)
                    _spriteBatch.Draw(GetBlockTexture((BlockEnum)idBlock), new Vector2(column * BlockSize, line * BlockSize), Color.White);
            }
        }

        DrawInventory();

        _spriteBatch.Draw(_playerTexture, new Vector2((int)xPlayer, (int)yPlayer), Color.White);

        for (int i = 0; i < _health_Bar / 20; i++)
        {
            _spriteBatch.Draw(_heartFull, new Vector2(8 + i * 50, 10), Color.White);
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }
    private void DrawInventory()
    {
        int inventoryX = startInventoryX;
        int Y = gapBetweenInventoryBlock;
        float blockScale = 0.8f;
        int scaleBlockDif = (BlockSize - (int)(BlockSize * blockScale)) / 2;
        for (int inventoryBlock = 0; inventoryBlock < _inventorySize; inventoryBlock++)
        {
            if (_selectedInventoryBlock == inventoryBlock)
            {
                _spriteBatch.Draw(_inventoryBlockSelected, new Vector2(inventoryX, Y), Color.White);
            }
            else
            {
                _spriteBatch.Draw(_inventoryBlock, new Vector2(inventoryX, Y), Color.White);
            }

            inventoryX += BlockSize + gapBetweenInventoryBlock;
        }

        for (int i = 0; i < _inventorySize; i++)
        {
            if (inventory[i][0] != 0)
            {
                int blocX = startInventoryX + (BlockSize + gapBetweenInventoryBlock) * i;
                _spriteBatch.Draw(GetBlockTexture((BlockEnum)inventory[i][0]), new Vector2(blocX + scaleBlockDif, Y + scaleBlockDif), null, Color.White, 0f, Vector2.Zero, blockScale, SpriteEffects.None, 1);

                _spriteBatch.DrawString(_font, inventory[i][1].ToString(), new Vector2(blocX + (int)(BlockSize * 0.3), (int)(Y + BlockSize * 0.2)), Color.White);
            }
        }
    }
    private static int[,] CreateMap()
    {
        int[,] map = new int[_nbLine, _nbCol];
        int baseFloor = _nbLine * 6 / 9, maxHeight = baseFloor / 2 + 4, formation = 0;
        int floorLvl = _floorLvl.Next(baseFloor - 2, baseFloor + 1);

        for (int column = 0; column < _nbCol; column++)
        {
            if (formation == 1)
            {
                floorLvl = _floorLvl.Next(floorLvl - 1, floorLvl + 2);
                formation = 0;
            }
            else
            {
                formation++;
            }

            if (floorLvl <= maxHeight)
            {
                floorLvl = maxHeight;
            }
            else if (floorLvl >= _nbLine)
            {
                floorLvl--;
            }
            for (int line = 0; line < _nbLine; line++)
            {
                if (line < floorLvl)
                {
                    map[line, column] = 0;

                }
                else if (line == floorLvl)
                {

                    map[line, column] = 2;

                }
                else if (line < floorLvl + 5)
                {

                    map[line, column] = 1;

                }
                else
                {

                    map[line, column] = 3;
                }
            }
        }
        return map;
    }

    protected override void UnloadContent()
    {
        _dirtTexture.Dispose();
        _grassTexture.Dispose();
        _stoneTexture.Dispose();

        _playerTexture.Dispose();

        _background.Dispose();

        _inventoryBlock.Dispose();
        _inventoryBlockSelected.Dispose();

        _heartEmpty.Dispose();
        _heartFull.Dispose();

        _spriteBatch.Dispose();
        _graphics.Dispose();

        base.UnloadContent();
    }

    private void PlayerPhysics()
    {
        int xMatPos = (int)xPlayer / BlockSize, yMatPos = (int)yPlayer / BlockSize, yFloor = -1, ySearch = yMatPos;

        while (yFloor < 0)
        {
            if (map[ySearch, xMatPos] != (int)BlockEnum.air)
            {
                yFloor = ySearch * BlockSize;
            }
            else
            {
                ySearch++;
            }
        }

        if (map[yMatPos + 2, xMatPos] == (int)BlockEnum.air)
        {
            if (yVelPlayer <= 5)
            {
                yVelPlayer += 0.49f;
            }
        }

        if (yVelPlayer != 0)
        {
            if (yVelPlayer > 0)
            {
                if (yPlayer + yVelPlayer < yFloor - 2 * BlockSize)
                {
                    yPlayer += yVelPlayer;
                }
                else
                {
                    yPlayer = yFloor - 2 * BlockSize;
                    yVelPlayer = 0;
                }
            }
            if (yVelPlayer < 0)
            {
                yPlayer += yVelPlayer;
                yVelPlayer += -yVelPlayer * 0.05f;

            }
        }
    }
    private void Jump()
    {
        int xMatPos = (int)xPlayer / BlockSize, yMatPos = (int)yPlayer / BlockSize, yFloor = -1, ySearch = yMatPos;
        while (yFloor < 0)
        {
            if (map[ySearch, xMatPos] != (int)BlockEnum.air)
            {
                yFloor = ySearch * BlockSize;
            }
            else
            {
                ySearch++;
            }
        }

        if (yPlayer == yFloor - 2 * BlockSize)
        {
            yVelPlayer -= 10;
        }
    }

    private void AddBlockToInventory(int block)
    {
        int freeSlotIndex = -1;
        for (int i = 0; i < _inventorySize; i++)
        {
            if (inventory[i][0] == block)
            {
                inventory[i][1]++; // increase qty
                return;
            }
            else if (inventory[i][0] == 0 && freeSlotIndex == -1) // first free slot found
            {
                freeSlotIndex = i;
            }
        }

        if(freeSlotIndex != -1)
        {
            // add new block to inventory
            inventory[freeSlotIndex] = new int[]{ block, 1 };
        }
    }

    private void RemoveBlockFromInventory()
    {
        if(inventory[_selectedInventoryBlock][1] == 1)
        {
            inventory[_selectedInventoryBlock][0] = 0;
        }
        inventory[_selectedInventoryBlock][1]--;
    }

    private bool IsPointWithinPlayerRange(Point mousePos)
    {
        return Math.Abs(mousePos.X - xPlayer) < _playerRange &&
                Math.Abs(mousePos.Y - yPlayer) < _playerRange;
    }

    private bool IsPointOnPlayer(Point pos)
    {
        return Math.Abs(pos.X - xPlayer) < BlockSize &&
                Math.Abs(pos.Y - yPlayer) < BlockSize * 2; // the player height is 2 blocks
    }

    private Texture2D GetBlockTexture(BlockEnum block)
    {
        return block switch
        {
            BlockEnum.dirt => _dirtTexture,
            BlockEnum.grass => _grassTexture,
            BlockEnum.stone => _stoneTexture,
            _ => null,
        };
    }
}
