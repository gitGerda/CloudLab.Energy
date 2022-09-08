current_dir=$(pwd);
echo "Starting [database][phpMyAdmin]";
cd ./dockerLocalDeploy/mysql_phpMyAdmin/;
docker-compose up -d;
cd $current_dir;
echo "Starting [rabbitMQ]"
cd ./dockerLocalDeploy/rabbitmq/;
docker-compose up -d;
cd $current_dir;
echo "Waiting timeout 10 seconds";
sleep 10;
echo "Starting [hangfire]";
cd ./dockerLocalDeploy/hangfire/;
docker-compose up -d;
cd $current_dir;

