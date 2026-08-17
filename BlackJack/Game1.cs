using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace BlackJack
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _cardsTexture;
        private Texture2D _tableTexture;
        private SpriteFont _font;
        private int cardWidth = 40;
        private int cardHeight = 60;
        private int cardDisWidth = 60;
        private int cardDisHeight = 90;
        private readonly int _virtualWidth = 800;
        private readonly int _virtualHeight = 600;
        private CardsDeck deck = new CardsDeck();
        private tableDeck playerTableDeck = new tableDeck();
        card cardToDesplay = new card(-1,-1);
        private KeyboardState _previousKeyboardState;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.AllowUserResizing = true;
            _graphics.HardwareModeSwitch = false;
            _graphics.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _cardsTexture = Content.Load<Texture2D>("cards");
            _tableTexture = Content.Load<Texture2D>("table");
            _font = Content.Load<SpriteFont>("ScoreFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Space)&& !_previousKeyboardState.IsKeyDown(Keys.Space))
            {
                cardToDesplay = deck.getNewCard();
                playerTableDeck.addCard(cardToDesplay);
            }
            _previousKeyboardState = state;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            float scaleX = (float)GraphicsDevice.Viewport.Width / _virtualWidth;
            float scaleY = (float)GraphicsDevice.Viewport.Height / _virtualHeight;

            Matrix transformMatrix = Matrix.CreateScale(scaleX, scaleY, 1.0f);

            _spriteBatch.Begin(transformMatrix: transformMatrix);
            Rectangle baseScreenRectangle = new Rectangle(0, 0, _virtualWidth, _virtualHeight);
            _spriteBatch.Draw(_tableTexture, baseScreenRectangle, Color.White);
            if (cardToDesplay.isCardGood())
            {
                Rectangle sourceRectangle = new Rectangle((cardToDesplay.value() - 1) * cardWidth, cardToDesplay.shape()*cardHeight, cardWidth, cardHeight);
                Rectangle destinationRectangle = new Rectangle(370, 390, cardDisWidth, cardDisHeight);

                _spriteBatch.Draw(_cardsTexture, destinationRectangle, sourceRectangle, Color.White);
            }
            int currentSum = playerTableDeck.getSum();
            string scoreText = $"Score: {currentSum}";
            int xOffset = currentSum < 10 ? 0 : -10;

            _spriteBatch.DrawString(_font, $"{currentSum}",  new Vector2(392+ xOffset, 492), Color.Black);

            // ציור הטקסט הראשי מעל הצל
            _spriteBatch.DrawString(_font, $"{currentSum}", new Vector2(390+ xOffset, 490), Color.Gold); _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
