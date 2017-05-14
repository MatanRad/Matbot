using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Matbot
{
    [Serializable]
    public enum UserRank
    {
        User = 0,
        Admin = 1,
        Owner = 2
    }

    [Serializable]
    public class BotUser
    {

        public bool exceptionRegister = false;

        public int ID
        {
            get;
             set;
        }
        public UserRank Rank
        {
            get;
             set;
        }
        
        public Queue<Command> OperationQueue = new Queue<Command>();

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ShouldSerializeOperationQueues() { return false; }

        public void ChangeID(int ID)
        {
            this.ID = ID;
        }

        public void ChangeRank(UserRank Rank)
        {
            this.Rank = Rank;
        }

    }
}
