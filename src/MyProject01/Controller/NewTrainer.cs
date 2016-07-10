using MyProject01.Controller.Jobs;
using MyProject01.Factorys.PopulationFactorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
 
    class NewTrainer : Trainer
    {
        private int _inVecLen;
        private int _outVecLen;
        private TrainerContex _context;
        public BasicPopulationFactory PopulationFacotry;
        public ICalculateScore ScoreCtrl;
        public NewTrainer(int inVectorLength, int outVectorLength)
        {
            _inVecLen = inVectorLength;
            _outVecLen = outVectorLength;
        }

        protected override void PostItration()
        {
            _context.Epoch = Epoch;
            _context.CurrentDate = DateTime.Now;
            CheckCtrl.Do(_context);
        }

        protected override TrainEA CreateTrainEA()
        {
            _context = new TrainerContex();
            _context.Trainer = this;
             
            TrainEA train = NEATUtil.ConstructNEATTrainer(
                PopulationFacotry.Get(_inVecLen, _outVecLen),
                ScoreCtrl);
            _context.trainEA = train;
            return train;
        }
    }
}
