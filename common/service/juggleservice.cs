using System;
using System.Collections.Generic;

namespace service
{
	public class juggleservice : service
    {
		public juggleservice()
		{
			process_set = new List<juggle.process>();
		}
		
        public void add_process(juggle.process _process)
        {
            process_set.Add(_process);
        }
        
        public void poll(Int64 tick)
        {
            foreach (juggle.process p in process_set)
            {
                p.poll();
            }
        }

        private List<juggle.process> process_set;
    }
}
