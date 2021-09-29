/*
 * closehandle.h
 *
 *  Created on: 2016-7-13
 *      Author: qianqians
 */
#ifndef _closehandle_h
#define _closehandle_h

namespace hub{

class closehandle {
public:
	closehandle() {
		is_closed = false;
	}

	~closehandle(){
	}

public:
	bool is_closed;

};

}

#endif // !_closehandle_h
