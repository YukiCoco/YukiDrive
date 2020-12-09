# 上传工具

## 使用方法

### 前言

目前处于 BETA 版本，将会是接下来开发的重心

### 变量介绍

站点名： SharePoint 创建时输入的名字，可在后台查看。输入 onedrive 则为 OneDrive。

### 初始化

*仅首次使用需要初始化*

`CLI密码` 应该在后台所设置

命令：`YukiDrive.CLI --init 网址 CLI密码`

示例：`./YukiDrive.CLI --init https://drive.yukino.co myPassword`

### 上传

#### 单文件

命令：`YukiDrive.CLI --upload 站点名 本地路径 远程路径`

示例：

`./YukiDrive.CLI --upload onedrive /Users/yukino/Desktop/upload.jpeg upload ` 将 upload.jpeg 上传到 OneDrive 的 upload 文件夹里

`./YukiDrive.CLI --upload YukiStudio /Users/yukino/Desktop/upload.jpeg upload/myimg.jpeg` 将 upload.jpeg 上传到SharePoint名为 YukiStudio 的 upload 文件夹里，并改名为 myimg.jpeg

#### 文件夹

多线程：每个线程将上传一个文件，建议不要超过 CPU 线程数，默认线程数为 1

命令：`YukiDrive.CLI --upload-folder 站点名 本地路径 远程路径 线程数`

示例：

`./YukiDrive.CLI --upload-folder onedrive /Users/yukino/Desktop/1.0.0 YukiDrive/WebApi/1.1.0 4` 使用4线程将文件夹 /Users/yukino/Desktop/1.0.0 上传到 OneDrive 的 YukiDrive/WebApi/1.1.0 文件夹里

