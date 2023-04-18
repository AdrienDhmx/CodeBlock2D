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
    private const int startInventoryX = WindowWidth - (_inventorySize + gapBetweenInventoryBlock) * BlockSize;

    private const int _inventorySize = 6;

    private SpriteFont _font;

    private Texture2D _background;
    private Texture2D _inventoryBlock;
    private Texture2D _inventoryBlockSelected;

    private Texture2D _dirtTexture;
    private Texture2D _grassTexture;
    private Texture2D _playerTexture;

    private int[,] map;

    private float yVelPlayer = 0;
    

    private float xPlayer = WindowWidth / 2 - BlockSize;
    private float yPlayer = WindowHeight / 2 - BlockSize;
    private float _speedPlayer = 0.3f;

    /// <summary>
    /// Key => Value
    /// blockIndex => [blockType, Quantity]
    /// </summary>
    private Dictionary<int, int[]> inventory;
    private int selectedInventoryBlock = 0;

    public CodeBlock2D()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferWidth = WindowWidth;
        _graphics.PreferredBackBufferHeight = WindowHeight;
        _graphics.ApplyChanges();

        map = CreateMap();
        inventory = new();

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
        _grassTexture = Content.Load<Texture2D>("Blocks/grass");
        _playerTexture = Content.Load<Texture2D>("player");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        PlayerPhysics();

        int ellapsedMs = gameTime.ElapsedGameTime.Milliseconds;
        Keys[] keysDown = Keyboard.GetState().GetPressedKeys();

        foreach (Keys key in keysDown)
        {
            switch (key)
            {
                case Keys.Q:
                    xPlayer -= ellapsedMs * _speedPlayer;
                    break;
                case Keys.D:
                    xPlayer += ellapsedMs * _speedPlayer;
                    break;
                case Keys.Space:
                    Jump();
                    break;
                default:
                    break;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        int idBlock;

        GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();

        _spriteBatch.Draw(_background, new Rectangle(0,0,WindowWidth,WindowHeight), Color.White);

        for (int line = 0; line < _nbLine; line++)
        {
            for (int column = 0; column < _nbCol; column++)
            {
                idBlock = map[line, column];
                switch ((BlockEnum)idBlock)
                {
                    case BlockEnum.dirt:
                        _spriteBatch.Draw(_dirtTexture, new Rectangle(BlockSize * column, BlockSize * line, _dirtTexture.Width, _dirtTexture.Height), Color.White);
                        break;

                    case BlockEnum.grass:
                        _spriteBatch.Draw(_grassTexture, new Rectangle(BlockSize * column, BlockSize * line, _grassTexture.Width, _grassTexture.Height), Color.White);
                        break;
                }
            }
        }

        DrawInventory();

        _spriteBatch.Draw(_playerTexture, new Rectangle((int)xPlayer, (int)(yPlayer), _playerTexture.Width, _playerTexture.Height), Color.White);
        _spriteBatch.Draw(_playerTexture, new Rectangle((int)xPlayer, (int)yPlayer, _playerTexture.Width, _playerTexture.Height), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        // free all memory
        _dirtTexture.Dispose();
        _grassTexture.Dispose();
        _playerTexture.Dispose();
        _background.Dispose();
        _inventoryBlock.Dispose();
        _inventoryBlockSelected.Dispose();
        _spriteBatch.Dispose();
        _graphics.Dispose();

        base.UnloadContent();
    }

    private static int[,] CreateMap()
    {
        int[,] map = new int[_nbLine, _nbCol];
        for (int line = 0; line < _nbLine; line++)
        {
            for (int column = 0; column < _nbCol; column++)
            {
                if (line < 2 * _nbLine / 3)
                {
                    map[line, column] = 0;

                }
                else if (line == 2 * _nbLine / 3)
                {

                    map[line, column] = 2;

                }
                else
                {

                    map[line, column] = 1;
                }

            }
        }
        return map;
    }
    private void DrawInventory()
    {
        int inventoryX = startInventoryX;
        int Y = gapBetweenInventoryBlock;
        float blockScale = 0.8f;
        int scaleBlockDif = (BlockSize - (int)(BlockSize * blockScale)) / 2;
        for (int inventoryBlock = 0; inventoryBlock < _inventorySize; inventoryBlock++)
        {
            if (selectedInventoryBlock == inventoryBlock)
            {
                _spriteBatch.Draw(_inventoryBlockSelected, new Rectangle(inventoryX, Y, BlockSize, BlockSize), Color.White);
            }
            else
            {
                _spriteBatch.Draw(_inventoryBlock, new Rectangle(inventoryX, Y, BlockSize, BlockSize), Color.White);
            }

            inventoryX += BlockSize + gapBetweenInventoryBlock;
        }

        foreach (KeyValuePair<int, int[]> kvp in inventory)
        {
            int blocX = startInventoryX + (BlockSize + gapBetweenInventoryBlock) * kvp.Key;
            _spriteBatch.Draw(GetBlockTexture((BlockEnum)kvp.Value[0]), new Vector2(blocX + scaleBlockDif, Y + scaleBlockDif), null, Color.White, 0f, Vector2.Zero, blockScale, SpriteEffects.None, 1);
            _spriteBatch.DrawString(_font, kvp.Value[1].ToString(), new Vector2(blocX + (int)(BlockSize * 0.3), (int)(Y + BlockSize * 0.2)), Color.White);
        }
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
    private Texture2D GetBlockTexture(BlockEnum block)
    {
        return block switch
        {
            BlockEnum.dirt => _dirtTexture,
            BlockEnum.grass => _grassTexture,
            _ => null,
        };
    }
}
