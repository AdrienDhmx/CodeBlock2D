using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CodeBlock2D;
public class CodeBlock2D : Game
{
    private const int BlocSize = 32;
    private const int WindowWidth = 1152; // 36 blocs
    private const int WindowHeight = 704; // 22 blocs

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _dirtTexture;

    private int[] Map;

    public CodeBlock2D()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // fix the window size
        _graphics.PreferredBackBufferWidth = WindowWidth;
        _graphics.PreferredBackBufferHeight = WindowHeight;
        _graphics.ApplyChanges();

        Map = CreateMap();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _dirtTexture = Content.Load<Texture2D>("dirt");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // col and line never change as the window size is fixed, they could be in a global variable
        int col = WindowWidth / BlocSize;
        int line = WindowHeight / BlocSize;
        for (int l = 0; l < line; l++)
        {
            for (int c = 0; c < col; c++)
            {
                if (Map[l * col + c] == 1)
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

    private static int[] CreateMap()
    {
        int col = WindowWidth / BlocSize;
        int line = WindowHeight / BlocSize;

        int[] map = new int[col * line];
        for (int l = 0; l < line; l++)
        {
            for (int c = 0; c < col; c++)
            {
                if(l < line/2) // start drawing dirt block at half the screen height
                {
                    map[l * col + c] = 0;
                }
                else
                {
                    map[l * col + c] = 1;
                }
            }
        }
        return map;
    }
}
