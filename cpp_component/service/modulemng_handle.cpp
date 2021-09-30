/*
 * modulemng_handle.h
 *
 *  Created on: 2021-9-29
 *      Author: qianqians
 */
#include "modulemng_handle.h"

namespace service {

std::shared_ptr<abelkhan::modulemng> _modulemng = std::make_shared<abelkhan::modulemng>();

}