using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CodeBlock2D;

namespace CodeBlock2D;
public class CodeBlock2D : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int BlockSize = 32;
    private const int WindowWidth = 1152; // 36 blocs
    private const int WindowHeight = 704; // 22 blocs
    private const int _nbLine = WindowHeight / BlockSize;
    private const int _nbCol = WindowWidth / BlockSize;

    private Texture2D _dirtTexture;
    private Texture2D _grassTexture;
    private Texture2D _playerTexture;

    private int[,] Map;

    private int xPlayer = WindowWidth / 2 - BlockSize;
    private int yPlayer = WindowHeight / 2 - BlockSize;

    public CodeBlock2D()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferWidth = 1152;
        _graphics.PreferredBackBufferHeight = 704;
        _graphics.ApplyChanges();

        Map = CreateMap();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _dirtTexture = Content.Load<Texture2D>("dirt");
        _grassTexture = Content.Load<Texture2D>("grass");
        _playerTexture = Content.Load<Texture2D>("player");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        int idBlock;

        GraphicsDevice.Clear(Color.SkyBlue);

        _spriteBatch.Begin();

        for (int line = 0; line < _nbLine; line++)
        {
            for (int column = 0; column < _nbCol; column++)
            {
                idBlock = Map[line, column];
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

        _spriteBatch.Draw(_playerTexture, new Rectangle(xPlayer, yPlayer, _playerTexture.Width, _playerTexture.Height), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        // free all memory
        _dirtTexture.Dispose();
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
                if(line < 2 * _nbLine / 3)
                {
                    map[line, column] = 0;

                } else if (line == 2* _nbLine / 3) {

                    map[line, column] = 2;

                } else {

                    map[line, column] = 1;
                }
                
            }
        }
        return map;
    }
}
