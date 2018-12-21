# Colin.Lottery
彩票策略分析与自动投注

## Docker方式部署项目

### 1.安装Docker

**CentOS 7安装Docker CE**

```sh
$ yum remove docker
$ yum remove docker-client
$ yum remove docker-client-latest
$ yum remove docker-common
$ yum remove docker-latest
$ yum remove docker-latest-logrotate
$ yum remove docker-logrotate
$ yum remove docker-selinux
$ yum remove docker-engine-selinux
$ yum remove docker-engine

$ yum install -y yum-utils
$ yum install device-mapper-persistent-data
$ yum install lvm2

$ yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo
$ yum install docker-ce
$ systemctl start docker
```

### 2.Asp.net Core Docker
```sh
# 构建镜像
$ docker build --pull -t lottery .

# 运行容器
$ docker run --name lottery -d --restart always lottery
```

### 3.Nginx Docker
```sh
# 获取nginx官方镜像
docker pull nginx

# 启动nginx容器
sudo docker run \
--name nginx \
-d \
-p 8000:80 \
-v ~/nginx/default.conf:/etc/nginx/conf.d/default.conf \
--link lottery:web \
nginx

# 查看服务日志
docker logs nginx
```

>  default.conf
```json
server {
    listen        80; 
    server_name   104.199.230.207 bet518.win www.bet518.win;
    location / {
        proxy_pass         http://web;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

### 4.常用Docker命令
```sh
# 查看镜像列表
$ docker images

# 查看容器列表(运行中)
$ docker ps

# 删除镜像
$ docker image rm image-name/image-id

# 删除容器
$ docker rm cotainer-name/container-id

# 启动/停止/重启容器
$ docker start/stop/restart cotainer-name/container-id

# 进入docker容器(后台)
$ docker exec -it cotainer-name/container-id /bin/bash
# Docker容器内部是一个微型Linux系统
```

## bug
1.相同号码追号中断

2.ChaseTimes错误导致下注金额错
