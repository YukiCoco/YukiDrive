FROM ubuntu
RUN apt update
RUN apt install -y nano vim sudo wget curl unzip
RUN wget -O /root/YukiDrive.zip https://github.com/PIKACHUIM/YukiDrive/releases/download/v1.1.3-bin/YukiDrive-Linux-x86-64.zip
RUN cd /root/&& unzip YukiDrive.zip
RUN mkdir -p /data
RUN mv /root/YukiDrive.db /data/YukiDrive.db
RUN ln -s /data/YukiDrive.db /root/YukiDrive.db
CMD cd /root/ && nohup /root/YukiDrive