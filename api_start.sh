current_dir=$(pwd);
echo "Starting [api]";
cd ./reactproject1;
docker-compose up -d;
cd $current_dir;
