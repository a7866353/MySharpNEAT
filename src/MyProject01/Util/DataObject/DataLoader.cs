using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MongoDB.Driver;
using MongoDB.Bson;
using MyProject01.DAO;

namespace MyProject01.Util
{
  
    public class RateSet
    {
        public DateTime Time;
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public long TickVolume;
        public long RealVolume;

        public RateSet()
        { 
        }
        public RateSet(RateData data)
        {
            Open = data.open;
            Close = data.close;
            High = data.high;
            Low = data.low;
            RealVolume = data.real_volume;
            TickVolume = data.tick_volume;
            Time = data.time;
        }

        public void SetValue(double v)
        {
            Open = Close = High = Low = v;
        }
        public override string ToString()
        {
            return Time.ToShortDateString() + " " + Time.ToShortTimeString() + ": " + Close.ToString();
        }
        public RateSet Clone()
        {
            return (RateSet)MemberwiseClone();
        }
    }
    public abstract class DataLoader : List<RateSet>
    {
        public double[] GetArr(DateTime date)
        {
            int index = SerachByDate(date);
            if (index >= 0)
            {
                double[] valueArr = new double[1];
                valueArr[0] = this[index].Close;
                return valueArr;
            }
            else
                return null;

        }
        public double[] GetArr(DateTime startDate, int days)
        {
            // return GetArr(startDate, startDate.AddDays(days));
            return null;
        }
        public double[] GetArr(DateTime startDate, DateTime endDate)
        {
            return null;
            /*
            if (endDate < startDate)
                return null;

            List<double> valueList = new List<double>();
            int index = 0;
            while (startDate < endDate)
            {
                index = SerachByDate(startDate);
                if (index >= 0)
                {
                    valueList.Add(this[index].Value);
                }
                startDate = startDate.AddDays(1);
            }
            return valueList.ToArray();
            */
        }


        public double[] GetArr(int startIndex, int length)
        {

            if ((startIndex + length) > this.Count)
                //     length = this.Count - startIndex;
                return null;
            double[] res = new double[length];
            for (int i = 0; i < length; i++)
            {
                res[i] = this[startIndex + i].Close;
            }
            return res;
        }

        public RateDataBlock CreateDataBlock(int startIndex, int length, int blockLength)
        {
            return new RateDataBlock(this, startIndex, length, blockLength);
        }

        public void Fillter(DateTime startTime, DateTime endTime)
        {
            int startIndex = -1;
            int stopIndex = -1;

            for(int idx=0;idx<this.Count;idx++)
            {
                if(startIndex == -1)
                {
                    if (this[idx].Time >= startTime)
                        startIndex = idx;
                }
                if(stopIndex == -1)
                {
                    if (this[idx].Time > endTime)
                        stopIndex = idx;
                }
                if(stopIndex != -1 && startIndex != -1)
                    break;
            }

            
            if( stopIndex != -1)
            {
                // Remove after
                this.RemoveRange(stopIndex, this.Count - stopIndex);
            }
            if( startIndex != -1)
            {
                // Remove before
                this.RemoveRange(0, startIndex);
            }

        }

        protected void SortByDate()
        {
            Sort(new DateComparer(true));
        }


        private int SerachByDate(DateTime date)
        {
            return BinarySearch(new RateSet() { Time = date }, new DateComparer(true));
        }
    }
    // Compare By Date
    class DateComparer : IComparer<RateSet>  
    {
        private bool _isIncr = false;

        public DateComparer(bool isIncr)
        {
            _isIncr = isIncr;
        }
        public int Compare(RateSet x, RateSet y)
        {
            if (_isIncr)
            {
                if (x.Time > y.Time)
                    return 1;
                else if (x.Time == y.Time)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (x.Time < y.Time)
                    return 1;
                else if (x.Time == y.Time)
                    return 0;
                else
                    return -1;
            }

        }
    }

}
