## Build & Run （ionic3）
* 1.搭建ionic环境，步骤参见 [官方文档](https://ionicframework.com/docs/intro/installation/)
* 2.Clone Code 安装node包

    ```sh
    $ npm install
    ```

* 3.build project

    ```sh
    # 详细参数参见 ionic cordova build android/ios --help
    $ ionic cordova build android/ios
    ```

* 4.run project

    ```sh
    # 详细参数参见 ionic cordova run android/ios --help
    $ ionic cordova run android/ios
    ```

> 常见问题
#### Android (API27)
* 本项目基于ionic 3。直接运行`run`命令部署到Android较新模拟器或设备时可能报端口错误(据说已在ionic 4.x修复)是因为Google升级API导致，可以忽略。
* 若设备部署成功，但运行失败提示设备运行指令错误，可能是由于全面屏手机没有返回键导致，可以尝试开启虚拟返回键后重试。
* 虚拟机部署经常出现不稳定的情况，可以编译后直接拖到模拟器中运行，使用chrome进行调试(chrome://inspect)。
* Android真机安装后需要给予足够权限才能保证消息推送及时。不同android设置有所差异，建议开启后台刷新，并在电源选项给予足够权限

#### iOS （XCode10）
* iOS由于证书问题一般不可直接运行。可以先编译项目，完成后在Xcode中打开`platforms/iOS`目录中xcodeproj项目文件。
* 本项目涉及`Push Notification`，需要付费开发者证书才能编译部署，简单调试可以在Xcode项目设置中 `General` -> `Signing` 取消`Automatically manage signing`选项。然后在`Capbilities`选项中取消`Push Notification`，然后回到`General`重新勾选`Automatically manage signing`选项。
* 完成上述步骤后可以在模拟器中运行。如要在部署到iPhone真机则需要在XCode中进行如下设置：`File`->`Project Settings`->`Shared Project Settings`->`Build System`->`Legacy Build System`


## Publish & Deploy
### 1.Android
```
# 生成密钥，仅第一次
$ keytool -genkey -v -keystore lottery-release-key.jks -keyalg RSA -keysize 2048 -validity 10000 -alias lottery

# 打包,完成后将apk文件拷贝到密钥目录下
$ ionic cordova build android --prod --release
$ cp -i platforms/android/app/build/outputs/apk/release/app-release-unsigned.apk .

# 签名
$ jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore lottery-release-key.jks app-release-unsigned.apk lottery

# 优化
$ mv app-release-unsigned.apk /Users/zhangcheng/Library/Developer/Xamarin/android-sdk-macosx/build-tools/27.0.3
$ cd /Users/zhangcheng/Library/Developer/Xamarin/android-sdk-macosx/build-tools/27.0.3
$ ./zipalign -v 4 app-release-unsigned.apk pk10.apk

# 审核
$ ./apksigner verify pk10.apk

# 清理
$ mv pk10.apk ~/Desktop
$ rm -f app-release-unsigned.apk
```

## 2.iOS
```
$ ionic cordova build ios --prod --release
```
具体使用不同版本证书发布请流程请自行百度。


# My Opinion

 `ionic + cordova` 开发跨平台Hybrid App，使用Angular(TypeScript版)+Scss+Html配合，简单高效，但环境部署等环节新手经常容易出现问题，修复略显繁琐。国内相关资料也不是很多。

> 深夜发布，文档写的比较粗糙，任何环节出现问题请直接联系作者 zhangcheng5468@gmail.com
