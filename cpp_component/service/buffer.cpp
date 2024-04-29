/*
 * qianqians
 * 2022-7-9
 * buffer.cpp
 */
#include "buffer.h"

#include <stdlib.h>

#include <gc.h>

namespace service {

static char* buffer = nullptr;
static size_t buffer_size = 0;

size_t get_buffer_size() {
	return buffer_size;
}

char* get_buffer(size_t _buffer_size) {
	if (_buffer_size > buffer_size) {
		buffer_size = ((_buffer_size + 16383) / 16384) * 16384;
		buffer = (char*)GC_malloc(buffer_size);
	}
	return buffer;
}

}