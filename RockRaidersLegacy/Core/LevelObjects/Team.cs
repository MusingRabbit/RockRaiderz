using System.Collections.Generic;

namespace RockRaiders.Core.LevelObjects
{
    class Team
    {
        private int _teamID;
        private int _score;
        private int _caps;
        private string _teamName;
        private List<SpaceMarine> _playerList;
        
        public int TeamScore
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
            }
        }
        public int Caps
        {
            get
            {
                return _caps;
            }
            set
            {
                _caps = value;
            }
        }
        public string TeamName
        {
            get
            {
                return _teamName;
            }
            set
            {
                _teamName = value;
            }
        }
        public List<SpaceMarine> Members
        {
            get
            {
                return _playerList;
            }
        }
        public int ID
        {
            get
            {
                return _teamID;
            }
        }

        public Team(int ID)
        {
            _teamID = ID;
            _teamName = "";
            _score = 0;
            _caps = 0;
            _playerList = new List<SpaceMarine>();
        }

        public void UpdateRoster()
        {
            for (int i =0; i<_playerList.Count; i++)
            {
                if (_playerList[i].Team != _teamID)
                    _playerList.RemoveAt(i);
            }
        }

        public SpaceMarine GetWinner()
        {
            SpaceMarine winner = null;
            SpaceMarine current = null;

            for (int x = 0; x<_playerList.Count; x++)
            {
                current = _playerList[x];
                if (x == 0)
                    winner = current;

                if (current.Score > winner.Score)
                    winner = current;
            }

            return winner;
        }


        private int SumTeamScore()
        {
            int sum = 0;

            foreach (SpaceMarine player in _playerList)
                sum += player.Score;

            return sum;
        }

        public void AddTeamMember(SpaceMarine Player)
        {
            _playerList.Add(Player);
            Player.Team = _teamID;
        }

        public void UpdateScore(SpaceMarine Player)
        {
            foreach (SpaceMarine player in _playerList)
                if (Player.ID == player.ID)
                    player.Score = Player.Score;
        }
    }
}
