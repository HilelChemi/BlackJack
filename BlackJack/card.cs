using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    internal class card
    {
        private int _shape, _value;
        public card(int shape, int value)
        {
            _shape = shape;
            _value = value;
        }
        public int shape()
        {
            return _shape;
        }
        public int value()
        {
            return _value;
        }
        public bool isCardGood()
        {
            return _shape >=0 && _value >0&& _shape <= 3 && _value <= 13;
        }

    }
}
