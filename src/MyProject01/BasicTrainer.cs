using MyProject01.Controller;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MyProject01.NeuroNetwork;
using MyProject01.Controller.Jobs;

namespace MyProject01
{
   
    class EvaluationgContext
    {
        public long Epoch;
        public DateTime StartDate;
        public DateTime CurrentDate;

        public double Fitness;
        public INeuroNetwork Network;
        public EvaluationgContext()
        {
            StartDate = DateTime.Now;
        }

    }
    abstract class BasicTrainer
    {
        public string TestName = "DefaultTest000";

        private long _epoch;

        private ResultEvaluator _evaluator;

        protected long Epoch
        {
            get { return _epoch; }
        }

        public BasicTrainer(ICheckJob checkCtrl)
        {
            _evaluator = new ResultEvaluator(checkCtrl);

        }

        public void RunTestCase()
        {
            LogFile.WriteLine(@"======================");
            LogFile.WriteLine(@"Beginning training...");
            LogFile.WriteLine(@"======================");
            _epoch = 1;

            StartTestCase();

        }

        protected void UpdateTrainInfo(INeuroNetwork network, double fitness = 0.0)
        {
            EvaluationgContext context = new EvaluationgContext();
            context.Epoch = Epoch;
            context.CurrentDate = DateTime.Now;
            context.Fitness = fitness;
            context.Network = network;

            _evaluator.Test(context);

           _epoch++;
        }

        protected abstract void StartTestCase();

  
    }
    class ResultEvaluator
    {
        private List<EvaluationgContext> _contextFifo;
        private Thread _workThread;
        private ICheckJob _mainCheckCtrl;

        public ResultEvaluator(ICheckJob mainCheckCtrl)
        {
            _contextFifo = new List<EvaluationgContext>();
            _mainCheckCtrl = mainCheckCtrl;
            _workThread = new Thread(new ThreadStart(MainTask));
            _workThread.Start();
        }
        public void Test(EvaluationgContext context)
        {
            TrainerContex c = new TrainerContex();
            c.Epoch = context.Epoch;
            c.StartDate = context.StartDate;
            c.BestNetwork = context.Network;
            c.ControllerName = "222";
            c.CurrentDate = context.CurrentDate;
            c.Fitness = context.Fitness;
            _mainCheckCtrl.Do(c);
        }
        public void Stop()
        {

        }

        private void MainTask()
        {

        }
        

    }

}
