using System;
using System.Collections.Generic;

namespace service
{
	public class juggleservice : service
    {
        public void add_process(process _process)
        {
            process_set.Add(_process);
        }
        
        public void poll(Int64 tick)
        {
            foreach (process p in process_set)
            {
                p.poll();
            }
        }

        private List<process> process_set;
    }
}
