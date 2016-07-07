using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    class BasicLearnCaseFactory : IDescriptionProvider
    {
        public IController Create()
        {
            SensorGroup senGroup = new SensorGroup();
            senGroup.Add(new RateSensor(16));

            BasicActor actor = new BasicActor();

            BasicController ctrl = new BasicController(senGroup, actor);
            return ctrl;
        }


        public string Description
        {
            get { throw new NotImplementedException(); }
        }
    }
}
