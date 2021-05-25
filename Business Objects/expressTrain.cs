using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Author: Calder MacLean MAtric No:  40313018
 * This class is the abstract class for train which has all the getters and setters that are used buy the sub classes of Train
 * Modification Date 29/11/18
*/

namespace Business_Objects
{
    [Serializable]
    public class expressTrain : Train
    {
        public override string stops
        {
            get
            {
                return null;
            }
        }
    }
}
