﻿using CommonUtil.Route;
using CommonUtil.View;
using System;
using System.Collections.Generic;

namespace CommonUtil.Store {
    public class Global {
        public static readonly string AppTitle = "工具集";
        public static readonly string ImagePath = "/Resource/image/";

        public static readonly List<ToolMenuItem> MenuItems = new() {
            new() { Name = "Base64 编码/解码", RouteView = MainWindowRouter.RouterView.Base64Tool, ImagePath = ImagePath + "base64.svg", ClassType = typeof(Base64ToolView) },
            new() { Name = "随机数/字符串生成器", RouteView = MainWindowRouter.RouterView.RandomGenerator, ImagePath = ImagePath + "random.svg", ClassType = typeof(RandomGeneratorView) },
            new() { Name = "简体繁体转换", RouteView = MainWindowRouter.RouterView.ChineseTransform, ImagePath = ImagePath + "ChineseTransform.svg", ClassType = typeof(ChineseTransformView) },
        };
    }

    public class ToolMenuItem {
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public MainWindowRouter.RouterView RouteView { get; set; } = MainWindowRouter.RouterView.MainContent;
        public Type ClassType { get; set; } = typeof(MainContentView);
    }
}
