## Build & Run （ionic3）
* 1.搭建ionic环境，步骤参见 [官方文档](https://ionicframework.com/docs/intro/installation/)
* 2.Clone Code 安装node包

    ```sh
    $ npm install
    ```

* 3.build project

    ```sh
    # 详细参数参见 ionic cordova build android/ios --help
    $ ionic cordova build android/ios
    ```

* 4.run project

    ```sh
    # 详细参数参见 ionic cordova run android/ios --help
    $ ionic cordova run android/ios
    ```

> 常见问题
#### Android (API27)
* 本项目基于ionic 3。直接运行`run`命令部署到Android较新模拟器或设备时可能报端口错误(据说已在ionic 4.x修复)是因为Google升级API导致，可以忽略。
* 若设备部署成功，但运行失败提示设备运行指令错误，可能是由于全面屏手机没有返回键导致，可以尝试开启虚拟返回键后重试。
* 虚拟机部署经常出现不稳定的情况，可以编译后直接拖到模拟器中运行，使用chrome进行调试(chrome://inspect)。
* Android真机安装后需要给予足够权限才能保证消息推送及时。不同android设置有所差异，建议开启后台刷新，并在电源选项给予足够权限

#### iOS （XCode10）
* iOS由于证书问题一般不可直接运行。可以先编译项目，完成后在Xcode中打开`platforms/iOS`目录中xcodeproj项目文件。
* 本项目涉及`Push Notification`，需要付费开发者证书才能编译部署，简单调试可以在Xcode项目设置中 `General` -> `Signing` 取消`Automatically manage signing`选项。然后在`Capbilities`选项中取消`Push Notification`，然后回到`General`重新勾选`Automatically manage signing`选项。
* 完成上述步骤后可以在模拟器中运行。如要在部署到iPhone真机则需要在XCode中进行如下设置：`File`->`Project Settings`->`Shared Project Settings`->`Build System`->`Legacy Build System`


## Publish & Deploy
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
$ cp -i app-release-unsigned.apk /Users/zhangcheng/Library/Developer/Xamarin/android-sdk-macosx/build-tools/27.0.3
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
具体使用不同版本证书发布请流程请自行百度。


# My Opinion

 `ionic + cordova` 开发跨平台Hybrid App，使用Angular(TypeScript版)+Scss+Html配合，简单高效，但环境部署等环节新手经常容易出现问题，修复略显繁琐。国内相关资料也不是很多。

> 深夜发布，文档写的比较粗糙，任何环节出现问题请直接联系作者 zhangcheng5468@gmail.com