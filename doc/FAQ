# 常见问题

## Centos 7.x 认证后跳转回调地址出现 500

解决方案：

1. 查看 OPENSSLDIR 路径 $ openssl version -a  
2. 然后把 CentOS 默认的 openssl CA证书拷贝过来。$ cp /etc/pki/tls/cert.pem /usr/local/openssl/
