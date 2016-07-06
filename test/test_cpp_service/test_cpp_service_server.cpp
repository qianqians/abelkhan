/*
 * qianqians
 * 2016-7-5
 * test_cpp_service_server.cpp
 */
#include <acceptnetworkservice.h>
#include <process_.h>
#include <Ichannel.h>
#include <Imodule.h>
#include <channel.h>
#include <juggleservice.h>
#include <timerservice.h>

#include <module/testmodule.h>

void handle_test(std::string argv0, int64_t argv1) {
	printf((argv0 + "%d \n").c_str(), argv1);
}

void main() {
	boost::shared_ptr<juggle::process> _process = boost::make_shared<juggle::process>();
	boost::shared_ptr<module::test> _test = boost::make_shared<module::test>();
	_test->sigtest_funchandle.connect(boost::bind(&handle_test, _1, _2));
	_process->reg_module(_test);

	boost::shared_ptr<service::acceptnetworkservice> _service = boost::make_shared<service::acceptnetworkservice>("127.0.0.1", 1234, _process);
	boost::shared_ptr<service::juggleservice> _juggleservice = boost::make_shared<service::juggleservice>();
	_juggleservice->add_process(_process);

	int64_t tick = clock();
	int64_t tickcount = 0;

	while (true)
	{
		int64_t tmptick = (clock() & 0xffffffff);
		if (tmptick < tick)
		{
			tickcount += 1;
			tmptick = tmptick + tickcount * 0xffffffff;
		}
		tick = tmptick;

		_service->poll(tick);
		_juggleservice->poll(tick);

		tmptick = (clock() & 0xffffffff);
		if (tmptick < tick)
		{
			tickcount += 1;
			tmptick = tmptick + tickcount * 0xffffffff;
		}
		int64_t ticktime = (tmptick - tick);
		tick = tmptick;

		Sleep(15);

	}
}
