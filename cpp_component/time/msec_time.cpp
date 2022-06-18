#include "msec_time.h"
#include <chrono>

int64_t msec_time()
{
	return std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
}
