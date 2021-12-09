using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirlineInvoice.Models
{
    public class ChooseRoomModel
    {
        public ChooseRoomModel()
        { 
        
        }
        private List<AgentBranch> _Rooms;
        private List<Agent> _Hotels;

        public List<AgentBranch> Rooms
        {
            get
            {
                if (_Rooms == null)
                {
                    _Rooms = new List<AgentBranch>();
                }
                return _Rooms;
            }
            set
            {
                _Rooms = value;
            }
        }
        public List<Agent> Hotels
        {
            get
            {
                if (_Hotels == null)
                {
                    _Hotels = new List<Agent>();
                }
                return _Hotels;
            }
            set
            {
                _Hotels = value;
            }
        }
    }
}