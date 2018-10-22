## 版本发布
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

$ ionic cordova build ios --prod --release
