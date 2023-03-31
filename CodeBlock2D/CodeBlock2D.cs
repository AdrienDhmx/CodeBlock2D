using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CodeBlock2D;
public class CodeBlock2D : Game
{
    private const double _minSpeed = 0.8d;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private double CircleSpeed = _minSpeed;
    private Texture2D circle;
    private Point CirclePos = new(); // 0.0 par default

    public CodeBlock2D()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        Window.AllowUserResizing = true;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        circle = Content.Load<Texture2D>("SmallCircle");
        
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        double milliseconds = gameTime.ElapsedGameTime.TotalMilliseconds;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // get all keys with their states
        KeyboardState keyboard = Keyboard.GetState();

        // loop all pressed key
        foreach (Keys key in keyboard.GetPressedKeys())
        {
            // update circle pos if needed
            switch (key)
            {
                case Keys.Q:
                    CirclePos = new((int)(CirclePos.X - milliseconds * CircleSpeed) , CirclePos.Y);
                    break;
                case Keys.S:
                    CirclePos = new(CirclePos.X, (int)(CirclePos.Y + milliseconds * CircleSpeed));
                    break;
                case Keys.D:
                    CirclePos = new((int)(CirclePos.X + milliseconds * CircleSpeed), CirclePos.Y);
                    break;
                case Keys.Z:
                    CirclePos = new(CirclePos.X, (int)(CirclePos.Y - milliseconds * CircleSpeed));
                    break;
                case Keys.Space:
                    CircleSpeed += 0.05 * CircleSpeed;
                    break;
                default:
                    break;
            }
        }

        CircleSpeed = SpeedCheck(CircleSpeed);
        CirclePos = CheckForOutOfScreen(CirclePos, circle);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        _spriteBatch.Draw(circle, CirclePos.ToVector2(), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private Point CheckForOutOfScreen(Point point, Texture2D texture)
    {
        if (point.X < 0) point.X = 0;
        else if (point.X > Window.ClientBounds.Width - texture.Width) point.X = Window.ClientBounds.Width - texture.Width;

        if (point.Y < 0) point.Y = 0;
        else if (point.Y > Window.ClientBounds.Height - texture.Height) point.Y = Window.ClientBounds.Height - texture.Height;

        return point;
    }

    private double SpeedCheck(double speed)
    {
        if(speed > 10)
        {
            return _minSpeed;
        }
        return speed;
    }
}
