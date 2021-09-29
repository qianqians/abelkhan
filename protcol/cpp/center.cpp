#include "center.h"

namespace abelkhan
{

/*this caller code is codegen by abelkhan codegen for cpp*/
std::shared_ptr<center_call_hub_rsp_cb> center_call_hub_caller::rsp_cb_center_call_hub_handle = nullptr;
std::shared_ptr<center_call_server_rsp_cb> center_call_server_caller::rsp_cb_center_call_server_handle = nullptr;
std::shared_ptr<center_rsp_cb> center_caller::rsp_cb_center_handle = nullptr;
std::shared_ptr<gm_center_rsp_cb> gm_center_caller::rsp_cb_gm_center_handle = nullptr;

}
