using System;

namespace center
{
    class hub_msg_handle
    {
        public hub_msg_handle(svrmanager _svrmanager_, hubmanager _hubmanager_)
        {
            _svrmanager = _svrmanager_;
            _hubmanager = _hubmanager_;
        }

        public void closed()
        {
            _hubmanager.hub_closed(juggle.Imodule.current_ch);
            if (_hubmanager.checkAllHubClosed())
            {
                _svrmanager.close_db();
                center.closeHandle.is_close = true;
            }
        }

        private svrmanager _svrmanager;
        private hubmanager _hubmanager;
    }
}
