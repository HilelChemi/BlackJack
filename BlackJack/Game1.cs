using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BlackJack
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _cardsTexture;
        private int cardWidth = 40;
        private int cardHeight = 60;
        private CardsDeck deck = new CardsDeck();
        card cardToDesplay = new card(-1,-1);
        private KeyboardState _previousKeyboardState;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _cardsTexture = Content.Load<Texture2D>("cards");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Space)&& !_previousKeyboardState.IsKeyDown(Keys.Space))
            {
                cardToDesplay = deck.getNewCard();
            }
            _previousKeyboardState = state;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            if (cardToDesplay.isCardGood())
            {
                Rectangle sourceRectangle = new Rectangle((cardToDesplay.value() - 1) * cardWidth, cardToDesplay.shape()*cardHeight, cardWidth, cardHeight);
                Vector2 positionOnScreen = new Vector2(100, 100);

                _spriteBatch.Draw(_cardsTexture, positionOnScreen, sourceRectangle, Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
