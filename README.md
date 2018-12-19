# Colin.Lottery
彩票策略分析与自动投注

## Docker 命令备忘
```sh
# 开发
$ docker run --rm -it -p 8000:80 -v ~/Desktop/lottery/:/app/ -w /app/Colin.Lottery.WebApp microsoft/dotnet:2.2-sdk dotnet watch run

# 编译
$ docker build --pull -t lottery .
# 测试
$ docker run --name lottery --rm -it -p 8000:80 lottery
# 生产
$ docker run --name lottery -d -p 8000:80 lottery
```

## bug
1.相同号码追号中断

2.ChaseTimes错误导致下注金额错