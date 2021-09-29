#ifndef _msec_time_h
#define _msec_time_h

#include <boost/date_time.hpp>

inline int64_t msec_time()
{
	boost::posix_time::ptime epoch(boost::gregorian::date(1970, boost::gregorian::Jan, 1));
	boost::posix_time::time_duration time_from_epoch = boost::posix_time::microsec_clock::universal_time() - epoch;

	return time_from_epoch.total_milliseconds();
}


#endif
