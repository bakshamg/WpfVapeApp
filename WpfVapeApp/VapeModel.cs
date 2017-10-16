using System;

namespace WpfVapeApp
{
    public class VapeModel
    {
        #region Private Variables
        private string _deviceName;
        private string _model;
        private int _tankCapacity;
        private Boolean _processed;
        #endregion

        #region Public Properties

        public string DeviceName
        {
            get
            {
                return _deviceName;
            }
            set
            {
                _deviceName = value;
            }
        }
        public string Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
            }
        }
        public int TankCapacity
        {
            get
            {
                return _tankCapacity;
            }
            set
            {
                _tankCapacity = value;
            }
        }
        public Boolean Processed
        {
            get
            {
                return _processed;
            }
            set
            {
                _processed = value;
            }
        }

        #endregion

        #region Public Methods
        public static VapeModel LoadData(string csvLine)
        {
            VapeModel vape = new VapeModel();
            string[] values = csvLine.Split(',');
            vape._deviceName = Convert.ToString(values[0]);
            vape._model = Convert.ToString(values[1]);
            vape._tankCapacity = Convert.ToInt16(values[2]);
            vape._processed = Convert.ToBoolean(values[3]);

            return vape;
        }
        #endregion 

        #region Interfaces

        #endregion

    }
}
