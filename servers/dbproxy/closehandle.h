/*
 * closehandle.h
 *
 *  Created on: 2016-7-13
 *      Author: qianqians
 */
#ifndef _closehandle_h
#define _closehandle_h

namespace dbproxy{

class closehandle {
public:
	closehandle() {
		online_logic_num = 0;
		is_closed = false;
	}

	~closehandle(){
	}

	void reg_logic() {
		online_logic_num += 1;
	}

	void logic_closed() {
		online_logic_num -= 1;
	}

	bool is_close(){
		return (online_logic_num == 0) && is_closed;
	}

public:
	bool is_closed;

private:
	int64_t online_logic_num;

};

}

#endif // !_closehandle_h
