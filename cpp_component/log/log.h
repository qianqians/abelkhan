/*
 * log.h
 *
 *  Created on: 2020-1-27
 *      Author: qianqians
 */
#ifndef _log_h
#define _log_h

#include <spdlog/spdlog.h>
#include <spdlog/sinks/rotating_file_sink.h>

namespace _log {

extern std::shared_ptr<spdlog::logger> file_logger;

inline void InitLog(std::string file_path, spdlog::level::level_enum log_level) {
    spdlog::set_level(log_level);
    spdlog::set_pattern("[%H:%M:%S %z] [%n] [%^---%L---%$] [thread %t] %v");

    file_logger = spdlog::rotating_logger_mt("basic_logger", file_path, 64*1024*1024, 5);
    spdlog::set_default_logger(file_logger);
}

} /* namespace log */

#endif //_log_h
