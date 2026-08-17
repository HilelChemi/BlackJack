using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    internal class tableDeck
    {
        private List<card> cards = new List<card>();
        public tableDeck()
        {
            cards.Clear();
        }
        public void addCard(card newCard)
        {
            cards.Add(newCard);
        }
        public void reaset()
        {
            cards.Clear();
        }
        public List<card> GetCards()
        {
            return cards;
        }
        public int getSum()
        {
            int AcesCount = 0;//for lowering the score
            int sum = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                sum += getVal(cards[i].value());
                if (cards[i].value() == 1)
                    AcesCount++;
            }
            for (; sum>21&&AcesCount > 0; AcesCount--)
            {
                    sum -= 10;
            }
            return sum;
        }
        private int getVal(int num)
        {
            if (num > 10)
                return 10;
            else if (num == 1)
                return 11;
            return num;
        }
    }
}
