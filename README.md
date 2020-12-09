# YukiDrive

一个 Onedrive &amp; SharePoint 文件浏览程序，支持国际版和世纪互联版。

后端采用 .net core 3.1，前端使用 Vue ，前后端分离，无刷新加载。

无需搭建运行环境，下载并配置完成后直接运行。

## 演示
Demo：https://drive.kurisu.moe  
Telegram 交流群：https://t.me/yukidrive

## Feature
+ .Net Core 多线程高并发  
+ 前后端分离，无刷新加载  
+ 可挂载 OneDrive 和任意多个 SharePoint 站点  
+ 提供文件上传 CLI
+ 上传文件
    + 无大小限制
    + 由 浏览器&CLI 直接对微软服务器上传，不消耗流量

## 部署

1. 查看 [安装文档](https://github.com/YukiCoco/YukiDrive/blob/master/doc/Usage.md)  
2. 上传工具 [使用方法](https://github.com/YukiCoco/YukiDrive/blob/master/doc/CLI-Usage.md)
3. 遇到问题请查看 [常见问题](https://github.com/YukiCoco/YukiDrive/blob/master/doc/FAQ.md)
4. 安装更新 [方法链接](https://github.com/YukiCoco/YukiDrive/blob/master/doc/Install-Update.md)

## TODO

+ 文件上传命令行接口（已完成）
+ 网页文件上传（已完成）
+ 离线下载
+ 多账户支持
+ ...

## 引用  

### 前端

+ vue
+ vuex
+ vuetify
+ vue-router
+ vue-meditor
+ dplayer
+ clipboard
+ js-cookie

### Nuget Packages

+ Microsoft.EntityFrameworkCore.Sqlite
+ Microsoft.Extensions.Configuration
+ Microsoft.Graph
+ Microsoft.Identity.Client
+ Microsoft.IdentityModel.Tokens
+ NLog

## License

Licensed under the [GPL](https://github.com/YukiCoco/YukiDrive/blob/master/LICENSE) license.


