/*
 * closehandle.h
 *
 *  Created on: 2016-7-13
 *      Author: qianqians
 */
#ifndef _closehandle_h
#define _closehandle_h

namespace gate{

class closehandle {
public:
	closehandle() {
		is_closed = false;
	}

	virtual ~closehandle(){
	}

public:
	bool is_closed;

};

}

#endif // !_closehandle_h
