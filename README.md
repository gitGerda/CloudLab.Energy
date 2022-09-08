# CloudLab.Energy

CloudLab.Energy - это автоматизированная система контроля учета электроэнергии, разработанная для контроля учета электроэнергии на предприятии АО "Кукморский 
Завод Металлопосуды". 

CloudLab.Energy разработан с использованием следующего стека технологий:
  
    Backend:
      - ASP.NET Core
      - Entity Framework Core
      - RabbitMQ (брокер сообщений)
      - Hangfire (планировщик задач)  
      
    TDD (Test-Driven Development)
      - xUnit
      - Moq
    
    Frontend:
      - HTML, CSS, JavaScript
      - ReactJS
    
    Database: 
      - MySQL  
    
    Deploy:
      - Docker
      - Docker Compose
  
> Полезные ссылки: [Документация Docker](https://docs.docker.com/) | [Документация Docker Compose](https://docs.docker.com/compose/) | [Документация RabbitMQ](https://www.rabbitmq.com/documentation.html) | [Hangfire](https://www.hangfire.io/)

---

CloudLab.Energy имеет следующую архитектуру:

![image](https://user-images.githubusercontent.com/42517716/188893783-0601ae9b-2723-475c-9116-26b4deb54e0a.png)

> На рисунке изображена самая простая схема развертывания АСКУЭ. Здесь сервер (Server) по средством брокера сообщений (RabbitMQ) общается с удаленными станциями 
  опроса (Mesaurement station). Станция опроса - это обычный компьютер на который устанавливается служба (Data Collection Service) считывания и обработки 
  показаний электроэнергии. 
  
  >>**Примечание: станций опроса, так же как и модемов подключенных к этой станции может быть множество. Так же станцией опроса может выступать и сам сервер.
  
---

Сервер CloudLab.Energy имеет следующую архитектуру:

![image](https://user-images.githubusercontent.com/42517716/189042108-4e19b552-0446-4d5a-b7ea-675d5be36e76.png)

> Так же в качестве обратного прокси-сервера можно использовать Nginx. Более подробно описано здесь: https://docs.microsoft.com/ru-ru/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-6.0#configure-a-reverse-proxy-server


Более детальную информацию по настройке файлов развертывания docker-compose можно найти здесь https://github.com/gitGerda/react1/tree/master/dockerLocalDeploy.

Перед запуском веб-сервера необходимо заполнить некоторые поля в файле https://github.com/gitGerda/react1/blob/master/reactproject1/kzmpCloudAPI_Publish/appsettings.json.
```json
    "DefaultConnection": "", // строка подключения к базе данных MySQL
   
    "HANGFIRE_STORAGE_CONN_STR": "", // строка подключения к базе данных планировщика Hangfire

    "LAST_DATETIME_REQUEST": "http://192.168.0.64:80/api/indications/PowerIndications/get_last_indic_datetime?meter_address=", // строка запроса даты последнего измерения (необходимо изменить адрес сервера. Остальное трогать не надо.) 
    "DEFAULT_QUEUE_NAME": "cloudlab_energy_indications_queue", // очередь RabbitMQ
    "DEFAULT_EXCHANGE_NAME": "cloudlab_energy_indications_ex", // обменник RabbitMQ
    "RABBITMQ_SERVER_NAME": "", // адрес сервера RabbitMQ
    "RABBITMQ_USER_NAME": "", // имя пользователя RabbitMQ
    "RABBITMQ_USER_PASS": "", // пароль

    "ADDRESS_DOWNLOADING": "", // адрес сервера в виде http://[адрес сервера]
```
    
Так же необходимо проинициализировать некоторые поля в файле appsettings.json (https://github.com/gitGerda/react1/blob/master/reactproject1/hangfireSheduleAPI/publish_api/appsettings.json) планировщика задач Hangfire:

```json
    "DefaultConnection": "", // строка подключения к базе данных планировщика Hangfire
    "RABBITMQ_SERVER_NAME": "", // адрес сервера RabbitMQ
    "RABBITMQ_USER_NAME": "", // имя пользователя RabbitMQ
    "RABBITMQ_USER_PASS": "" // пароль
```
---
Запуск сервера MySQL, phpMyAdmin и Hangfire можно выполнить следующим образом (https://github.com/gitGerda/react1/blob/master/start.sh)

```sh
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
sleep 10; // таймаут необходимый для того, чтобы база данных успела подняться до того, как начнёт подниматься планировщик Hagfire
echo "Starting [hangfire]";
cd ./dockerLocalDeploy/hangfire/;
docker-compose up -d;
cd $current_dir;
```

Остановка MySQL, phpMyAdmin и Hangfire (https://github.com/gitGerda/react1/blob/master/stop.sh)

```sh
current_dir=$(pwd);
echo "RM [database][phpMyAdmin]";
cd ./dockerLocalDeploy/mysql_phpMyAdmin;
docker-compose kill;
docker-compose rm;
cd $current_dir;
echo "RM [rabbitMQ]";
cd ./dockerLocalDeploy/rabbitmq;
docker-compose kill;
docker-compose rm;
cd $current_dir;
echo "RM and RMI [hangfireShedule]";
cd ./dockerLocalDeploy/hangfire;
docker-compose kill;
docker-compose rm;
docker rmi hangfiresheduleapi;
cd $current_dir;
```
---

Запуск веб-сервера необходимо выполнять после того, как будут запущены MySQL, phpMyAdmin и Hangfire (https://github.com/gitGerda/react1/blob/master/api_start.sh)
```sh
current_dir=$(pwd);
echo "Starting [api]";
cd ./reactproject1;
docker-compose up -d;
cd $current_dir;
```
Остановка веб-сервера (https://github.com/gitGerda/react1/blob/master/api_stop.sh)

```sh
current_dir=$(pwd);
echo "RMI [api]";
cd ./reactproject1
docker-compose kill;
docker-compose rm;
docker rmi kzmpcloudapi;
cd $current_dir;
```

Установочное приложение службы считывания и обработки показаний электроэнергии расположено по этому пути https://github.com/gitGerda/react1/tree/master/workerService/Installers. Запустите исполняемый файл CloudlabDataCollectServiceSetup.exe для установки службы. После установки в директории, которая была выбрана в качестве корневой вы можете найти папку Cloudlab\DataCollectService\Installers в которой расположен PowerShell скрипт install.ps1. Запустите этот скрипт с помощью консоли PowerShell для регистрации службы в системе. 

![image](https://user-images.githubusercontent.com/42517716/189044440-03c69395-5ada-45be-aa22-b01cdb8fb089.png)

Перед регистрацией службы или после (необходима перезагрузка службы) необходимо заполнить следующие поля в файле appsettings.json :

```json
      "APP_CONFIG_FILE_PATH": "./appsettings.json", // путь к файлу appsettings.json 
      "DEFAULT_CONSUMER_QUEUE_NAME": "", // имя очереди RabbitMQ для получения сообщений от сервера
      "DEFAULT_PUBLISHER_ROUTING_KEY": "cloudlab_energy_indications_queue", // routing key для публикации сообщений
      "DEFAULT_PUBLISHER_QUEUE_NAME": "cloudlab_energy_indications_queue", // очередь RabbitMQ из которой сервер будет получать сообщения от службы
      "DEFAULT_EXCHANGE_NAME": "cloudlab_energy_indications_ex", // имя обменника RabbitMQ для отправки сообщений на сервер
      "RABBITMQ_SERVER_NAME": "", // IP-адрес или имя сервера RabbitMQ
      "RABBITMQ_USER_NAME": "", // имя пользователя для подключения к серверу RabbitMQ
      "RABBITMQ_USER_PASS": "", // пароль 
      "DEFAULT_COM_PORT": "", // COM-порт модема
      "REPEATS_COUNT_ON_FAILURE": 50, // количество повторных попыток чтения показаний с счётчиков при неудачных опросах
      "TIMEOUT_AFTER_FAILURE_MS": 60000 // таймаут после недачного опроса
```

Контроль службы происходит с помощью оснастки services.msc: 
![image](https://user-images.githubusercontent.com/42517716/189054289-a55fb445-6ce0-4d71-a8f0-ef9791c72ba3.png)

---
#### Cкриншоты
---
![image](https://user-images.githubusercontent.com/42517716/189059423-c41e9f2d-e3c3-4bd9-ab54-8e9d16322ecf.png)
---
![image](https://user-images.githubusercontent.com/42517716/189059781-df3b9a43-e3d0-4f28-8eb7-1d6bde47f147.png)
---
![image](https://user-images.githubusercontent.com/42517716/189059854-39f6269e-fec4-4bc0-8e6d-996e6ced80e6.png)
---
![image](https://user-images.githubusercontent.com/42517716/189059934-0a93154e-dcf9-468d-a86d-bc2629efe64f.png)
---
![image](https://user-images.githubusercontent.com/42517716/189060078-80989f23-d96f-4750-b474-bd018b10a058.png)
---
![image](https://user-images.githubusercontent.com/42517716/189060141-ce8fff27-ed84-4739-8aff-6809362c4fe9.png)
---
![image](https://user-images.githubusercontent.com/42517716/189060262-929f570d-c200-4af0-8911-b038bfcb0bc7.png)
---
![image](https://user-images.githubusercontent.com/42517716/189060374-6246a98d-be37-447a-9ffc-b0b5f3ed6d26.png)
---
![image](https://user-images.githubusercontent.com/42517716/189060445-7d678553-38b0-4216-80c0-44f6eaa5b1f0.png)
---
![image](https://user-images.githubusercontent.com/42517716/189060512-d750dd19-758c-473a-8344-b406034b8806.png)
---
![image](https://user-images.githubusercontent.com/42517716/189060603-329ae7de-6c75-493c-bb7b-badd619e7e31.png)

