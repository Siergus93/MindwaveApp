using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace MindwaveLib
{
    public class DataCollection : RingArray<Data>
    {
        private const int _max = 300;
        public DataCollection() : base(_max)
        {
        }
        public void Add(Data item)
        {
            base.Add(item);
        }
    }

    public class Data
    {
        public double Value { get; set; }
        public DateTime Date { get; set; }
        public Data(double value, DateTime date)
        {
            this.Date = date;
            this.Value = value;
        }
    }
}
