/*
 * msque.hpp
 *		 Created on: 2012-8-30
 *			 Author: qianqians
 * msque:
 */

#ifndef _Test_H
#define _Test_H

#include <boost/thread.hpp>
#include <boost/bind.hpp>
#include <time.h>

bool end_flag[4];

struct _event {
	int thid;
	int num;
};

template<class T>
void push(T * que, int thid) {
	for (int i = 0; i < 1000000; i++) {
		_event ev;
		ev.thid = thid;
		ev.num = i;

		que->push(ev);
	}
	end_flag[thid] = true;
}

template<class T>
void pop(T * que) {
	int flag[4];
	for (int i = 0; i < 4; i++) {
		flag[i] = 0;
	}

	while (end_flag[0] == false || end_flag[1] == false || end_flag[2] == false || end_flag[3] == false) {
		_event ev;
		if (que->pop(ev)) {
			if (ev.num != flag[ev.thid]) {
				printf("thread %d error\n", ev.thid);
			}

			flag[ev.thid] = ev.num + 1;
		}
	}
}

template<class T>
void test() {
	T que;

	boost::thread_group th_group;

	for (int i = 0; i < 4; i++){
		end_flag[i] = false;
	}

	clock_t begin = clock();

	for (int i = 0; i < 4; i++) {
		th_group.create_thread(boost::bind(push<T>, &que, i));
	}
	th_group.create_thread(boost::bind(pop<T>, &que));
	
	th_group.join_all();

	clock_t time = clock() - begin;

	printf("test time %d\n", time);


}


#endif //_Test_H