using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    internal class CardsDeck
    {
        private List<card> cards = new List<card>();
        Random rand = new Random();
        public CardsDeck()
        {
            reasetCards();
        }
        public void reasetCards()
        {
            cards.Clear();
            for (int shape = 0; shape < 4; shape++)
            {
                for (int value = 1; value <= 13; value++)
                {
                    cards.Add(new card(shape,value));
                }
            }
        }
       
        public card getNewCard()
        {
            if (cards.Count() == 0)
                return new card( -1, -1 );
            return getCard();
        }
        private card getCard()
        {
            int index = rand.Next(cards.Count());
            card newCard = cards[index];
            cards.RemoveAt(index);
            return newCard;
        }

    }
}
