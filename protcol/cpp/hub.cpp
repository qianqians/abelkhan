#include "hub.h"

namespace abelkhan
{

/*this caller code is codegen by abelkhan codegen for cpp*/
std::shared_ptr<hub_call_hub_rsp_cb> hub_call_hub_caller::rsp_cb_hub_call_hub_handle = nullptr;
std::shared_ptr<gate_call_hub_rsp_cb> gate_call_hub_caller::rsp_cb_gate_call_hub_handle = nullptr;
std::shared_ptr<client_call_hub_rsp_cb> client_call_hub_caller::rsp_cb_client_call_hub_handle = nullptr;
std::shared_ptr<hub_call_client_rsp_cb> hub_call_client_caller::rsp_cb_hub_call_client_handle = nullptr;

}
