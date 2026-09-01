using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    internal class dealer
    {
        private float _delayTimer = 0f; 
        private bool isPlaying = false;
        private int scoreToBeat = 0;
        private tableDeck tableDeck = new tableDeck();
        private card upsideDownCard;
        private CardsDeck cardsDeck;
        public dealer(CardsDeck cardsDeck)
        {
            this.cardsDeck = cardsDeck;//ref to original deck
            reaset();
        }
        public void reaset()
        {
            tableDeck.reaset();
            tableDeck.addCard(cardsDeck.getNewCard());
            upsideDownCard = cardsDeck.getNewCard();
            tableDeck.addCard(new card(3, 14));//updiseDown texture
        }
        public void start(int playersScore)
        {
            isPlaying = true;
            scoreToBeat = playersScore;
        }
        public int update(float dt)
        {
            if (isPlaying)
            {
                if(_delayTimer<=0f)
                    setTimer(1f);
                if (_delayTimer > 0f)
                {
                    _delayTimer -= dt;
                }
                if (_delayTimer <= 0f)
                {
                    if (tableDeck.GetCards()[1].value() == 14)//upsideDown texture card
                    {
                        tableDeck.removeFirst();
                        tableDeck.addCard(upsideDownCard);
                    }
                    else if (tableDeck.getSum() < 17)
                    {
                        tableDeck.addCard(cardsDeck.getNewCard());
                    }
                    else
                    {
                        isPlaying = false;
                        int sum = tableDeck.getSum();
                        if (sum > 21)
                            return -2;//busted
                        else if (sum > scoreToBeat)
                            return 1;//win
                        else if (sum < scoreToBeat)
                            return -1;//losw
                        else
                            return 0;//split       
                    }
                }
            }
            return -3;
        }
        private void setTimer(float t)
        {
            _delayTimer = t;
        }
        public tableDeck GetTableDeck()
        {
            return tableDeck;
        }
        public bool IsPlaying()
        {
            return isPlaying;
        }
    }
}
