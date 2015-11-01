using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace MindwaveLib
{
    public class DataCollection : RingArray<DataValue>
    {
        // Trzeba to rozszerzyć.
        private const int MAX_ELEMENTS = 300;
        public double MaxValue { get; set; }

        public DataCollection() : base(MAX_ELEMENTS)
        {
        }

        public void Add(DataValue item)
        {
            base.Add(item);
            if(MaxValue < item.Value)
            {
                MaxValue = item.Value;
            }
        }


    }

    public class DataValue
    {
        public DateTime Date { get; set; }
        public double Value { get; set;}

        public DataValue(double value, DateTime date)
        {
            this.Date = date;
            this.Value = value;
        }


    }
}
