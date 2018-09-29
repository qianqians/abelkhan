using System;

namespace center
{
    class hub_msg_handle
    {
        public hub_msg_handle(svrmanager _svrmanager_, hubmanager _hubmanager_, clutter _clutter_)
        {
            _svrmanager = _svrmanager_;
            _hubmanager = _hubmanager_;
            _clutter = _clutter_;
        }

        public void closed()
        {
            _hubmanager.hub_closed(juggle.Imodule.current_ch);
            if (_hubmanager.checkAllHubClosed())
            {
                _svrmanager.close_db();
                center.closeHandle.is_close = true;
            }

            var _hubproxy = _hubmanager.get_hub(juggle.Imodule.current_ch);
            if (_hubproxy != null)
            {
                var _zone = _clutter.get_zone(_hubproxy.zone_id);
                if (_zone.is_closed)
                {
                    _zone.hub_closed(juggle.Imodule.current_ch);
                    if (_zone.checkAllHubClosed())
                    {
                        _zone.close_db();
                    }
                }
            }
        }

        private svrmanager _svrmanager;
        private hubmanager _hubmanager;
        private clutter _clutter;
    }
}
