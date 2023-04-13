using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CodeBlock2D;
public class CodeBlock2D : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int BlocSize = 32;
    private const int WindowWidth = 1152; // 36 blocs
    private const int WindowHeight = 704; // 22 blocs
    private const int _nbLine = WindowHeight / BlocSize;
    private const int _nbCol = WindowWidth / BlocSize;

    private Texture2D _dirtTexture;

    private int[,] Map;

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        for (int l = 0; l < _nbLine; l++)
        {
            for (int c = 0; c < _nbCol; c++)
            {
                if (Map[l,c] == 1)
                {
                    _spriteBatch.Draw(_dirtTexture, new Rectangle(BlocSize*c, BlocSize * l, _dirtTexture.Width, _dirtTexture.Height), Color.White);
                }
            }
        }

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
        for (int l = 0; l < _nbLine; l++)
        {
            for (int c = 0; c < _nbCol; c++)
            {
                if(l < _nbLine / 2)
                {
                    map[l, c] = 0;
                }
                else
                {
                    map[l, c] = 1;
                }
                
            }
        }
        return map;
    }
}
