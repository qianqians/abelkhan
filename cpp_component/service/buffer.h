/*
 * buffer.h
 *
 *  Created on: 2022-7-9
 *      Author: qianqians
 */
#ifndef _buffer_h
#define _buffer_h

#include <stdlib.h>

namespace service {

size_t get_buffer_size();
char* get_buffer(size_t _buffer_size);

}

#endif //_buffer_h
