# ablekhan
abelkhan是一个开源的游戏服务器框架。目标是提供一个稳定、高效、可扩展的服务器框架。

论坛:http://abelkhan.com/forum.php?mod=forumdisplay&fid=2  
博客:http://www.cnblogs.com/qianqians/  
QQ群:494405542

服务器框架采用分布式架构：

#center
全局唯一的单点，负责调度整个集群  
#dbproxy
数据库代理，集群的数据通过dbproxy存入数据库  
#hub
逻辑单点，可以配置多个，每个hub处理一种或多种逻辑上的单点服务  
#logic
逻辑服务器，支持动态扩展，为玩家提供逻辑服务  
#gate网关服务器，支持动态扩展，为玩家提供接入服务
  
服务器基于自研的RPC框架juggle(https://github.com/qianqians/abelkhan/tree/master/juggle)  
c#语言的反射机制实现了二级的RPC机制，易于扩展和二次开发。
  
abelkhan采用LGPL2协议开源(https://github.com/qianqians/abelkhan/blob/master/LICENSE)  
欢迎大家参与我们的项目。
  
技术支持:theDarkforce@aliyun.com  
QQ:451517996  
微信:451517996  
手机:15013470639
