/*
 * qianqians
 * 2022-7-9
 * buffer.cpp
 */
#include "buffer.h"

#include <stdlib.h>

namespace service {

static char* buffer = nullptr;
static size_t buffer_size = 0;

size_t get_buffer_size() {
	return buffer_size;
}

char* get_buffer(size_t _buffer_size) {
	if (_buffer_size > buffer_size) {
		if (buffer) {
			free(buffer);
		}
		buffer_size = ((_buffer_size + 1023) / 1024) * 1024;
		buffer = (char*)malloc(buffer_size);
	}
	return buffer;
}

}