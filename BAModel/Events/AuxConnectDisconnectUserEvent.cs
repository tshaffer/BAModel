using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAModel
{
    public class AuxConnectDisconnectUserEvent : UserEvent
    {
        public enum AudioConnectorType
        {
            BrightSignAuxIn,
            Aux300Audio1,
            Aux300Audio2,
            Aux300Audio3
        }

        public AudioConnectorType AudioConnector { get; set; }

        public AuxConnectDisconnectUserEvent()
        {
            AudioConnector = AuxConnectDisconnectUserEvent.AudioConnectorType.Aux300Audio1;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is AuxConnectDisconnectUserEvent)) return false;

            AuxConnectDisconnectUserEvent auxConnectUserEvent = (AuxConnectDisconnectUserEvent)bsEvent;

            return auxConnectUserEvent.AudioConnector == this.AudioConnector;
        }

    }
}
