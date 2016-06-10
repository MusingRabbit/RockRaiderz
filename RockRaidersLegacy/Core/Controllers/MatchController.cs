namespace RockRaiders.Core.Controllers
{
    enum MatchType
    {
        DM,
        TDM,
        CTF
    }

    class MatchController
    {
        private MatchType _currMatch;
        private SpaceMarine _winner;
        private Team _teamBlue;
        private Team _teamRed;
        private Team _teamNull;
        private Team _winningTeam;
        private bool _endMatch;

        private int _scoreLimit = 1;
        private int _capLimit = 5;

        public MatchType Match
        {
            get
            {
                return _currMatch;
            }
            set
            {
                _currMatch = value;
            }
        }
        public Team RedTeam
        {
            get
            {
                return _teamRed;
            }
        }
        public Team BlueTeam
        {
            get
            {
                return _teamBlue;
            }
        }
        public Team NoTeam
        {
            get
            {
                return _teamNull;
            }
        }
        public bool MatchDone
        {
            get
            {
                return _endMatch;
            }
        }
        public Team WinningTeam
        {
            get
            {
                return _winningTeam;
            }
        }
        public int ScoreLimit
        {
            get
            {
                return _scoreLimit;
            }
        }
        public int CapLimit
        {
            get
            { return _capLimit; }
        }

        public MatchController()
        {
            _teamNull = new Team(0);
            _teamBlue = new Team(1);
            _teamRed = new Team(2);
            _currMatch = MatchType.DM;
            _endMatch = false;
            _winner = null;
        }

        public void updateMe(World gameWorld)
        {
            switch (_currMatch)
            {
                case MatchType.DM:
                    if (_teamNull.GetWinner() != null)
                    {
                        SpaceMarine player = _teamNull.GetWinner();
                        if (player.Score >= _scoreLimit)
                        {
                            _winner = player;
                            _endMatch = true;
                        }
                    }
                    break;

                case MatchType.TDM:
                    _scoreLimit = 40;
                    if (_teamBlue.TeamScore >= _scoreLimit)
                        _winningTeam = _teamBlue;
                    else if (_teamRed.TeamScore >= _scoreLimit)
                        _winningTeam = _teamRed;

                    if (_winningTeam != null)
                        _endMatch= true;

                    break;

                case MatchType.CTF:

                    if (gameWorld.BlueBase.MadeGoal)
                    {
                        gameWorld.BlueBase.MadeGoal = false;
                        _teamBlue.Caps++;
                    }
                    if (gameWorld.RedBase.MadeGoal)
                    {
                        gameWorld.RedBase.MadeGoal = false;
                        _teamRed.Caps++;
                    }

                    if (_teamBlue.Caps >= _capLimit)
                        _winningTeam = _teamBlue;
                    else if (_teamRed.Caps >= _capLimit)
                        _winningTeam = _teamRed;

                    if (_winningTeam != null)
                        _endMatch = true;

                    break;
            }

            _teamBlue.UpdateRoster();
            _teamNull.UpdateRoster();
            _teamRed.UpdateRoster();
        }
    }
}
