## Ablekhan
abelkhan是一个轻量级的游戏服务器框架，基于c#开发，使用mongodb提供数据存储，支持c#和typescript前端，目标是提供一个稳定、高效、可扩展的服务器框架。  

## 架构
* 基于consul实现服务发现
* 使用protobuf进行序列化/反序列化  
&nbsp;
* dbproxy 数据库代理
* gate 网关服务器
* hub 微服务节点（含 `hub/Inventory` 抽象背包：数组存储、物品接口、邮件附件发放、BSON 存档，见 `docs/inventory.md`）
* admin 管理后台（dashboard：监控大盘/玩家查询/支付查询/GM面板/邮件发送，见 abelkhan/admin/README.md）

## License
[LGPL-2.1](https://github.com/qianqians/abelkhan/blob/master/LICENSE)
  
## 技术支持
theDarkforce@aliyun.com  

## QQ群
494405542
