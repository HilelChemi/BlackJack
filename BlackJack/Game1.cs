using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

namespace BlackJack
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _cardsTexture;
        private Texture2D _tableTexture;
        private Texture2D _chipsTexture;
        private SpriteFont _font;
        private int cardWidth = 40;
        private int cardHeight = 60;
        private int chipWidth = 210;
        private int chipHeight = 320;
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
            deck.reasetCards();
            playerTableDeck.reaset();
            playerTableDeck.addCard(deck.getNewCard());
            playerTableDeck.addCard(deck.getNewCard());
            dealer.reaset();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _cardsTexture = Content.Load<Texture2D>("cards");
            _tableTexture = Content.Load<Texture2D>("table");
            _chipsTexture = Content.Load<Texture2D>("chipsNoBack");
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
                    if (dealerCode != -3 || playerTableDeck.getSum() > 21)
                    {
                        dealerCode = -3;
                        deck.reasetCards();
                        playerTableDeck.reaset();
                        playerTableDeck.addCard(deck.getNewCard());
                        playerTableDeck.addCard(deck.getNewCard());
                        dealer.reaset();
                    }
                    else
                    {
                        cardToDesplay = deck.getNewCard();
                        playerTableDeck.addCard(cardToDesplay);
                    }

                }
                else if (state.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                {
                    if (dealerCode != -3|| playerTableDeck.getSum() > 21)
                    {
                        dealerCode = -3;
                        deck.reasetCards();
                        playerTableDeck.reaset();
                        playerTableDeck.addCard(deck.getNewCard());
                        playerTableDeck.addCard(deck.getNewCard());
                        dealer.reaset();
                    }
                    else
                    {
                        dealer.start(playerTableDeck.getSum());
                    }
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
            drowChip(1);
            drowChip(5);
            drowChip(10);
            drowChip(25);
            drowChip(50);
            drowChip(100);
            drowChip(500);
            drowChip(1000);
            drowChip(5000);
            drowChip(10000);


            playerTableDeck.drowDeck(_spriteBatch, _cardsTexture, cardWidth, cardHeight, playersDestinationRectangle);
            dealer.GetTableDeck().drowDeck(_spriteBatch, _cardsTexture, cardWidth, cardHeight, dealersDestinationRectangle);

            int playerSum = playerTableDeck.getSum();
            int dealerSum = dealer.GetTableDeck().getSum();
            int dxOffset = dealerSum < 10 ? 0 : -10;
            int pxOffset = playerSum < 10 ? 0 : -10;

            drowText(playerSum.ToString(), 390+pxOffset, 492);

            if (playerSum > 21)
            {
                drowText("Busted!",340, 300);
            }
            if ((dealerCode!=-3||dealer.IsPlaying()) && dealer.GetTableDeck().GetCards()[1].value()!=14)
            {
                drowText(dealerSum.ToString(), 390 + dxOffset, 50);
            }
            switch (dealerCode)
            {
                case 0:
                    drowText("Split", 350, 250);
                    break;
                case 1:
                    drowText("HaHa Nigga Dealer Wins!", 300, 250);
                    break;
                case -1:
                    drowText("You won!", 340, 250);
                    break;
                case -2:
                    drowText("Busted", 340, 120);
                    drowText("You won!", 340, 250);
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
        private void drowChip(int money)
        {
            Rectangle dest = new Rectangle(50, 450, chipWidth/4, chipHeight/4);
            int spaceBetweenChips = 70;
            int y = 0; int x = 0;
            switch (money)
            {
                case 1:
                    break;
                case 5:
                    x=1;
                    break;
                case 10:
                    x = 2;
                    break;
                case 25:
                    x = 3;
                    break;
                case 50:
                    x = 4;
                    break;
                case 100:
                    y = 1;
                    break;
                case 500:
                    x = 1;
                    y = 1;
                    break;
                case 1000:
                    x = 2;
                    y = 1;
                    break;
                case 5000:
                    x = 3;
                    y = 1;
                    break;
                case 10000:
                    x = 4;
                    y = 1;
                    break;
                default:
                    return;
            }
            dest.X += x* spaceBetweenChips + y*5* spaceBetweenChips;
            Rectangle sourceRectangle = new Rectangle(95 + (chipWidth+18)*x, chipHeight*y, chipWidth, chipHeight);
            if (money == 1)
            {
                sourceRectangle.X -= 8; sourceRectangle.Width += 13;
                dest.Width= (int)(dest.Width*0.9f);
                dest.Height = (int)(dest.Height * 0.9f);
                dest.Y +=  5;

            }
            dest.Y += y*20;
            _spriteBatch.Draw(_chipsTexture, dest, sourceRectangle, Color.White);
        }
}
}
