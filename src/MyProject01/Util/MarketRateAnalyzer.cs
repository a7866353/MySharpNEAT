using MyProject01.Util.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace MyProject01.Util
{
    public class DealPointInfomation
    {
        public enum DealTypes
        {
            Buy, Sell
        };
        public int Index;
        public DealTypes Type;
        public double AmountOfChange;
        public double Price;

        public override string ToString()
        {
            
            return Index.ToString() + ":" + Type.ToString();
        }
    }
    public class MarketRateAnalyzer
    {
        private RateSet[] _rateSetArr;
        private double[] _sourceDataArr;
        private double[] aveDataArr;
        

        private delegate List<DealPointInfomation> DealPointFunction(List<DealPointInfomation> infoArr);
        // Debug
        private GraphLine[] _graphLineArr;
        
        public MarketRateAnalyzer(RateSet[] rateSetArr)
        {
            _rateSetArr = rateSetArr;
            _sourceDataArr = new double[rateSetArr.Length];
            for (int i = 0; i < rateSetArr.Length; i++)
            {
                _sourceDataArr[i] = rateSetArr[i].Close;
            }

            _graphLineArr = new GraphLine[4];
        }

        public DealPointInfomation[] GetDealInfo()
        {
            List<DealPointInfomation> DealPointList = new List<DealPointInfomation>();
            List<DealPointFunction> dealFuncs = new List<DealPointFunction>();
            aveDataArr = FilterAve(_sourceDataArr);

            // Deal function filler
            dealFuncs.Add(FindAroundMaximin);
            dealFuncs.Add(ThredHoldFilter);
            // Debug
            _graphLineArr[0] = GraphViewer.Instance.AddLineData(_sourceDataArr);
            _graphLineArr[1] = GraphViewer.Instance.AddLineData(aveDataArr);



            List<DealPointInfomation> infoList = ConvertToDealPoints(aveDataArr);
            for (int i = 0; i < dealFuncs.Count; i++)
            {
                infoList = dealFuncs[i](infoList);
            }

            // Debug
            DealPointInfomation[] infoArr = infoList.ToArray();
            foreach( DealPointInfomation info in infoList)
            {
                if(info.Type == DealPointInfomation.DealTypes.Buy)
                    _graphLineArr[0].AddMark(info.Index, Brushes.Red);
                else
                    _graphLineArr[0].AddMark(info.Index, Brushes.Green);
            }
            _graphLineArr[0].Update();
            return infoList.ToArray();
        }

        /// <summary>
        ///  Find change point in double[]
        /// </summary>
        /// <param name="dataArr"></param>
        /// <returns></returns>
        private List<DealPointInfomation> ConvertToDealPoints(double[] dataArr)
        {
            List<DealPointInfomation> infoList = new List<DealPointInfomation>();
            // current increasing direction
            double delt = 0;
            // previous increasing direction
            double preDelt = 1;


            for (int i = 0; i < dataArr.Length - 1; i++)
            {
                delt = dataArr[i + 1] - dataArr[i];
                if ((delt * preDelt) < 0)
                {
                    // Change point.
                    DealPointInfomation info = new DealPointInfomation();
                    info.Index = i;
                    if (delt > 0)
                        info.Type = DealPointInfomation.DealTypes.Buy;
                    else
                        info.Type = DealPointInfomation.DealTypes.Sell;
                    info.Price = dataArr[i];
                    infoList.Add(info);
                    preDelt = delt;
                }
            }
            return infoList;
        }

        /// <summary>
        /// Find Maximin value around point
        /// fix point after ave.
        /// </summary>
        /// <param name="inList"></param>
        /// <returns></returns>
        private List<DealPointInfomation> FindAroundMaximin(List<DealPointInfomation> inList)
        {
            // Parms
            int searchRaio = 1;

            List<DealPointInfomation> resList = new List<DealPointInfomation>();
            foreach (DealPointInfomation info in inList)
            {
                int index = info.Index;
                int startIndex = index - searchRaio;
                int endIndex = index + searchRaio;
                int targIndex;
                if (startIndex < 0)
                    startIndex = 0;
                if (endIndex > _sourceDataArr.Length - 1)
                    endIndex = _sourceDataArr.Length - 1;

                if (endIndex - startIndex > 0)
                {
                    targIndex = startIndex;
                    for (int j = startIndex; j <= endIndex; j++)
                    {
                        if ((info.Type == DealPointInfomation.DealTypes.Buy) &&
                            (_sourceDataArr[targIndex] > _sourceDataArr[j]))
                        {
                            targIndex = j;
                        }
                        else if ((info.Type == DealPointInfomation.DealTypes.Sell) &&
                            (_sourceDataArr[targIndex] < _sourceDataArr[j]))
                        {
                            targIndex = j;
                        }
                    }
                    info.Index = targIndex;
                }
                resList.Add(info);
            }

            return resList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inList"></param>
        /// <returns></returns>
        private List<DealPointInfomation> ThredHoldFilter(List<DealPointInfomation> inList)
        {
            // Parms
            double rateThrehold = 0.3; // for 30%

            List<DealPointInfomation> resList = new List<DealPointInfomation>();
            for (int i = 0; i < inList.Count - 1; i++)
            {
                // Check next item
                double change = Math.Abs(_sourceDataArr[inList[i+1].Index] - _sourceDataArr[inList[i].Index]) / _sourceDataArr[inList[i].Index];
                if (change > rateThrehold)
                    continue;

                inList.RemoveAt(i + 1);
                if( i > inList.Count-1)
                    continue;

                // Check Same Item
                if (inList[i].Type == DealPointInfomation.DealTypes.Buy)
                {
                    if (_sourceDataArr[inList[i].Index] > _sourceDataArr[inList[i + 1].Index])
                    {
                        inList.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        inList.RemoveAt(i + 1);
                    }
                }
                else
                {
                    if (_sourceDataArr[inList[i].Index] > _sourceDataArr[inList[i + 1].Index])
                    {
                        inList.RemoveAt(i + 1);
                    }
                    else
                    {
                        inList.RemoveAt(i);
                        i--;
                    }
                }

            }
            return inList;
        }

        private double[] FilterAve(double[] dataArr)
        {
            if (dataArr.Length < 3)
                return (double[])dataArr.Clone();
            double[] resArr = new double[dataArr.Length];
            resArr[0] = dataArr[0];
            resArr[dataArr.Length - 1] = dataArr[dataArr.Length - 1];
            for (int i = 1; i < dataArr.Length - 1; i++)
            {
                resArr[i] = (dataArr[i - 1] + dataArr[i] + dataArr[i + 1]) / 3;
            }
            return resArr;

        }

    }
}
