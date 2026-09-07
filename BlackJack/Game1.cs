using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

namespace BlackJack
{
    public class Game1 : Game
    {
        KeyboardState state;
        Stack<int> chipsStack = new Stack<int>();

        Stack<int> prevChipsStack = new Stack<int>();
        private Rectangle rebetButtonRect = new Rectangle(620, 380, 150, 50);
        private Rectangle allInButtonRect = new Rectangle(300, 380, 150, 50);
        private Rectangle reasetButtonRect = new Rectangle(280, 230, 250, 100);
        private Texture2D _pixelTexture;

        List<Rectangle> chipsToDespalyHitboxes = new List<Rectangle>();
        private bool isMakingABet = true;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _cardsTexture;
        private Texture2D _tableTexture;
        private Texture2D _chipsTexture;
        private SpriteFont _font;
        private int cardWidth = 40;
        private int cardHeight = 60;
        private int chipWidth = 210;
        private int chipHeight = 210;
        private int cardDisWidth = 60;
        private int cardDisHeight = 90;
        private int[] chipsVals = {1,5,10,25,50,100,500,1000,5000,10000};
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
        private int initialMoney = 2000;
        private int playersMoney = 0;
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
            playersMoney = initialMoney;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _cardsTexture = Content.Load<Texture2D>("cards");
            _tableTexture = Content.Load<Texture2D>("table");
            _chipsTexture = Content.Load<Texture2D>("chipsNoBack");

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            _font = Content.Load<SpriteFont>("ScoreFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            updatePlayKeys((float)gameTime.ElapsedGameTime.TotalSeconds);
            _previousKeyboardState = state;
            state = Keyboard.GetState();
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

            if(isMakingABet)
                makeABet();
            else
                drowPlayingThings(0);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void drowText(string text, int x, int y)
        {
            _spriteBatch.DrawString(_font, text, new Vector2(x + 2, y + 2), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(x, y), Color.Gold);
        }
        private Rectangle drowChip(int money,bool isStack)
        {
            Rectangle dest = new Rectangle(60, 500, chipWidth/4, chipHeight/4);
            int spaceBetweenChips = 70;
            int y = 0; 
            int x = 0;
            for(int i=1;i<chipsVals.Length;i++) 
            {
                if (money == chipsVals[i])
                {
                    x = i;
                    if (i > 4)
                    {
                        y = 1;
                        x -= 5;
                    }
                    break;

                }              
            }
            if (isStack)
                dest.Y = 300;
            else
                dest.X += x * spaceBetweenChips + y * 5 * spaceBetweenChips;
            Rectangle sourceRectangle = new Rectangle(95 + (chipWidth+18)*x, chipHeight*y+100+y*35, chipWidth, chipHeight);
            if (money == 1)
            {
                sourceRectangle.X -= 8; 
                sourceRectangle.Width += 13;
                sourceRectangle.Height += 10;
            }
            _spriteBatch.Draw(_chipsTexture, dest, sourceRectangle, Color.White);
            
            return dest;
        }
        private void makeABet()
        {
            float targetWidth = 800f;
            float targetHeight = 600f;
            float windowWidth = GraphicsDevice.Viewport.Width;
            float windowHeight = GraphicsDevice.Viewport.Height;
            float scaleX = targetWidth / windowWidth;
            float scaleY = targetHeight / windowHeight;
            Point pt = new Point
                (
                (int)(_currentMouseState.X * scaleX),
                (int)(_currentMouseState.Y * scaleY)
            );
            chipsToDespalyHitboxes.Clear();
            if(prevChipsStack.Count > 0&&prevChipsStack.Sum()<=playersMoney) 
                DrawButton("REBET",rebetButtonRect,pt);
            if (playersMoney > 0)
                DrawButton("ALL IN", allInButtonRect, pt);
            else if(chipsStack.Count ==0)
                DrawButton("REASET", reasetButtonRect, pt);

                Rectangle chipsStackHitBox = new Rectangle();
            if (chipsStack.Count > 0)
                chipsStackHitBox= drowChip(chipsStack.Peek(), true);

            if (state.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter)&&chipsStack.Sum()>0)
            {
                reasetPlayingStats();//start playing
                _previousKeyboardState = state;
                return;
            }
            else
            { 
                for (int i = 0; i < chipsVals.Length; i++)
                {
                    if (chipsVals[i] <= playersMoney)
                        chipsToDespalyHitboxes.Add(drowChip(chipsVals[i], false));
                    else
                        break;
                }
                if (_currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed)//press
                {

                    if (rebetButtonRect.Contains(pt) && prevChipsStack.Sum() <= playersMoney)
                    {
                        playersMoney += chipsStack.Sum();
                        chipsStack = new Stack<int>(prevChipsStack.ToArray().Reverse());
                        playersMoney -= chipsStack.Sum();
                    }
                    else if (playersMoney + chipsStack.Sum() == 0 && reasetButtonRect.Contains(pt))
                        playersMoney = initialMoney;
                    else if (allInButtonRect.Contains(pt))
                    {
                        allIn();
                    }
                    else if (!chipsStackHitBox.IsEmpty && chipsStackHitBox.Contains(pt))
                        playersMoney += chipsStack.Pop();
                    else
                        for (int i = 0; i < chipsToDespalyHitboxes.Count; i++)
                        {

                            if (chipsToDespalyHitboxes[i].Contains(pt))//press at a chip
                            {
                                chipsStack.Push(chipsVals[i]);//serial chip num
                                playersMoney -= chipsVals[i];
                                break;
                            }
                        }

                }
            }
            int stackSum = chipsStack.Sum();
            if (stackSum > 0)
                drowText(stackSum.ToString()+"$", 40, 200);
            if(playersMoney > 0)
                drowText(playersMoney.ToString() + "$", 40, 450);
            else if(stackSum ==0)
                drowText("You are out of money!",reasetButtonRect.X-50,reasetButtonRect.Y-100);
        }
        private void drowPlayingThings(int betMoney)
        {
            playerTableDeck.drowDeck(_spriteBatch, _cardsTexture, cardWidth, cardHeight, playersDestinationRectangle);
            dealer.GetTableDeck().drowDeck(_spriteBatch, _cardsTexture, cardWidth, cardHeight, dealersDestinationRectangle);

            int playerSum = playerTableDeck.getSum();
            int dealerSum = dealer.GetTableDeck().getSum();
            int dxOffset = dealerSum < 10 ? 0 : -10;
            int pxOffset = playerSum < 10 ? 0 : -10;

            drowText(playerSum.ToString(), 390 + pxOffset, 492);

            if (playerSum > 21)
            {
                drowText("Busted!", 340, 300);
            }
            if ((dealerCode != -3 || dealer.IsPlaying()) && dealer.GetTableDeck().GetCards()[1].value() != 14)
            {
                drowText(dealerSum.ToString(), 390 + dxOffset, 50);
            }
            switch (dealerCode)
            {
                case 0:
                    drowText("Split", 350, 250);
                    break;
                case 1:
                    drowText("HaHa Nigga Dealer Wins!", 200, 250);
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

        }
        private void updatePlayKeys(float dt)
        {
            if (isMakingABet)
            {
                return;
            }
            if (!dealer.IsPlaying())
            {
                if (state.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
                {
                    if (updateMoney())//reaset
                    {
                        isMakingABet = true;
                        _previousKeyboardState = state;
                        return;
                    }
                    else
                    {
                        cardToDesplay = deck.getNewCard();
                        playerTableDeck.addCard(cardToDesplay);
                    }

                }
                else if (state.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                {
                    if (updateMoney())//reaset
                    {
                        isMakingABet = true;
                        _previousKeyboardState = state;
                        return;
                    }
                    else
                    {
                        dealer.start(playerTableDeck.getSum());
                    }
                }
            }
            else
            {
                dealerCode = dealer.update(dt);
            }
        }
        private void reasetPlayingStats()
        {
            dealerCode = -3;
            deck.reasetCards();
            playerTableDeck.reaset();
            playerTableDeck.addCard(deck.getNewCard());
            playerTableDeck.addCard(deck.getNewCard());
            dealer.reaset();
            isMakingABet = false;
        }
        private bool updateMoney()//isRoundOver
        {
            if (playerTableDeck.getSum() > 21)
            {
                prevChipsStack = new Stack<int>(chipsStack.ToArray().Reverse()); 
                chipsStack.Clear();
                return true;
            }
            if (dealerCode == -3)
                return false;

            int chipsStackSum =chipsStack.Sum();
            prevChipsStack = new Stack<int>(chipsStack.ToArray().Reverse()); 
            chipsStack.Clear();

            
         
            switch (dealerCode)
                {
                    case 0:
                        playersMoney += chipsStackSum;
                        break;
                    case 1:
                        break;
                    case -1:
                        playersMoney += chipsStackSum * 2;
                        break;
                    case -2:
                         playersMoney += chipsStackSum * 2;
                        break;
                    default:
                        return false;
                }
            return true;

        }
        private void DrawButton(string text, Rectangle buttonRect, Point mousePt)
        {
            Vector2 baseTextSize = _font.MeasureString(text);

            int paddingX = 10;
            int paddingY = 5;
            float maxTextWidth = buttonRect.Width - (paddingX * 2);
            float maxTextHeight = buttonRect.Height - (paddingY * 2);

            float textScaleX = maxTextWidth / baseTextSize.X;
            float textScaleY = maxTextHeight / baseTextSize.Y;
            float textScale = Math.Min(textScaleX, textScaleY); 

            Vector2 scaledTextSize = baseTextSize * textScale;

            bool isHovered = buttonRect.Contains(mousePt);
            Color bgColor = isHovered ? new Color(40, 140, 60) : new Color(20, 80, 35);
            Color borderColor = isHovered ? Color.Gold : Color.DarkGoldenrod;

            _spriteBatch.Draw(_pixelTexture, buttonRect, bgColor);

            int borderWidth = 2;
            _spriteBatch.Draw(_pixelTexture, new Rectangle(buttonRect.X, buttonRect.Y, buttonRect.Width, borderWidth), borderColor);
            _spriteBatch.Draw(_pixelTexture, new Rectangle(buttonRect.X, buttonRect.Bottom - borderWidth, buttonRect.Width, borderWidth), borderColor);
            _spriteBatch.Draw(_pixelTexture, new Rectangle(buttonRect.X, buttonRect.Y, borderWidth, buttonRect.Height), borderColor);
            _spriteBatch.Draw(_pixelTexture, new Rectangle(buttonRect.Right - borderWidth, buttonRect.Y, borderWidth, buttonRect.Height), borderColor);

            Vector2 textPos = new Vector2(
                buttonRect.X + (buttonRect.Width - scaledTextSize.X) / 2,
                buttonRect.Y + (buttonRect.Height - scaledTextSize.Y) / 2
            );

            _spriteBatch.DrawString(_font, text, textPos + new Vector2(1, 1), Color.Black, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(_font, text, textPos, isHovered ? Color.Yellow : Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
        }
        private void allIn()
        {
            for (int i = chipsVals.Count()-1; i >=0; i--)
            {
                int heighestChipVal = chipsVals[i];
                while (playersMoney >= heighestChipVal && playersMoney != 0)
                {
                    playersMoney -= heighestChipVal;
                    chipsStack.Push(heighestChipVal);
                }
            }
        }
    }
}
