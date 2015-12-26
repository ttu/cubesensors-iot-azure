#### Azure requirements

* Azure Web Jobs will execute first file, so thats why this is named aa_XXXX.py
  * Maybe possible to define in settings.job?
* sys.path.append is required by Azure Web Jobs. Web Jobs require that all packages are provided to it in the zip file.
  * env\Lib\site-packages is virtual env path in Windows

#### keys.py

keys.py is not in version control. Add it manually.

```py
cube_consumer_key = "VALUE"
cube_consumer_secret = "VALUE"

cube_oauth_verifier = "VALUE"
cube_access_token = "VALUE"
cube_access_token_secret = "VALUE"

azure_servicebus_namespace = "VALUE"
cubesender_private_key = "VALUE"

db_user = "VALUE"
db_password = "VALUE"
db_name = "VALUE"
db_server = "VALUE"
```

#### VirtualEnv (Ubuntu)

```sh
apt-get install python3-dev python-virtualenv
virtualenv -p /usr/bin/python3 env
source env/bin/activate
pip install -r requirements.txt
```

#### VirtualEnv (Windows)

When having multiple Python versions installed, define which one to use when creating VirtualEnv

```sh
C:\Python34\Scripts\virtualenv.exe env
env\Scripts\activate
pip install -r requirements.txt
python XXXX.py
env\Scripts\deactivate
```

#### Prerequirements (Ubuntu)

```sh
sudo apt-get install freetds-dev freetds-bin
```

#### Tests

Exeute tests with nose. Use -s if you want to see prints in output.

```sh
nosetests -s
```

Execute single test

```sh
nosetests -s tests/cube_parser_tests.py:CubeParserTests.test_current_real_data
```

#### Cron

Open crontab

```sh
crontab -e
```

Add these lines to the file (source requires bash)

```sh
SHELL=/bin/bash
* * * * * cd /home/stetson/python_parser && source env/bin/activate && python aa_azure_start.py
```
  
Cron log
```sh
grep CRON /var/log/syslog
 ```