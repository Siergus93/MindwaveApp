using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleApp2
{
    public class Score
    {
        public string playersNickname { get; set; }
        public string trialName { get; set; }
        public int scoreValue { get; set; }

        public Score(string playersNickname, string trialName, int scoreValue)
        {
            this.playersNickname = playersNickname;
            this.trialName = trialName;
            this.scoreValue = scoreValue;
        }

        public Score()
        {

        }
    }
}
