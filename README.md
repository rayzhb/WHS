# WHS

插件式应用，为web而生

[English Document](https://whs.ray-zhb.com/en/)

[中文文档](https://whs.ray-zhb.com)

[NET5](https://github.com/rayzhb/WHS/tree/net5)

## 介绍

WHS目的是为了让硬件操作变的简单。插件式加载各类子程序。
使用websocket双工通信，让WEB,PAD,MOBILE,DESKTOP的应用都能访问硬件，同时也让硬件访问的接口统一简单化

## 它是如何工作的

查看[程序目录结构](https://whs.ray-zhb.com/guide.md#程序目录结构)。

WHS属于主程序，所有插件的开发都保存在**Plugins**文件夹

## 安装插件模版

程序目录下有2个批处理文件

1. InstallTemplate.bat
2. UninstallTemplate.bat

双击InstallTemplate.bat，安装成功后。可以使用

```cmd
dotnet new -l
```
查看是否安装成功，会存在一个名叫：**WHS5**的模板


# 快速上手

## 快速开发插件

### 步骤1
进入**Plugins**文件夹，使用以下命令
```cmd
dotnet new WHS5 -n  WHS.HelloWord -D WHS.HelloWord -M rayzhb
```

::: tip

-n 命名空间
-D 插件显示名字（在多语言中会被替换）
-M 开发者

:::
### 步骤2
将 WHS.HelloWord，加入到解决方案下的plugins

点击WHS.HelloWord生成会发现编译出错。

进入DevicePluginDefinition.cs [插件目录结构](https://whs.ray-zhb.com/guide.md#插件目录结构)
```cs 

 public override Guid Id
        {
            get
            {
                //按照下面生成一个GUID
                //return new Guid("xxxxxxxxxxxxxxxxxxx");
            }
        }

```
修改后编译成功
### 步骤3
运行程序后插件的名称并不叫WHS.HelloWord，这是因为 [多语言](https://whs.ray-zhb.com/guide.md#多语言)已经修改名称。

可以在插件中的resouces文件夹中修改PluginDisplayText对应的值

空白插件创建完毕
