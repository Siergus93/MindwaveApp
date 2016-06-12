using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleApp2
{
    public class Score
    {
        public string playersNickname;
        public string trialName;
        public int scoreValue;

        public Score(string playersNickname, string trialName, int scoreValue)
        {
            this.playersNickname = playersNickname;
            this.trialName = trialName;
            this.scoreValue = scoreValue;
        }
    }
}
