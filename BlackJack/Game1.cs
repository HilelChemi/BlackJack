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
        private dealer dealer;
        card cardToDesplay = new card(-1, -1);
        private KeyboardState _previousKeyboardState;
        private Rectangle playersDestinationRectangle;
        private Rectangle dealersDestinationRectangle;
        private int dealerCode = -3;
        private bool displaymsg = false;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.AllowUserResizing = true;
            _graphics.HardwareModeSwitch = false;
            _graphics.SynchronizeWithVerticalRetrace = true;
            dealer = new dealer(deck);
            playersDestinationRectangle = new Rectangle(370, 390, cardDisWidth, cardDisHeight);
            dealersDestinationRectangle = new Rectangle(370, 100, cardDisWidth, cardDisHeight);
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

            if (!dealer.IsPlaying())
            {
                if (state.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
                {
                    if (playerTableDeck.getSum() > 21)
                    {
                        playerTableDeck.reaset();
                        deck.reasetCards();

                    }
                    cardToDesplay = deck.getNewCard();
                    playerTableDeck.addCard(cardToDesplay);

                }
                else if (state.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                {
                    dealer.start(playerTableDeck.getSum());
                }
            }
            else
            {
                dealerCode = dealer.update((float)gameTime.ElapsedGameTime.TotalSeconds);
                

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


            playerTableDeck.drowDeck(_spriteBatch, _cardsTexture, cardWidth, cardHeight, playersDestinationRectangle);
            dealer.GetTableDeck().drowDeck(_spriteBatch, _cardsTexture, cardWidth, cardHeight, dealersDestinationRectangle);

            int currentSum = playerTableDeck.getSum();
            string scoreText = $"Score: {currentSum}";
            int xOffset = currentSum < 10 ? 0 : -10;

            _spriteBatch.DrawString(_font, $"{currentSum}", new Vector2(392 + xOffset, 492), Color.Black);
            _spriteBatch.DrawString(_font, $"{currentSum}", new Vector2(390 + xOffset, 490), Color.Gold);

            if (currentSum > 21)
            {
                drowText("Busted!",340, 300);
            }
            switch (dealerCode)
            {
                case 0:
                    drowText("Split", 340, 200);
                    break;
                case 1:
                    drowText("Lose", 340, 200);
                    break;
                case -1:
                    drowText("Win!", 340, 200);
                    break;
                case -2:
                    drowText("Busted", 340, 100);
                    drowText("Win!", 340, 200);
                    break;
                default:
                    break;
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void drowText(string text, int x, int y)
        {
            _spriteBatch.DrawString(_font, text, new Vector2(x+2, y+2), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(x, y), Color.Gold);
        }
}
}
