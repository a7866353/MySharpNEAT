using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller.Jobs
{
    class TrainerContex
    {
        public long Epoch;
        public DateTime StartDate;
        public DateTime CurrentDate;

        public Trainer Trainer;
        public bool IsEnd;
        public bool IsChanged;

        // Test Data
        public BasicDataBlock _testDataBlock;
        public BasicDataBlock _trainDataBlock;

        // TestCase

        public NEATNetwork BestNetwork;
        public double Fitness;

        public TrainerContex()
        {
            StartDate = DateTime.Now;
        }
        public void SetDataLength(BasicDataBlock dataBlock, int trainLength)
        {
            _testDataBlock = dataBlock;

            // Update test data
            _trainDataBlock = _testDataBlock.GetNewBlock(0, trainLength);

        }

        // Controller
        public string ControllerName;


        public TrainerContex Clone()
        {
            return (TrainerContex)MemberwiseClone();
        }
    }
}
