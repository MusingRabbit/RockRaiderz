using Microsoft.Xna.Framework.Input;

namespace RockRaiders.UI
{
    public class InputController
    {
        private bool _ButtonUp;
        private bool _ButtonDown;
        private bool _ButtonLeft;
        private bool _ButtonRight;
        private bool _UseKeyPushed;
        private bool _SpaceKeyPushed;
        private bool _WeaponDropPushed;
        private bool _FlagDropPushed;
        private bool _btnOnePushed;
        private bool _btnTwoPushed;

        private KeyboardState _oldKb;
        private KeyboardState _currKb;


        public bool UpPressed
        {
            get
            {
                return _ButtonUp;
            }
            set
            {
                _ButtonUp = value;
            }
        }
        public bool DownPressed
        {
            get
            {
                return _ButtonDown;
            }
            set
            {
                _ButtonDown = value;
            }
        }
        public bool LeftPressed
        {
            get
            {
                return _ButtonLeft;
            }
            set
            {
                _ButtonLeft = value;
            }
        }
        public bool RightPressed
        {
            get
            {
                return _ButtonRight;
            }
            set
            {
                _ButtonRight = value;
            }
        }
        public bool SpacePressed
        {
            get
            {
                return _SpaceKeyPushed;
            }
            set
            {
                _SpaceKeyPushed = value;
            }
        }
        public bool UsePressed
        {
            get
            {
                return _UseKeyPushed;
            }
            set
            {
                _UseKeyPushed = value;
            }
        }
        public bool DropWeapon
        {
            get
            {
                return _WeaponDropPushed;
            }
            set
            {
                _WeaponDropPushed = value;
            }
        }
        public bool DropFlag
        {
            get
            {
                return _FlagDropPushed;
            }
            set
            {
                _FlagDropPushed = value;
            }
        }
        public bool OnePressed
        {
            get
            {
                return _btnOnePushed;
            }
            set
            {
                _btnOnePushed = value;
            }
        }
        public bool TwoPressed
        {
            get
            {
                return _btnTwoPushed;
            }
            set
            {
                _btnTwoPushed = value;
            }
        }


        public InputController()
        {
            resetControls();
        }

        private void resetControls()
        {
            _ButtonUp = false;
            _ButtonLeft = false;
            _ButtonRight = false;
            _ButtonDown = false;
            _SpaceKeyPushed = false;
            _UseKeyPushed = false;
            _WeaponDropPushed = false;
            _FlagDropPushed = false;
            _btnOnePushed = false;
            _btnTwoPushed = false;
        }

        public void ProcessInput(KeyboardState currentKBState)
        {
            resetControls();

            _currKb = currentKBState;
            if (_currKb.IsKeyDown(Keys.W))
                _ButtonUp = true;
            if (_currKb.IsKeyDown(Keys.S))
                _ButtonDown = true;
            if (_currKb.IsKeyDown(Keys.A))
                _ButtonLeft = true;
            if (_currKb.IsKeyDown(Keys.D))
                _ButtonRight = true;
            if (_currKb.IsKeyDown(Keys.Space) && _oldKb.IsKeyUp(Keys.Space))
                _SpaceKeyPushed = true;
            if (_currKb.IsKeyDown(Keys.F) && _oldKb.IsKeyUp(Keys.F))
                _UseKeyPushed = true;
            if (_currKb.IsKeyDown(Keys.G) && _oldKb.IsKeyUp(Keys.G))
                _WeaponDropPushed = true;
            if (_currKb.IsKeyDown(Keys.F) && _oldKb.IsKeyUp(Keys.F) && _currKb.IsKeyDown(Keys.LeftControl))
                _FlagDropPushed = true;
            if (_currKb.IsKeyDown(Keys.D1) && _oldKb.IsKeyUp(Keys.D1))
                _btnOnePushed = true;
            if (_currKb.IsKeyDown(Keys.D2) && _oldKb.IsKeyUp(Keys.D2))
                _btnTwoPushed = true;

            _oldKb = _currKb;
        }
    }
}
