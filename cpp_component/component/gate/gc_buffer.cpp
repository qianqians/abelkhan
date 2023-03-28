/*
 * qianqians
 * 2023-3-28
 * gc_buffer.cpp
 */
#include <gc.h>
#include "buffer.h"

namespace gate {

char* gc_get_buffer(size_t _buffer_size) {
	return (char*)GC_malloc(_buffer_size);
}

}