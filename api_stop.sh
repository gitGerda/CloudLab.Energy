current_dir=$(pwd);
echo "RMI [api]";
cd ./reactproject1
docker-compose kill;
docker-compose rm;
docker rmi kzmpcloudapi;
cd $current_dir;
